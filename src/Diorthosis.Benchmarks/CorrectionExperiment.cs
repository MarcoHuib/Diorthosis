using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;

namespace Diorthosis.Benchmarks;

internal static class CorrectionExperiment
{
    public static async Task<int> RunAsync(string[] args, JsonSerializerOptions options)
    {
        if (args.Length is not (7 or 8)) throw new ArgumentException("experiment <cases.jsonl> <local.gguf> <NEW-directory> <variant> <max-output-tokens> <provenance.json> [--grammar]");
        if (!Enum.TryParse<CorrectionExperimentVariant>(args[4], out var variant) || !Enum.IsDefined(variant)) throw new ArgumentException("Unknown experiment variant.");
        var constrain = args.Length == 8 && args[7] == "--grammar";
        if (args.Length == 8 && !constrain || constrain && variant != CorrectionExperimentVariant.StructuredOffsets) throw new ArgumentException("Grammar is supported only for StructuredOffsets.");
        var maxTokens = int.Parse(args[5], System.Globalization.CultureInfo.InvariantCulture);
        if (maxTokens is < 1 or > 4096) throw new ArgumentException("Experiment token cap must be between 1 and 4096.");
        var cases = DatasetCommands.ReadCases(args[1], options);
        if (cases.Length == 0) throw new ArgumentException("Experiment cases are empty.");
        var audit = DatasetAudit.Analyze(cases);
        if (audit.AnnotationIssues.Count != 0 || audit.DuplicateRecords != 0) throw new ArgumentException("Experiments require unique valid development cases.");
        using var provenance = JsonDocument.Parse(await File.ReadAllTextAsync(args[6]));
        foreach (var key in new[] { "modelRepository", "modelRevision", "modelSha256", "quantization", "reviewReference" })
            if (!provenance.RootElement.TryGetProperty(key, out var value) || value.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(value.GetString()))
                throw new ArgumentException("Experiment requires explicit pinned model provenance and review reference.");
        var modelHash = HashFile(args[2]);
        if (modelHash != provenance.RootElement.GetProperty("modelSha256").GetString()) throw new InvalidDataException("Local model hash does not match the reviewed provenance file.");
        if (Directory.Exists(args[3]) || File.Exists(args[3])) throw new IOException("Experiment directory must be new.");
        Directory.CreateDirectory(args[3]);
        var predictions = new List<BenchmarkPrediction>();
        var dispositions = new Dictionary<string, CorrectionDisposition>(StringComparer.Ordinal);
        var latencies = new List<double>();
        var tokens = new List<int>();
        var started = DateTimeOffset.UtcNow;
        var complete = false;
        var commit = Environment.GetEnvironmentVariable("DIORTHOSIS_EXPERIMENT_COMMIT") ?? "unknown";
        var native = Path.Combine(AppContext.BaseDirectory, "runtimes", RuntimeInformation.RuntimeIdentifier, "native");
        var nativeHashes = Directory.Exists(native) ? Directory.GetFiles(native).Where(path => path.EndsWith(".dylib", StringComparison.Ordinal) || path.EndsWith(".dll", StringComparison.Ordinal) || path.EndsWith(".so", StringComparison.Ordinal))
            .Order(StringComparer.Ordinal).ToDictionary(path => Path.GetFileName(path)!, HashFile) : new Dictionary<string, string>();
        object Manifest(string state) => new { experimentId = Path.GetFileName(Path.GetFullPath(args[3])), purpose = "controlled development prompt/format diagnostic", repositoryCommit = commit,
            datasetId = Path.GetFileName(args[1]), datasetSha256 = HashFile(args[1]), datasetReview = "unreviewed/development-exposed", evaluatorVersion = ReferenceEvaluator.Version,
            promptVersion = ExperimentalCorrectionPrompt.Version(variant), promptHashes = cases.ToDictionary(item => item.Id, item => DatasetAudit.Hash(ExperimentalCorrectionPrompt.Build(new(item.Language, item.InputText), variant))),
            model = provenance.RootElement.Clone(), modelSha256 = modelHash, modelBytes = new FileInfo(args[2]).Length,
            inferenceAdapter = LlamaSharpInferenceAdapter.RuntimeVersion, llamaCppRevision = "3f7c29d318e317b63f54c558bc69803963d7d88c", nativeHashes,
            operatingSystem = RuntimeInformation.OSDescription, architecture = RuntimeInformation.ProcessArchitecture.ToString(), framework = RuntimeInformation.FrameworkDescription,
            cpuModel = Environment.GetEnvironmentVariable("DIORTHOSIS_EXPERIMENT_CPU") ?? "unknown", physicalRamBytes = Environment.GetEnvironmentVariable("DIORTHOSIS_EXPERIMENT_RAM_BYTES") ?? "unknown",
            inference = new { contextSize = 4096, threads = 9, gpuLayers = 0, temperature = 0, decoding = "greedy", seed = (int?)null, seedPolicy = "no random sampler", maxTokens, constrain,
                grammarVersion = constrain ? ExperimentalCorrectionPrompt.OffsetGrammarVersion : null, grammarSha256 = constrain ? DatasetAudit.Hash(ExperimentalCorrectionPrompt.OffsetGrammar) : null, timeoutSecondsPerCase = 90, completionPolicy = "natural EOG required" },
            startedUtc = started, updatedUtc = DateTimeOffset.UtcNow, completionState = state, completedCases = predictions.Count };
        var manifestPath = Path.Combine(args[3], "manifest.json");
        await File.WriteAllTextAsync(manifestPath, JsonSerializer.Serialize(Manifest("running"), options));
        try
        {
            var load = Stopwatch.StartNew();
            using var adapter = new LlamaSharpInferenceAdapter(args[2], threads: 9);
            load.Stop();
            using var process = Process.GetCurrentProcess();
            long peak = process.WorkingSet64;
            await using var memoryTimer = new Timer(_ => { process.Refresh(); InterlockedExtensions.Max(ref peak, process.WorkingSet64); }, null, 0, 100);
            var capturing = new ExperimentAdapter(adapter);
            var engine = new ExperimentalCorrectionEngine(capturing, variant, maxTokens, constrain);
            using var traceWriter = new StreamWriter(Path.Combine(args[3], "trace.jsonl"), append: false);
            foreach (var item in cases)
            {
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(90));
                var watch = Stopwatch.StartNew();
                CorrectionResult result;
                try { result = await engine.CorrectAsync(new(item.Language, item.InputText), timeout.Token); }
                catch (OperationCanceledException) { throw new InvalidDataException("Experiment case exceeded timeout; partial run retained."); }
                watch.Stop();
                predictions.Add(new(item.Id, result.Edits));
                dispositions[item.Id] = result.Disposition;
                latencies.Add(watch.Elapsed.TotalMilliseconds);
                tokens.Add(capturing.Last!.SampledTokens);
                var jsonValid = false;
                try { using var json = JsonDocument.Parse(result.Analysis!.RawOutput); jsonValid = true; } catch (JsonException) { }
                var row = new { caseId = item.Id, language = item.Language, input = item.InputText, promptSha256 = DatasetAudit.Hash(ExperimentalCorrectionPrompt.Build(new(item.Language, item.InputText), variant)),
                    rawModelOutput = result.Analysis!.RawOutput, terminationReason = result.Termination, sampledTokens = tokens[^1], latencyMilliseconds = latencies[^1], jsonValid,
                    disposition = result.Disposition, rejectionStage = result.Analysis.RejectionStage, rejectionReason = result.Analysis.RejectionReason,
                    rawCandidateTextEdits = result.Analysis.RawCandidateEdits, validatedTextEdits = result.Edits };
                await traceWriter.WriteLineAsync(JsonSerializer.Serialize(row, options));
                await traceWriter.FlushAsync();
                Console.WriteLine($"{item.Id}: {result.Disposition}; {tokens[^1]} tokens; {watch.ElapsedMilliseconds} ms");
            }
            await File.WriteAllLinesAsync(Path.Combine(args[3], "predictions.jsonl"), predictions.Select(prediction => JsonSerializer.Serialize(prediction, options)));
            await File.WriteAllTextAsync(Path.Combine(args[3], "metrics.json"), JsonSerializer.Serialize(ReferenceEvaluator.Evaluate(cases, predictions, dispositions), options));
            await File.WriteAllTextAsync(Path.Combine(args[3], "resources.json"), JsonSerializer.Serialize(new { modelLoadMilliseconds = load.Elapsed.TotalMilliseconds,
                loadPolicy = "OS file-cache uncontrolled; not cold boot", meanLatencyMilliseconds = latencies.Average(), maximumLatencyMilliseconds = latencies.Max(),
                totalSampledTokens = tokens.Sum(), sampledPeakProcessWorkingSetBytes = peak, memorySamplingMilliseconds = 100, memoryLimitations = "sampling may miss brief peaks; not system-wide pressure", completedCases = predictions.Count }, options));
            complete = true;
            return 0;
        }
        finally { await File.WriteAllTextAsync(manifestPath, JsonSerializer.Serialize(Manifest(complete ? "complete" : "failed"), options)); }
    }
    private static string HashFile(string path) { using var stream = File.OpenRead(path); return Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant(); }
    private sealed class ExperimentAdapter(IModelInferenceAdapter adapter) : IModelInferenceAdapter
    {
        public InferenceResult? Last { get; private set; }
        public async ValueTask<InferenceResult> GenerateAsync(InferenceRequest request, CancellationToken cancellationToken = default) => Last = await adapter.GenerateAsync(request, cancellationToken);
    }
    private static class InterlockedExtensions
    {
        public static void Max(ref long location, long value) { var current = Volatile.Read(ref location); while (value > current) { var observed = Interlocked.CompareExchange(ref location, value, current); if (observed == current) break; current = observed; } }
    }
}
