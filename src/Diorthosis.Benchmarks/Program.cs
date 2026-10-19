using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Diorthosis.Benchmarks;
using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
if (args.Length < 1 || (args[0] != "evaluate" && args[0] != "run" && args[0] != "compare"))
{
    Console.Error.WriteLine("Usage: diorthosis-benchmarks evaluate <cases.jsonl> <predictions.jsonl> | run <cases.jsonl> <model.gguf> <predictions.jsonl> <report.json> | compare <comparison.json> <report1.json> <report2.json> [reportN.json ...]");
    return 2;
}

try
{
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
        using var adapter = new LlamaSharpInferenceAdapter(args[2]);
        loadTimer.Stop();
        var maximumOutputTokens = !runOptions.TryGetValue("max-output-tokens", out var maxTokensText)
            ? 512
            : int.TryParse(maxTokensText, out var parsedMaxTokens) && parsedMaxTokens > 0 ? parsedMaxTokens
            : throw new ArgumentException("--max-output-tokens must be a positive integer.");
        var engine = new ModelCorrectionEngine(adapter, maximumOutputTokens);
        var predictions = new List<BenchmarkPrediction>(cases.Length);
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
        }
        await File.WriteAllLinesAsync(args[3], predictions.Select(item => JsonSerializer.Serialize(item, options)));
        memorySampler.Dispose();
        SampleWorkingSet();
        var metrics = BenchmarkEvaluator.Evaluate(cases, predictions);
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
            semanticAlterations = "Require human review; not inferred by this runner",
            decoding = new { temperature = 0.0, maximumOutputTokens = engine.MaximumOutputTokens },
            caseCount = cases.Length,
            metrics
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
                    metrics = report.RootElement.GetProperty("metrics")
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
        if (arguments[i] is not ("--model-revision" or "--quantization" or "--max-output-tokens"))
            throw new ArgumentException("Supported run options are --model-revision, --quantization, and --max-output-tokens.");
        if (!result.TryAdd(arguments[i][2..], arguments[i + 1])) throw new ArgumentException("Duplicate benchmark run option.");
    }
    return result;
}
