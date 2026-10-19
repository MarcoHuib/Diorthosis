using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Diorthosis.Benchmarks;
using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
if (args.Length < 1 || (args[0] != "evaluate" && args[0] != "run" && args[0] != "compare" && args[0] != "reparse"))
{
    Console.Error.WriteLine("Usage: diorthosis-benchmarks evaluate <cases.jsonl> <predictions.jsonl> | run <cases.jsonl> <model.gguf> <predictions.jsonl> <report.json> [--trace <trace.jsonl>] | reparse <cases.jsonl> <raw-trace.jsonl> <run-report.json> <predictions.jsonl> <trace.jsonl> <report.json> | compare <comparison.json> <report1.json> <report2.json> [reportN.json ...]");
    return 2;
}

try
{
    if (args[0] == "reparse")
    {
        if (args.Length != 7) throw new ArgumentException("Reparse requires cases, captured raw trace, original run report, and three output paths.");
        var cases = ReadCases(args[1], options);
        var caseMap = cases.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var predictions = new List<BenchmarkPrediction>(cases.Length);
        var rawPredictions = new List<BenchmarkPrediction>(cases.Length);
        var traces = new List<object>(cases.Length);
        var rejections = new Dictionary<string, int>(StringComparer.Ordinal);
        var rejectedButRawCorrectErrors = 0;
        foreach (var line in File.ReadLines(args[2]).Where(line => !string.IsNullOrWhiteSpace(line)))
        {
            using var traceDocument = JsonDocument.Parse(line);
            var trace = traceDocument.RootElement;
            var caseId = trace.GetProperty("caseId").GetString()!;
            if (!caseMap.TryGetValue(caseId, out var item)) throw new InvalidDataException("Captured trace contains an unknown benchmark case ID.");
            if (trace.GetProperty("input").GetString() != item.InputText) throw new InvalidDataException("Captured trace input does not match the benchmark dataset.");
            var raw = trace.GetProperty("rawModelOutput").GetString() ?? string.Empty;
            var analysis = ModelCorrectionEngine.AnalyzeModelResponse(raw, item.InputText);
            predictions.Add(new BenchmarkPrediction(caseId, analysis.Edits));
            var rawPrediction = new BenchmarkPrediction(caseId, analysis.RawCandidateEdits);
            rawPredictions.Add(rawPrediction);
            var rawCaseMetrics = BenchmarkEvaluator.Evaluate([item], [rawPrediction]).Where(metric => metric.Category == "all").ToArray();
            var acceptedCaseMetrics = BenchmarkEvaluator.Evaluate([item], [new BenchmarkPrediction(caseId, analysis.Edits)])
                .Where(metric => metric.Category == "all").ToArray();
            var rawCorrectErrors = rawCaseMetrics[0].CorrectlyResolvedErrors;
            if (analysis.RejectionStage is not null && rawCorrectErrors > 0) rejectedButRawCorrectErrors += rawCorrectErrors;
            if (analysis.RejectionStage is not null)
            {
                var reason = $"{analysis.RejectionStage}: {analysis.RejectionReason}";
                rejections[reason] = rejections.GetValueOrDefault(reason) + 1;
            }
            traces.Add(new
            {
                caseId,
                input = item.InputText,
                rawModelOutput = raw,
                extractedCorrection = analysis.ExtractedCorrection,
                rawCandidateTextEdits = analysis.RawCandidateEdits,
                validatedTextEdits = analysis.Edits,
                rejectionStage = analysis.RejectionStage,
                rejectionReason = analysis.RejectionReason,
                rejectedButRawCorrectErrors = analysis.RejectionStage is not null ? rawCorrectErrors : 0,
                rawCandidateBenchmark = rawCaseMetrics,
                acceptedBenchmark = acceptedCaseMetrics
            });
        }
        if (predictions.Count != cases.Length || predictions.Select(item => item.CaseId).Distinct(StringComparer.Ordinal).Count() != cases.Length)
            throw new InvalidDataException("Captured trace must contain exactly one response for each benchmark case.");
        await File.WriteAllLinesAsync(args[4], predictions.Select(item => JsonSerializer.Serialize(item, options)));
        await File.WriteAllLinesAsync(args[5], traces.Select(item => JsonSerializer.Serialize(item, options)));
        var metrics = BenchmarkEvaluator.Evaluate(cases, predictions);
        var rawMetrics = BenchmarkEvaluator.Evaluate(cases, rawPredictions);
        var report = System.Text.Json.Nodes.JsonNode.Parse(await File.ReadAllTextAsync(args[3]))
            ?? throw new InvalidDataException("Original run report is empty.");
        report["metrics"] = JsonSerializer.SerializeToNode(metrics, options);
        report["rawCandidateMetrics"] = JsonSerializer.SerializeToNode(rawMetrics, options);
        report["rawCandidateMetrics"] = JsonSerializer.SerializeToNode(rawMetrics, options);
        report["rejectedModelOutputs"] = rejections.Values.Sum();
        report["rejectedModelOutputsByReason"] = JsonSerializer.SerializeToNode(rejections, options);
        report["rejectedButRawCorrectErrors"] = rejectedButRawCorrectErrors;
        report["postProcessingRevision"] = "current deterministic parser reapplied to captured raw generation; inference timings/memory are from the referenced run report";
        await File.WriteAllTextAsync(args[6], report.ToJsonString(options));
        Console.WriteLine(JsonSerializer.Serialize(metrics.Where(metric => metric.Category is "all" or "CorrectText"), options));
        return 0;
    }

    if (args[0] == "run")
    {
        if (args.Length < 5) throw new ArgumentException("Run requires a cases file, model, predictions output, and report output.");
        var runOptions = ParseOptions([.. args.Skip(5)]);
        var cases = ReadCases(args[1], options);
        using var process = Process.GetCurrentProcess();
        long sampledPeakWorkingSet = 0;
        var languageIndexes = Enum.GetValues<Diorthosis.Core.Corrections.CorrectionLanguage>()
            .Select((language, index) => (language, index)).ToDictionary(item => item.language, item => item.index);
        var sampledLanguagePeakWorkingSet = new long[languageIndexes.Count];
        var activeLanguageIndex = -1;
        static void UpdatePeak(ref long peak, long current)
        {
            var observed = Volatile.Read(ref peak);
            while (current > observed)
            {
                var prior = Interlocked.CompareExchange(ref peak, current, observed);
                if (prior == observed) break;
                observed = prior;
            }
        }
        void SampleWorkingSet()
        {
            process.Refresh();
            var current = process.WorkingSet64;
            UpdatePeak(ref sampledPeakWorkingSet, current);
            var languageIndex = Volatile.Read(ref activeLanguageIndex);
            if (languageIndex >= 0) UpdatePeak(ref sampledLanguagePeakWorkingSet[languageIndex], current);
        }
        SampleWorkingSet();
        using var memorySampler = new Timer(_ => SampleWorkingSet(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
        var loadTimer = Stopwatch.StartNew();
        using var nativeAdapter = new LlamaSharpInferenceAdapter(args[2]);
        var adapter = new CapturingAdapter(nativeAdapter);
        loadTimer.Stop();
        var maximumOutputTokens = !runOptions.TryGetValue("max-output-tokens", out var maxTokensText)
            ? 512
            : int.TryParse(maxTokensText, out var parsedMaxTokens) && parsedMaxTokens > 0 ? parsedMaxTokens
            : throw new ArgumentException("--max-output-tokens must be a positive integer.");
        var engine = new ModelCorrectionEngine(adapter, maximumOutputTokens);
        var predictions = new List<BenchmarkPrediction>(cases.Length);
        var rawPredictions = new List<BenchmarkPrediction>(cases.Length);
        var traces = new List<object>(cases.Length);
        var rejectionCounts = new Dictionary<string, int>(StringComparer.Ordinal);
        var rejectedButRawCorrectErrors = 0;
        var latencies = new List<double>(cases.Length);
        foreach (var item in cases)
        {
            Volatile.Write(ref activeLanguageIndex, languageIndexes[item.Language]);
            SampleWorkingSet();
            var timer = Stopwatch.StartNew();
            var result = await engine.CorrectAsync(new CorrectionRequest(item.Language, item.InputText));
            timer.Stop();
            SampleWorkingSet();
            predictions.Add(new BenchmarkPrediction(item.Id, result.Edits));
            latencies.Add(timer.Elapsed.TotalMilliseconds);
            var analysis = ModelCorrectionEngine.AnalyzeModelResponse(adapter.LastOutput ?? string.Empty, item.InputText);
            var rawPrediction = new BenchmarkPrediction(item.Id, analysis.RawCandidateEdits);
            rawPredictions.Add(rawPrediction);
            var rawCaseMetrics = BenchmarkEvaluator.Evaluate([item], [rawPrediction]).Where(metric => metric.Category == "all").ToArray();
            var acceptedCaseMetrics = BenchmarkEvaluator.Evaluate([item], [new BenchmarkPrediction(item.Id, result.Edits)])
                .Where(metric => metric.Category == "all").ToArray();
            var rawCorrectErrors = rawCaseMetrics[0].CorrectlyResolvedErrors;
            if (analysis.RejectionStage is not null && rawCorrectErrors > 0) rejectedButRawCorrectErrors += rawCorrectErrors;
            if (analysis.RejectionStage is not null)
            {
                var reasonKey = $"{analysis.RejectionStage}: {analysis.RejectionReason}";
                rejectionCounts[reasonKey] = rejectionCounts.GetValueOrDefault(reasonKey) + 1;
            }
            traces.Add(new
            {
                caseId = item.Id,
                input = item.InputText,
                rawModelOutput = analysis.RawOutput,
                extractedCorrection = analysis.ExtractedCorrection,
                rawCandidateTextEdits = analysis.RawCandidateEdits,
                validatedTextEdits = result.Edits,
                rejectionStage = analysis.RejectionStage,
                rejectionReason = analysis.RejectionReason,
                rejectedButRawCorrectErrors = analysis.RejectionStage is not null ? rawCorrectErrors : 0,
                rawCandidateBenchmark = rawCaseMetrics,
                acceptedBenchmark = acceptedCaseMetrics
            });
        }
        await File.WriteAllLinesAsync(args[3], predictions.Select(item => JsonSerializer.Serialize(item, options)));
        if (runOptions.TryGetValue("trace", out var tracePath))
            await File.WriteAllLinesAsync(tracePath, traces.Select(item => JsonSerializer.Serialize(item, options)));
        memorySampler.Dispose();
        SampleWorkingSet();
        var metrics = BenchmarkEvaluator.Evaluate(cases, predictions);
        var rawMetrics = BenchmarkEvaluator.Evaluate(cases, rawPredictions);
        var modelHash = await HashFileAsync(args[2]);
        var datasetHash = await HashFileAsync(args[1]);
        var report = new
        {
            modelFile = Path.GetFileName(args[2]),
            modelSha256 = modelHash,
            modelRevision = runOptions.GetValueOrDefault("model-revision", "unspecified-local-file"),
            quantization = runOptions.GetValueOrDefault("quantization", "unspecified; inspect GGUF metadata"),
            datasetSha256 = datasetHash,
            runtime = LlamaSharpInferenceAdapter.RuntimeVersion,
            promptVersion = CorrectionPrompt.Version,
            hardware = new { architecture = System.Runtime.InteropServices.RuntimeInformation.ProcessArchitecture.ToString(), logicalProcessorCount = Environment.ProcessorCount, framework = System.Runtime.InteropServices.RuntimeInformation.FrameworkDescription },
            operatingSystem = System.Runtime.InteropServices.RuntimeInformation.OSDescription,
            inference = new { contextSize = 4096, cpuThreads = Math.Max(1, Environment.ProcessorCount - 1), gpuLayerCount = 0 },
            coldStartMilliseconds = loadTimer.Elapsed.TotalMilliseconds,
            latencyMilliseconds = new { mean = latencies.Count == 0 ? 0 : latencies.Average(), maximum = latencies.Count == 0 ? 0 : latencies.Max() },
            latencyMillisecondsByLanguage = Enum.GetValues<Diorthosis.Core.Corrections.CorrectionLanguage>().ToDictionary(
                language => language.ToString(), language =>
                {
                    var values = cases.Select((item, index) => (item, index)).Where(value => value.item.Language == language)
                        .Select(value => latencies[value.index]).ToArray();
                    return new { caseCount = values.Length, mean = values.Length == 0 ? 0 : values.Average(), maximum = values.Length == 0 ? 0 : values.Max() };
                }),
            peakWorkingSetBytes = sampledPeakWorkingSet,
            peakWorkingSetBytesByLanguage = languageIndexes.ToDictionary(item => item.Key.ToString(), item => sampledLanguagePeakWorkingSet[item.Value]),
            peakMemoryMeasurement = "Maximum sampled process working set, sampled every 100 ms across model load and inference",
            managedHeapBytesAfterRun = GC.GetTotalMemory(forceFullCollection: false),
            rejectedModelOutputs = engine.RejectedOutputCount,
            rejectedModelOutputsByReason = rejectionCounts,
            rejectedButRawCorrectErrors,
            semanticAlterations = "Require human review; not inferred by this runner",
            decoding = new { temperature = 0.0, maximumOutputTokens = engine.MaximumOutputTokens },
            caseCount = cases.Length,
            metrics,
            rawCandidateMetrics = rawMetrics
        };
        await File.WriteAllTextAsync(args[4], JsonSerializer.Serialize(report, options));
        Console.WriteLine(JsonSerializer.Serialize(metrics, options));
        return 0;
    }

    if (args[0] == "compare")
    {
        if (args.Length < 4) throw new ArgumentException("Compare requires an output path and at least two run reports.");
        using var first = JsonDocument.Parse(await File.ReadAllTextAsync(args[2]));
        var datasetHash = first.RootElement.GetProperty("datasetSha256").GetString();
        var promptVersion = first.RootElement.GetProperty("promptVersion").GetString();
        var runtimeVersion = first.RootElement.GetProperty("runtime").GetString();
        var operatingSystem = first.RootElement.GetProperty("operatingSystem").GetString();
        var hardware = first.RootElement.GetProperty("hardware").GetRawText();
        var decoding = first.RootElement.GetProperty("decoding").GetRawText();
        var inference = first.RootElement.GetProperty("inference").GetRawText();
        if (first.RootElement.GetProperty("modelRevision").GetString() is "" or "unspecified-local-file"
            || first.RootElement.GetProperty("quantization").GetString() is null or "unspecified; inspect GGUF metadata")
            throw new ArgumentException("Every compared model report must identify its exact revision and quantization.");
        var reports = new List<JsonDocument> { first };
        try
        {
            foreach (var reportPath in args.Skip(3))
            {
                var report = JsonDocument.Parse(await File.ReadAllTextAsync(reportPath));
                reports.Add(report);
                if (report.RootElement.GetProperty("datasetSha256").GetString() != datasetHash
                    || report.RootElement.GetProperty("promptVersion").GetString() != promptVersion
                    || report.RootElement.GetProperty("runtime").GetString() != runtimeVersion
                    || report.RootElement.GetProperty("operatingSystem").GetString() != operatingSystem
                    || report.RootElement.GetProperty("hardware").GetRawText() != hardware
                    || report.RootElement.GetProperty("decoding").GetRawText() != decoding
                    || report.RootElement.GetProperty("inference").GetRawText() != inference)
                    throw new ArgumentException("Model reports must use the same dataset, runtime, hardware, operating system, prompt version, and inference/decoding settings.");
                if (report.RootElement.GetProperty("modelRevision").GetString() is "" or "unspecified-local-file"
                    || report.RootElement.GetProperty("quantization").GetString() is null or "unspecified; inspect GGUF metadata")
                    throw new ArgumentException("Every compared model report must identify its exact revision and quantization.");
            }
            var comparison = new
            {
                datasetSha256 = datasetHash,
                promptVersion,
                runtime = runtimeVersion,
                inference = first.RootElement.GetProperty("inference"),
                decoding = first.RootElement.GetProperty("decoding"),
                models = reports.Select(report => new
                {
                    modelFile = report.RootElement.GetProperty("modelFile").GetString(),
                    modelSha256 = report.RootElement.GetProperty("modelSha256").GetString(),
                    modelRevision = report.RootElement.GetProperty("modelRevision").GetString(),
                    quantization = report.RootElement.GetProperty("quantization").GetString(),
                    runtime = report.RootElement.GetProperty("runtime").GetString(),
                    operatingSystem = report.RootElement.GetProperty("operatingSystem").GetString(),
                    hardware = report.RootElement.GetProperty("hardware"),
                    coldStartMilliseconds = report.RootElement.GetProperty("coldStartMilliseconds").GetDouble(),
                    latencyMilliseconds = report.RootElement.GetProperty("latencyMilliseconds"),
                    latencyMillisecondsByLanguage = report.RootElement.GetProperty("latencyMillisecondsByLanguage"),
                    peakWorkingSetBytesByLanguage = report.RootElement.GetProperty("peakWorkingSetBytesByLanguage"),
                    rejectedModelOutputs = report.RootElement.GetProperty("rejectedModelOutputs"),
                    rejectedButRawCorrectErrors = report.RootElement.GetProperty("rejectedButRawCorrectErrors"),
                    rejectedModelOutputsByReason = report.RootElement.GetProperty("rejectedModelOutputsByReason"),
                    rawCandidateMetrics = report.RootElement.GetProperty("rawCandidateMetrics"),
                    acceptedCorrectionMetrics = report.RootElement.GetProperty("metrics")
                }).ToArray()
            };
            await File.WriteAllTextAsync(args[1], JsonSerializer.Serialize(comparison, options));
            Console.WriteLine($"Compared {reports.Count} model runs on the same dataset and decoding settings.");
            return 0;
        }
        finally
        {
            foreach (var report in reports) report.Dispose();
        }
    }

    if (args.Length != 3) throw new ArgumentException("Evaluate requires cases and predictions JSONL files.");
    var evalCases = ReadCases(args[1], options);
    var predictionsFromFile = File.ReadLines(args[2]).Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => JsonSerializer.Deserialize<BenchmarkPrediction>(line, options)
            ?? throw new InvalidDataException("Prediction line is null.")).ToArray();
    Console.WriteLine(JsonSerializer.Serialize(BenchmarkEvaluator.Evaluate(evalCases, predictionsFromFile), options));
    return 0;
}
catch (Exception exception) when (exception is IOException or JsonException or ArgumentException or InvalidDataException or DllNotFoundException or BadImageFormatException)
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}

static BenchmarkCase[] ReadCases(string path, JsonSerializerOptions options) =>
[
    .. File.ReadLines(path).Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => JsonSerializer.Deserialize<BenchmarkCase>(line, options)
                        ?? throw new InvalidDataException("Benchmark case line is null."))
];

static async Task<string> HashFileAsync(string path)
{
    await using var stream = File.OpenRead(path);
    return Convert.ToHexString(await SHA256.HashDataAsync(stream)).ToLowerInvariant();
}

static Dictionary<string, string> ParseOptions(string[] arguments)
{
    var result = new Dictionary<string, string>(StringComparer.Ordinal);
    if (arguments.Length % 2 != 0) throw new ArgumentException("Benchmark run options must be name/value pairs.");
    for (var i = 0; i < arguments.Length; i += 2)
    {
        if (arguments[i] is not ("--model-revision" or "--quantization" or "--max-output-tokens" or "--trace"))
            throw new ArgumentException("Supported run options are --model-revision, --quantization, --max-output-tokens, and --trace.");
        if (!result.TryAdd(arguments[i][2..], arguments[i + 1])) throw new ArgumentException("Duplicate benchmark run option.");
    }
    return result;
}

sealed class CapturingAdapter(IModelInferenceAdapter inner) : IModelInferenceAdapter
{
    public string? LastOutput { get; private set; }
    public async ValueTask<string> GenerateAsync(InferenceRequest request, CancellationToken cancellationToken = default)
    {
        LastOutput = await inner.GenerateAsync(request, cancellationToken).ConfigureAwait(false);
        return LastOutput;
    }
}
