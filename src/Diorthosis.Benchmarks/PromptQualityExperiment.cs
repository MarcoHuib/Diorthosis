using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;
using Diorthosis.Experiments;

namespace Diorthosis.Benchmarks;

internal static class PromptQualityExperiment
{
    public static async Task<int> RunAsync(string[] args, JsonSerializerOptions options)
    {
        if (args.Length != 8) throw new ArgumentException("prompt-quality <panel.jsonl> <local.gguf> <provenance.json> <Representation:Instruction,...> <NEW-output-directory> <freeze.json> <source-identity.json>");
        var cases = DatasetCommands.ReadCases(args[1], options);
        var audit = DatasetAudit.Analyze(cases);
        if (cases.Length == 0 || audit.AnnotationIssues.Count != 0 || audit.DuplicateRecords != 0) throw new InvalidDataException("Invalid panel.");
        using var freeze = JsonDocument.Parse(File.ReadAllText(args[6]));
        if (freeze.RootElement.GetProperty("files").GetProperty(Path.GetFileName(args[1])).GetString() != Hash(args[1])) throw new InvalidDataException("Panel is not frozen.");
        using var provenance = JsonDocument.Parse(File.ReadAllText(args[3]));
        using var software = JsonDocument.Parse(File.ReadAllText(args[7]));
        if (provenance.RootElement.GetProperty("modelSha256").GetString() != Hash(args[2]) ||
            provenance.RootElement.GetProperty("modelSha256").GetString() != "9ed150d4367e68df0ac8e1540f6ddc65b42d0ee26378329d1ecbca60f93fc5f8")
            throw new InvalidDataException("Requires previously reviewed official Ministral 3B weights.");
        var arms = args[4].Split(',').Select(s => s.Split(':')).Select(s => s.Length == 2 ?
            (Representation: Enum.Parse<InputRepresentation>(s[0], false), Instruction: Enum.Parse<ProofreadingInstruction>(s[1], false)) : throw new ArgumentException("Invalid arm.")).ToArray();
        if (arms.Length == 0 || arms.Distinct().Count() != arms.Length) throw new ArgumentException("Empty or duplicate arms.");
        foreach (var arm in arms) _ = PromptQuality.Version(arm.Representation, arm.Instruction);
        if (Directory.Exists(args[5]) || File.Exists(args[5])) throw new IOException("Output must be new.");
        Directory.CreateDirectory(args[5]);
        using var process = Process.GetCurrentProcess();
        var started = DateTimeOffset.UtcNow;
        long peak = 0, attemptPeak = 0;
        void Sample()
        {
            process.Refresh(); var rss = process.WorkingSet64;
            static void Update(ref long target, long value) { long old; do { old = Volatile.Read(ref target); if (old >= value) return; } while (Interlocked.CompareExchange(ref target, value, old) != old); }
            Update(ref peak, rss); Update(ref attemptPeak, rss);
        }
        await using var timer = new Timer(_ => Sample(), null, 0, 50);
        var native = Path.Combine(AppContext.BaseDirectory, "runtimes", RuntimeInformation.RuntimeIdentifier, "native");
        var binaries = Directory.GetFiles(native).Where(p => p.EndsWith(".dylib", StringComparison.Ordinal) || p.EndsWith(".dll", StringComparison.Ordinal) || p.EndsWith(".so", StringComparison.Ordinal)).Order(StringComparer.Ordinal).ToDictionary(p => Path.GetFileName(p)!, Hash);
        var assemblyHashes = new[] { typeof(PromptQualityExperiment).Assembly.Location, typeof(PromptQuality).Assembly.Location, typeof(ModelCorrectionEngine).Assembly.Location }.ToDictionary(p => Path.GetFileName(p)!, Hash);
        var completed = false; var attempts = 0; var failures = 0;
        void Save(string name, object value) => File.WriteAllText(Path.Combine(args[5], name), JsonSerializer.Serialize(value, options) + "\n");
        void Manifest() => Save("manifest.json", new { version = "phase4-prompt-quality-1.0.0", state = completed ? "complete" : "incomplete", startedUtc = started,
            updatedUtc = DateTimeOffset.UtcNow, datasetSha256 = Hash(args[1]), freezeSha256 = Hash(args[6]), softwareIdentity = software.RootElement.Clone(),
            provenance = provenance.RootElement.Clone(), modelBytes = new FileInfo(args[2]).Length, runtime = LlamaSharpInferenceAdapter.RuntimeVersion,
            llamaCppRevision = "3f7c29d318e317b63f54c558bc69803963d7d88c", nativeHashes = binaries, assemblyHashes,
            os = RuntimeInformation.OSDescription, architecture = RuntimeInformation.ProcessArchitecture.ToString(), framework = RuntimeInformation.FrameworkDescription,
            cpu = Environment.GetEnvironmentVariable("DIORTHOSIS_EXPERIMENT_CPU") ?? "unknown", ramBytes = Environment.GetEnvironmentVariable("DIORTHOSIS_EXPERIMENT_RAM_BYTES") ?? "unknown",
            settings = new { context = 4096, threads = 9, gpuLayers = 0, maximumOutputTokens = 256, timeoutSeconds = 90, decoding = "greedy", validation = "unchanged Phase 3 head 6f78219", order = "case-major; arm rotation by case index" },
            arms = arms.Select(a => new { representation = a.Representation, instruction = a.Instruction, promptVersion = PromptQuality.Version(a.Representation, a.Instruction) }),
            cases = cases.Length, attempts, failures, sampledPeakProcessWorkingSetBytes = peak, memorySamplingMilliseconds = 50,
            limitations = "AI-authored labels; no human validation. Process RSS includes model/cache/runtime; sampled not isolated allocations. Prompt tokens exclude chat-template/BOS. No warmup; file cache uncontrolled." });
        Manifest();
        try
        {
            var load = Stopwatch.StartNew();
            using var adapter = new LlamaSharpInferenceAdapter(args[2], threads: 9);
            load.Stop(); Save("load.json", new { milliseconds = load.Elapsed.TotalMilliseconds });
            using var trace = new StreamWriter(Path.Combine(args[5], "trace.jsonl"), false);
            var predictions = arms.ToDictionary(a => PromptQuality.Version(a.Representation, a.Instruction), _ => new List<BenchmarkPrediction>());
            var dispositions = arms.ToDictionary(a => PromptQuality.Version(a.Representation, a.Instruction), _ => new Dictionary<string, CorrectionDisposition>());
            for (var index = 0; index < cases.Length; index++)
            {
                var item = cases[index];
                var request = new CorrectionRequest(item.Language, item.InputText);
                foreach (var offset in Enumerable.Range(0, arms.Length))
                {
                    var arm = arms[(index + offset) % arms.Length];
                    var version = PromptQuality.Version(arm.Representation, arm.Instruction);
                    var prompt = PromptQuality.Build(request, arm.Representation, arm.Instruction);
                    var tokens = adapter.CountPromptContentTokens(prompt);
                    Interlocked.Exchange(ref attemptPeak, 0); Sample();
                    var watch = Stopwatch.StartNew();
                    InferenceResult? inference = null; ModelResponseAnalysis analysis; string? failure = null;
                    try
                    {
                        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(90));
                        inference = await adapter.GenerateAsync(new(prompt, 256), timeout.Token);
                        analysis = ModelCorrectionEngine.AnalyzeInference(inference, request);
                    }
                    catch (Exception exception) when (exception is OperationCanceledException or InvalidDataException or InvalidOperationException)
                    {
                        failure = exception.GetType().Name + ": " + exception.Message;
                        analysis = ModelResponseAnalysis.Rejected("", null, [], "inference-failure", failure); failures++;
                    }
                    watch.Stop(); Sample(); attempts++;
                    var delivered = SourceEdits.Apply(item.InputText, analysis.Edits);
                    await trace.WriteLineAsync(JsonSerializer.Serialize(new { item.Id, item.Language, input = item.InputText, promptVersion = version,
                        representation = arm.Representation, instruction = arm.Instruction, prompt, promptSha256 = DatasetAudit.Hash(prompt),
                        promptContentTokens = tokens, inference, generationTermination = inference?.Termination.ToString() ?? (failure?.StartsWith("OperationCanceledException", StringComparison.Ordinal) == true ? "Cancelled" : "Failure"),
                        sampledTokens = inference?.SampledTokens ?? adapter.LastSampledTokenCount, failure, analysis, delivered,
                        latencyMilliseconds = watch.Elapsed.TotalMilliseconds, sampledPeakProcessWorkingSetBytes = attemptPeak }, options));
                    await trace.FlushAsync();
                    predictions[version].Add(new(item.Id, analysis.Edits)); dispositions[version][item.Id] = analysis.Disposition;
                    Console.WriteLine($"{attempts}/{cases.Length * arms.Length} {item.Id} {version}: {analysis.Disposition} {watch.ElapsedMilliseconds}ms");
                }
            }
            foreach (var pair in predictions)
            {
                File.WriteAllLines(Path.Combine(args[5], pair.Key + "-predictions.jsonl"), pair.Value.Select(p => JsonSerializer.Serialize(p, options)));
                Save(pair.Key + "-reference-diagnostics.json", ReferenceEvaluator.Evaluate(cases, pair.Value, dispositions[pair.Key]));
            }
            completed = true; return failures == 0 ? 0 : 1;
        }
        finally { Manifest(); }
    }
    private static string Hash(string path) { using var file = File.OpenRead(path); return Convert.ToHexString(SHA256.HashData(file)).ToLowerInvariant(); }
}
