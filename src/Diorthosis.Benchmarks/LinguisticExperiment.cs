using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text.Json;
using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;
using Diorthosis.Experiments;

namespace Diorthosis.Benchmarks;

internal static class LinguisticExperiment
{
    public static async Task<int> ReplayAsync(string[] args, JsonSerializerOptions options)
    {
        if (args.Length != 4) throw new ArgumentException("linguistic-replay <panel.jsonl> <completed-run-directory> <NEW-report.json>");
        BenchmarkArtifactPolicy.RequireNewOutputs(args[3]);
        using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(args[2], "manifest.json")));
        if (manifest.RootElement.GetProperty("state").GetString() != "complete" || manifest.RootElement.GetProperty("datasetSha256").GetString() != Hash(args[1]))
            throw new InvalidDataException("Replay requires a complete dataset-bound run.");
        var cases = DatasetCommands.ReadCases(args[1], options).ToDictionary(c => c.Id, StringComparer.Ordinal);
        var sourceRows = File.ReadLines(args[1]).Select(line => JsonSerializer.Deserialize<JsonElement>(line, options)).ToDictionary(r => r.GetProperty("id").GetString()!, r => r);
        var rows = File.ReadLines(Path.Combine(args[2], "trace.jsonl")).Select(line => JsonSerializer.Deserialize<JsonElement>(line, options)).ToArray();
        var seen = new HashSet<(string, string)>();
        var mismatches = new List<string>();
        foreach (var group in rows.GroupBy(r => r.GetProperty("id").GetString()!, StringComparer.Ordinal))
        {
            var item = cases[group.Key];
            var spans = sourceRows[item.Id].TryGetProperty("protectedSpans", out var value) ? JsonSerializer.Deserialize<ProtectedSpan[]>(value, options) ?? [] : [];
            var request = new CorrectionRequest(item.Language, item.InputText, spans);
            var candidates = new List<VotingCandidate>();
            foreach (var row in group.Where(r => r.TryGetProperty("inference", out _)))
            {
                var arm = row.GetProperty("arm").GetString()!;
                if (!seen.Add((item.Id, arm)) || row.GetProperty("input").GetString() != request.Text) throw new InvalidDataException("Duplicate or source-mismatched trace.");
                var inference = JsonSerializer.Deserialize<InferenceResult>(row.GetProperty("inference"), options)!;
                var analysis = ModelCorrectionEngine.AnalyzeInference(inference, request);
                if (JsonSerializer.Serialize(analysis, options) != row.GetProperty("analysis").GetRawText()) mismatches.Add(item.Id + "/" + arm + "/analysis");
                var prompt = arm == "hints" ? SpellingDiagnostics.HintPrompt(request, JsonSerializer.Deserialize<WordDiagnostic[]>(row.GetProperty("dictionaryDiagnostics"), options)!) : CorrectionPrompt.Build(request);
                if (prompt != row.GetProperty("prompt").GetString() || DatasetAudit.Hash(prompt) != row.GetProperty("promptSha256").GetString()) mismatches.Add(item.Id + "/" + arm + "/prompt");
                if (arm.StartsWith("sample-", StringComparison.Ordinal)) candidates.Add(new(arm, inference));
            }
            var voted = EditVoting.Aggregate(request, candidates);
            var savedVote = group.Single(r => r.GetProperty("arm").GetString() == "voting-3");
            if (JsonSerializer.Serialize(voted.Analysis, options) != savedVote.GetProperty("analysis").GetRawText()) mismatches.Add(item.Id + "/voting-3/analysis");
        }
        if (seen.Count != cases.Count * 5 || rows.Length != cases.Count * 6) throw new InvalidDataException("Incomplete replay coverage.");
        await BenchmarkArtifactPolicy.WriteTextNewAsync(args[3], JsonSerializer.Serialize(new { version = "phase3-replay-1.0.0", cases = cases.Count,
            generatedCandidates = seen.Count, votedPassages = cases.Count, traceSha256 = Hash(Path.Combine(args[2], "trace.jsonl")), datasetSha256 = Hash(args[1]),
            mismatches, interpretation = "Deterministic replay of retained outputs and literal prompts, not fresh model inference or human linguistic validation." }, options));
        return mismatches.Count == 0 ? 0 : 1;
    }

    public static async Task<int> RunAsync(string[] args, JsonSerializerOptions options)
    {
        if (args.Length != 7) throw new ArgumentException("linguistic-experiment <panel.jsonl> <local.gguf> <asset-directory> <assets.json> <model-provenance.json> <NEW-output-directory>");
        var cases = DatasetCommands.ReadCases(args[1], options);
        var audit = DatasetAudit.Analyze(cases);
        if (cases.Length == 0 || audit.AnnotationIssues.Count != 0 || audit.DuplicateRecords != 0) throw new InvalidDataException("Expected unique valid development cases.");
        using var assets = JsonDocument.Parse(File.ReadAllText(args[4]));
        foreach (var entry in assets.RootElement.GetProperty("files").EnumerateArray())
            if (Hash(Path.Combine(args[3], entry.GetProperty("path").GetString()!)) != entry.GetProperty("sha256").GetString())
                throw new InvalidDataException("Dictionary/package hash mismatch.");
        using var provenance = JsonDocument.Parse(File.ReadAllText(args[5]));
        foreach (var key in new[] { "modelRepository", "modelRevision", "modelSha256", "quantization", "reviewReference" })
            if (string.IsNullOrWhiteSpace(provenance.RootElement.GetProperty(key).GetString())) throw new InvalidDataException("Missing reviewed model provenance.");
        if (Hash(args[2]) != provenance.RootElement.GetProperty("modelSha256").GetString()) throw new InvalidDataException("Model hash mismatch.");
        if (Directory.Exists(args[6]) || File.Exists(args[6])) throw new IOException("Output directory must be new.");
        Directory.CreateDirectory(args[6]);
        var originalRows = File.ReadLines(args[1]).Select(line => JsonSerializer.Deserialize<JsonElement>(line, options)).ToDictionary(row => row.GetProperty("id").GetString()!, row => row);
        using var process = Process.GetCurrentProcess();
        long peak = process.WorkingSet64;
        await using var memory = new Timer(_ => { process.Refresh(); var value = process.WorkingSet64; long old;
            do { old = Volatile.Read(ref peak); if (value <= old) break; } while (Interlocked.CompareExchange(ref peak, value, old) != old); }, null, 0, 100);
        var started = DateTimeOffset.UtcNow;
        var complete = false;
        var traceCount = 0;
        var predictions = new Dictionary<string, List<BenchmarkPrediction>>();
        var dispositions = new Dictionary<string, Dictionary<string, CorrectionDisposition>>();
        var resources = new List<object>();
        var native = Path.Combine(AppContext.BaseDirectory, "runtimes", RuntimeInformation.RuntimeIdentifier, "native");
        var nativeHashes = Directory.GetFiles(native).Where(p => p.EndsWith(".dylib", StringComparison.Ordinal) || p.EndsWith(".dll", StringComparison.Ordinal) || p.EndsWith(".so", StringComparison.Ordinal))
            .Order(StringComparer.Ordinal).ToDictionary(p => Path.GetFileName(p)!, Hash);
        // Freeze software identity at start, even if a developer rebuilds files during a long run.
        var assemblyHash = Hash(typeof(LinguisticExperiment).Assembly.Location);
        var coreHash = Hash(typeof(ModelCorrectionEngine).Assembly.Location);
        var experimentalHash = Hash(typeof(EditVoting).Assembly.Location);
        object Manifest() => new { version = "phase3-linguistic-experiment-1.0.0", state = complete ? "complete" : "incomplete",
            startedUtc = started, updatedUtc = DateTimeOffset.UtcNow, repositoryCommit = Environment.GetEnvironmentVariable("DIORTHOSIS_EXPERIMENT_COMMIT") ?? "unknown",
            datasetSha256 = Hash(args[1]), assetManifestSha256 = Hash(args[4]), provenance = provenance.RootElement.Clone(),
            assemblySha256 = assemblyHash, coreAssemblySha256 = coreHash,
            experimentalAssemblySha256 = experimentalHash, evaluator = ReferenceEvaluator.Version,
            modelBytes = new FileInfo(args[2]).Length, runtime = LlamaSharpInferenceAdapter.RuntimeVersion,
            llamaCppRevision = "3f7c29d318e317b63f54c558bc69803963d7d88c", nativeHashes,
            os = RuntimeInformation.OSDescription, architecture = RuntimeInformation.ProcessArchitecture.ToString(), framework = RuntimeInformation.FrameworkDescription,
            cpu = Environment.GetEnvironmentVariable("DIORTHOSIS_EXPERIMENT_CPU") ?? "unknown", ramBytes = Environment.GetEnvironmentVariable("DIORTHOSIS_EXPERIMENT_RAM_BYTES") ?? "unknown",
            settings = new { context = 4096, threads = 9, gpuLayers = 0, maximumOutputTokens = 256, timeoutSeconds = 90,
                seeds = new[] { 101, 202, 303 }, temperature = 1, topP = 1, topK = 0, minP = 0, typicalP = 1, repeatPenalty = 1,
                singleStochasticControl = "sample-101 reused from voting pool; not additional independent generation", englishDialect = "union US and GB-ise", completion = "natural EOG only" },
            review = "development-exposed; AI diagnostics only; no human approval", traceRows = traceCount };
        void Save(string name, object data) => File.WriteAllText(Path.Combine(args[6], name), JsonSerializer.Serialize(data, options) + "\n");
        Save("manifest.json", Manifest());
        try
        {
            var init = Stopwatch.StartNew();
            var initialRam = process.WorkingSet64;
            using var dictionary = new HunspellDiagnostics(Path.Combine(args[3], "nl/nl.dic"), Path.Combine(args[3], "en_US/en_US.dic"), Path.Combine(args[3], "en_GB-ise/en_GB-ise.dic"));
            init.Stop(); process.Refresh();
            Save("dictionary-resources.json", new { initializationMilliseconds = init.Elapsed.TotalMilliseconds, workingSetBefore = initialRam, workingSetAfter = process.WorkingSet64,
                limitation = "snapshot delta includes runtime activity; not isolated dictionary heap size", assets = assets.RootElement.Clone() });
            var probeWords = new Dictionary<CorrectionLanguage, string[]>
            {
                [CorrectionLanguage.Dutch] = ["start", "startt", "onmiddelijk", "onmiddellijk", "rapport", "raport", "langetermijnplanning", "lange", "termijnplanning", "opgebeld", "belt", "op", "word", "wordt", "quarantaine", "coördinator", "reünie", "goesting", "Diorthosis", "retry_count"],
                [CorrectionLanguage.English] = ["recieve", "receive", "seperate", "separate", "colour", "color", "organise", "organize", "can't", "can’t", "sesquipedalian", "Diorthosis", "OAuth2", "is", "are"]
            };
            Save("dictionary-probes.json", probeWords.SelectMany(p => p.Value.Select(word => new { language = p.Key, word, recognized = dictionary.Check(p.Key, word), suggestions = dictionary.Suggest(p.Key, word) })));
            var startRequest = new CorrectionRequest(CorrectionLanguage.Dutch, "De vergadering start om negen uur.");
            Save("known-regression.json", SpellingDiagnostics.InspectReplacements(dictionary, startRequest, [new(15, 5, "start", "startt")]));
            var load = Stopwatch.StartNew();
            using var adapter = new LlamaSharpInferenceAdapter(args[2], threads: 9);
            load.Stop();
            using var writer = new StreamWriter(Path.Combine(args[6], "trace.jsonl"), append: false);
            foreach (var item in cases)
            {
                var spans = originalRows[item.Id].TryGetProperty("protectedSpans", out var protectedValue)
                    ? JsonSerializer.Deserialize<ProtectedSpan[]>(protectedValue, options) ?? [] : [];
                var request = new CorrectionRequest(item.Language, item.InputText, spans);
                var diagnosticWatch = Stopwatch.StartNew();
                var diagnostics = SpellingDiagnostics.Inspect(dictionary, item.Language, item.InputText);
                diagnosticWatch.Stop();
                var generated = new List<VotingCandidate>();
                var generationTimes = new List<double>();
                var generationTokens = new List<int>();
                void Prediction(string arm, ModelResponseAnalysis analysis)
                {
                    if (!predictions.TryGetValue(arm, out var list)) { predictions[arm] = list = []; dispositions[arm] = new(StringComparer.Ordinal); }
                    list.Add(new(item.Id, analysis.Edits)); dispositions[arm][item.Id] = analysis.Disposition;
                }
                async Task Trace(object row) { traceCount++; await writer.WriteLineAsync(JsonSerializer.Serialize(row, options)); await writer.FlushAsync(); }
                foreach (var arm in new[] { "baseline", "hints", "sample-101", "sample-202", "sample-303" })
                {
                    var prompt = arm == "hints" ? SpellingDiagnostics.HintPrompt(request, diagnostics) : CorrectionPrompt.Build(request);
                    var sample = arm.StartsWith("sample-", StringComparison.Ordinal) ? new ExperimentalSampling(uint.Parse(arm[7..], System.Globalization.CultureInfo.InvariantCulture)) : null;
                    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(90));
                    process.Refresh(); var cpuBefore = process.TotalProcessorTime;
                    var watch = Stopwatch.StartNew();
                    var inference = await adapter.GenerateAsync(new(prompt, 256, sampling: sample), timeout.Token);
                    watch.Stop(); process.Refresh();
                    var analysis = ModelCorrectionEngine.AnalyzeInference(inference, request);
                    Prediction(arm, analysis);
                    var post = analysis.ExtractedCorrection is null ? [] : SpellingDiagnostics.InspectReplacements(dictionary, request,
                        ModelCorrectionEngine.DeriveDiagnosticEdits(request.Text, analysis.ExtractedCorrection));
                    await Trace(new { item.Id, item.Language, arm, input = item.InputText, prompt, promptSha256 = DatasetAudit.Hash(prompt),
                        promptVersion = arm == "hints" ? SpellingDiagnostics.HintPromptVersion : CorrectionPrompt.Version, sampling = sample,
                        inference, analysis, operations = analysis.ExtractedCorrection is null ? null : LocalEdits.Derive(request.Text, analysis.ExtractedCorrection),
                        dictionaryDiagnostics = diagnostics, postDiagnostics = post, latencyMilliseconds = watch.Elapsed.TotalMilliseconds,
                        cpuMilliseconds = (process.TotalProcessorTime - cpuBefore).TotalMilliseconds, dictionaryDiagnosticMilliseconds = diagnosticWatch.Elapsed.TotalMilliseconds });
                    resources.Add(new { item.Id, arm, latencyMilliseconds = watch.Elapsed.TotalMilliseconds, inference.SampledTokens });
                    Console.WriteLine($"{item.Id} {arm}: {analysis.Disposition}; {inference.SampledTokens} tokens; {watch.ElapsedMilliseconds} ms");
                    if (sample is not null) { generated.Add(new(arm, inference)); generationTimes.Add(watch.Elapsed.TotalMilliseconds); generationTokens.Add(inference.SampledTokens); }
                }
                var voteWatch = Stopwatch.StartNew();
                var vote = EditVoting.Aggregate(request, generated);
                voteWatch.Stop();
                Prediction("voting-3", vote.Analysis);
                await Trace(new { item.Id, item.Language, arm = "voting-3", input = item.InputText, vote, analysis = vote.Analysis,
                    postDiagnostics = SpellingDiagnostics.InspectReplacements(dictionary, request, vote.Analysis.Edits),
                    latencyMilliseconds = generationTimes.Sum() + voteWatch.Elapsed.TotalMilliseconds, sampledTokens = generationTokens.Sum() });
            }
            foreach (var (arm, rows) in predictions)
            {
                File.WriteAllLines(Path.Combine(args[6], arm + "-predictions.jsonl"), rows.Select(row => JsonSerializer.Serialize(row, options)));
                Save(arm + "-metrics.json", ReferenceEvaluator.Evaluate(cases, rows, dispositions[arm]));
            }
            Save("resources.json", new { modelLoadMilliseconds = load.Elapsed.TotalMilliseconds, sampledPeakProcessWorkingSetBytes = peak,
                memorySamplingMilliseconds = 100, limitations = "warm file cache uncontrolled; sampled process working set may miss peaks; not minimum device RAM", generations = resources });
            complete = true;
            return 0;
        }
        finally { Save("manifest.json", Manifest()); }
    }
    private static string Hash(string path) { using var file = File.OpenRead(path); return Convert.ToHexString(SHA256.HashData(file)).ToLowerInvariant(); }
}
