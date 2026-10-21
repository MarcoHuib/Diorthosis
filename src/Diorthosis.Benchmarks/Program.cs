using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using System.Text.Json.Serialization;
using Diorthosis.Benchmarks;
using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;

var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
if (args.Length < 1 || (args[0] != "evaluate" && args[0] != "run" && args[0] != "compare" && args[0] != "reparse" && args[0] != "audit-dataset" && args[0] != "check-splits" && args[0] != "evaluate-v3" && args[0] != "experiment" && args[0] != "adjudicate" && args[0] != "linguistic-experiment" && args[0] != "linguistic-replay" && args[0] != "linguistic-impact" && args[0] != "prompt-quality"))
{
    Console.Error.WriteLine("Usage: diorthosis-benchmarks evaluate <cases.jsonl> <predictions.jsonl> | run <cases.jsonl> <model.gguf> <predictions.jsonl> <report.json> [--trace <trace.jsonl>] | reparse <cases.jsonl> <raw-trace.jsonl> <run-report.json> <predictions.jsonl> <trace.jsonl> <report.json> | compare <comparison.json> <report1.json> <report2.json> [reportN.json ...]");
    return 2;
}

try
{
    if (args[0] == "adjudicate")
    {
        if (args.Length is not (5 or 7) || args.Length == 7 && args[5] != "--lineage") throw new ArgumentException("adjudicate <subjects.jsonl> <decisions.jsonl> <adjudications.jsonl> <NEW-report.json> [--lineage <lineage.json>]");
        BenchmarkArtifactPolicy.RequireNewOutputs(args[4]);
        T[] Lines<T>(string path) => File.ReadLines(path).Where(line => !string.IsNullOrWhiteSpace(line))
            .Select(line => JsonSerializer.Deserialize<T>(line, options) ?? throw new InvalidDataException("Null review record.")).ToArray();
        var subjects = Lines<ReviewSubject>(args[1]); var decisions = Lines<ReviewDecision>(args[2]); var finals = Lines<ReviewAdjudication>(args[3]);
        var reviewed = LinguisticReview.Evaluate(subjects, decisions, finals);
        object output = reviewed;
        if (args.Length == 7)
        {
            var lineage = JsonSerializer.Deserialize<Dictionary<string, JsonElement[]>>(await File.ReadAllTextAsync(args[6]), options)
                ?? throw new InvalidDataException("Missing lineage.");
            var known = subjects.Select(item => item.SubjectId).ToHashSet(StringComparer.Ordinal);
            if (lineage.Any(pair => !known.Contains(pair.Key) || pair.Value is null || pair.Value.Length == 0
                || pair.Value.Any(value => !value.TryGetProperty("model", out var model) || model.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(model.GetString()))))
                throw new InvalidDataException("Lineage must reference known bound subjects and explicit model names.");
            var names = lineage.Values.SelectMany(values => values).Select(value => value.GetProperty("model").GetString()!).Distinct(StringComparer.Ordinal);
            var byModel = names.ToDictionary(name => name, name =>
            {
                var ids = lineage.Where(pair => pair.Value.Any(value => value.GetProperty("model").GetString() == name)).Select(pair => pair.Key).ToHashSet(StringComparer.Ordinal);
                return LinguisticReview.Evaluate(subjects.Where(item => ids.Contains(item.SubjectId)).ToArray(), decisions.Where(item => ids.Contains(item.SubjectId)).ToArray(), finals.Where(item => ids.Contains(item.SubjectId)).ToArray());
            }, StringComparer.Ordinal);
            output = new { aggregateUniqueSubjects = reviewed, byModel, declaredLineageSha256 = await HashFileAsync(args[6]),
                lineagePolicy = "Model membership is a declared mapping, not authenticated identity. Source trace/run binding status remains in lineage records; shared judgments are reused per model, never independent observations." };
        }
        await BenchmarkArtifactPolicy.WriteTextNewAsync(args[4], JsonSerializer.Serialize(output, options));
        return 0;
    }
    if (args[0] == "prompt-quality") return await PromptQualityExperiment.RunAsync(args, options);
    if (args[0] == "linguistic-impact") return await LinguisticImpact.RunAsync(args, options);
    if (args[0] == "linguistic-replay") return await LinguisticExperiment.ReplayAsync(args, options);
    if (args[0] == "linguistic-experiment") return await LinguisticExperiment.RunAsync(args, options);
    if (args[0] == "experiment") return await CorrectionExperiment.RunAsync(args, options);
    if (args[0] == "evaluate-v3")
    {
        if (args.Length != 3) throw new ArgumentException("evaluate-v3 <cases.jsonl> <predictions.jsonl>");
        var cases = ReadCases(args[1], options);
        var predictions = File.ReadLines(args[2]).Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => JsonSerializer.Deserialize<BenchmarkPrediction>(line, options) ?? throw new InvalidDataException("Null prediction.")).ToArray();
        Console.WriteLine(JsonSerializer.Serialize(ReferenceEvaluator.Evaluate(cases, predictions), options));
        return 0;
    }
    if (args[0] is "audit-dataset" or "check-splits") return await DatasetCommands.ExecuteAsync(args, options);
    if (args[0] == "reparse")
    {
        if (args.Length != 7) throw new ArgumentException("Reparse requires cases, captured raw trace, original run report, and three output paths.");
        RequireNewOutputs(args[4], args[5], args[6]);
        var cases = ReadCases(args[1], options);
        using var sourceReportDocument = JsonDocument.Parse(await File.ReadAllTextAsync(args[3]));
        var datasetSha256 = await HashFileAsync(args[1]);
        var sourceTraceSha256 = await HashFileAsync(args[2]);
        var sourceReportSha256 = await HashFileAsync(args[3]);
        var binding = BenchmarkArtifactPolicy.ValidateReplay(sourceReportDocument.RootElement, datasetSha256, cases.Length, sourceTraceSha256);
        var caseMap = cases.ToDictionary(item => item.Id, StringComparer.Ordinal);
        var predictions = new List<BenchmarkPrediction>(cases.Length);
        var rawPredictions = new List<BenchmarkPrediction>(cases.Length);
        var traces = new List<object>(cases.Length);
        var dispositions = new Dictionary<string, CorrectionDisposition>(StringComparer.Ordinal);
        var replayComparisons = new List<object>(cases.Length);
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
            var historicalEdits = trace.TryGetProperty("validatedTextEdits", out var historicalEditsValue)
                ? JsonSerializer.Deserialize<IReadOnlyList<TextEdit>>(historicalEditsValue.GetRawText(), options) ?? []
                : [];
            var historicalCorrectErrors = ReadTraceMetric(trace, "acceptedBenchmark", "correctlyResolvedErrors");
            var termination = trace.TryGetProperty("terminationReason", out var terminationElement)
                && Enum.TryParse<GenerationTermination>(terminationElement.GetString(), ignoreCase: true, out var parsedTermination) ? parsedTermination : GenerationTermination.Unknown;
            var analysis = ModelCorrectionEngine.AnalyzeInference(new InferenceResult(raw, termination, 0), new CorrectionRequest(item.Language, item.InputText));
            dispositions[caseId] = analysis.Disposition;
            predictions.Add(new BenchmarkPrediction(caseId, analysis.Edits));
            var rawPrediction = new BenchmarkPrediction(caseId, analysis.RawCandidateEdits);
            rawPredictions.Add(rawPrediction);
            var rawCaseMetrics = BenchmarkEvaluator.EvaluateHistorical([item], [rawPrediction]).Where(metric => metric.Category == "all").ToArray();
            var acceptedCaseMetrics = BenchmarkEvaluator.EvaluateHistorical([item], [new BenchmarkPrediction(caseId, analysis.Edits)])
                .Where(metric => metric.Category == "all").ToArray();
            var currentCorrectErrors = acceptedCaseMetrics[0].CorrectlyResolvedErrors;
            var currentFalsePositives = acceptedCaseMetrics[0].FalsePositiveSuggestions;
            var replayClassification = ClassifyReplayChange(historicalEdits.Count, analysis.Edits.Count,
                analysis.Disposition, analysis.RejectionReason, historicalCorrectErrors, currentCorrectErrors,
                currentFalsePositives);
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
                terminationReason = termination,
                sampledTokenCount = trace.TryGetProperty("sampledTokenCount", out var sampled) ? sampled.Clone() : (JsonElement?)null,
                extractedCorrection = analysis.ExtractedCorrection,
                rawCandidateTextEdits = analysis.RawCandidateEdits,
                validatedTextEdits = analysis.Edits,
                historicalValidatedTextEdits = historicalEdits,
                disposition = analysis.Disposition,
                replayClassification,
                rejectionStage = analysis.RejectionStage,
                rejectionReason = analysis.RejectionReason,
                rejectedButRawCorrectErrors = analysis.RejectionStage is not null ? rawCorrectErrors : 0,
                rawCandidateBenchmark = rawCaseMetrics,
                acceptedBenchmark = acceptedCaseMetrics
            });
            replayComparisons.Add(new
            {
                caseId,
                language = item.Language.ToString(),
                disposition = analysis.Disposition,
                replayClassification,
                historicalEditCount = historicalEdits.Count,
                currentEditCount = analysis.Edits.Count,
                historicalCorrectErrors,
                currentCorrectErrors,
                currentFalsePositiveSuggestions = currentFalsePositives,
                rejectionReason = analysis.RejectionReason
            });
        }
        if (predictions.Count != cases.Length || predictions.Select(item => item.CaseId).Distinct(StringComparer.Ordinal).Count() != cases.Length)
            throw new InvalidDataException("Captured trace must contain exactly one response for each benchmark case.");
        await BenchmarkArtifactPolicy.WriteLinesNewAsync(args[4], predictions.Select(item => JsonSerializer.Serialize(item, options)));
        await BenchmarkArtifactPolicy.WriteLinesNewAsync(args[5], traces.Select(item => JsonSerializer.Serialize(item, options)));
        var metrics = BenchmarkEvaluator.EvaluateHistorical(cases, predictions);
        var qualityMetrics = BenchmarkEvaluator.EvaluateQuality(cases, predictions, dispositions);
        var rawMetrics = BenchmarkEvaluator.EvaluateHistorical(cases, rawPredictions);
        var report = System.Text.Json.Nodes.JsonNode.Parse(sourceReportDocument.RootElement.GetRawText())
            ?? throw new InvalidDataException("Original run report is empty.");
        var originalRunReport = report.DeepClone();
        report["kind"] = "deterministic-replay";
        report["originalRunReport"] = originalRunReport;
        // Inference identities/resources are source claims, never newly measured replay values.
        foreach (var key in new[] { "modelFile", "modelSha256", "modelRevision", "quantization", "promptVersion", "runtime", "operatingSystem", "hardware", "inference", "decoding", "coldStartMilliseconds", "latencyMilliseconds", "latencyMillisecondsByLanguage", "peakWorkingSetBytes", "peakWorkingSetBytesByLanguage", "peakMemoryMeasurement", "managedHeapBytesAfterRun", "generationTerminations" })
            report.AsObject().Remove(key);
        report["datasetSha256"] = datasetSha256;
        report["caseCount"] = cases.Length;
        report["traceSha256"] = await HashFileAsync(args[5]);
        report["replayProvenance"] = JsonSerializer.SerializeToNode(new { binding, sourceReportSha256, sourceTraceSha256,
            evaluatorVersion = BenchmarkEvaluator.QualityEvaluatorVersion, correctionContract = CorrectionResult.ContractVersion,
            sourceRevision = Environment.GetEnvironmentVariable("DIORTHOSIS_EXPERIMENT_COMMIT") ?? "unknown" }, options);
        report["historicalV4Metrics"] = report["metrics"]?.DeepClone();
        report["historicalV4RawCandidateMetrics"] = report["rawCandidateMetrics"]?.DeepClone();
        report["metrics"] = JsonSerializer.SerializeToNode(metrics, options);
        report["qualityMetrics"] = JsonSerializer.SerializeToNode(qualityMetrics, options);
        report["rawCandidateMetrics"] = JsonSerializer.SerializeToNode(rawMetrics, options);
        report["rejectedModelOutputs"] = rejections.Values.Sum();
        report["rejectedModelOutputsByReason"] = JsonSerializer.SerializeToNode(rejections, options);
        report["rejectedButRawCorrectErrors"] = rejectedButRawCorrectErrors;
        report["dispositionsAvailable"] = true;
        report["dispositionCounts"] = JsonSerializer.SerializeToNode(dispositions.Values
            .GroupBy(value => value.ToString()).ToDictionary(group => group.Key, group => group.Count()), options);
        report["replayComparisons"] = JsonSerializer.SerializeToNode(replayComparisons, options);
        report["replaySummary"] = JsonSerializer.SerializeToNode(replayComparisons
            .Select(item => ((JsonElement)JsonSerializer.SerializeToElement(item, options)).GetProperty("replayClassification").GetString()!)
            .GroupBy(value => value).ToDictionary(group => group.Key, group => group.Count()), options);
        report["postProcessingRevision"] = "current deterministic parser reapplied to captured raw generation; inference timings/memory are from the referenced run report";
        await BenchmarkArtifactPolicy.WriteTextNewAsync(args[6], report.ToJsonString(options));
        Console.WriteLine(JsonSerializer.Serialize(metrics.Where(metric => metric.Category is "all" or "CorrectText"), options));
        return 0;
    }

    if (args[0] == "run")
    {
        if (args.Length < 5) throw new ArgumentException("Run requires a cases file, model, predictions output, and report output.");
        var runOptions = ParseOptions([.. args.Skip(5)]);
        RequireNewOutputs([args[3], args[4], .. (runOptions.TryGetValue("trace", out var requestedTrace) ? new[] { requestedTrace } : Array.Empty<string>())]);
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
        var dispositions = new Dictionary<string, CorrectionDisposition>(StringComparer.Ordinal);
        var rejectionCounts = new Dictionary<string, int>(StringComparer.Ordinal);
        var terminationCounts = new Dictionary<string, int>(StringComparer.Ordinal);
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
            terminationCounts[adapter.LastTerminationReason] = terminationCounts.GetValueOrDefault(adapter.LastTerminationReason) + 1;
            predictions.Add(new BenchmarkPrediction(item.Id, result.Edits));
            latencies.Add(timer.Elapsed.TotalMilliseconds);
            var analysis = result.Analysis!;
            dispositions[item.Id] = analysis.Disposition;
            var rawPrediction = new BenchmarkPrediction(item.Id, analysis.RawCandidateEdits);
            rawPredictions.Add(rawPrediction);
            var rawCaseMetrics = BenchmarkEvaluator.EvaluateHistorical([item], [rawPrediction]).Where(metric => metric.Category == "all").ToArray();
            var acceptedCaseMetrics = BenchmarkEvaluator.EvaluateHistorical([item], [new BenchmarkPrediction(item.Id, result.Edits)])
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
                terminationReason = adapter.LastTerminationReason,
                sampledTokenCount = adapter.LastSampledTokenCount,
                latencyMilliseconds = latencies[^1],
                extractedCorrection = analysis.ExtractedCorrection,
                rawCandidateTextEdits = analysis.RawCandidateEdits,
                validatedTextEdits = result.Edits,
                disposition = analysis.Disposition,
                rejectionStage = analysis.RejectionStage,
                rejectionReason = analysis.RejectionReason,
                rejectedButRawCorrectErrors = analysis.RejectionStage is not null ? rawCorrectErrors : 0,
                rawCandidateBenchmark = rawCaseMetrics,
                acceptedBenchmark = acceptedCaseMetrics
            });
        }
        await BenchmarkArtifactPolicy.WriteLinesNewAsync(args[3], predictions.Select(item => JsonSerializer.Serialize(item, options)));
        if (runOptions.TryGetValue("trace", out var tracePath))
            await BenchmarkArtifactPolicy.WriteLinesNewAsync(tracePath, traces.Select(item => JsonSerializer.Serialize(item, options)));
        memorySampler.Dispose();
        SampleWorkingSet();
        var metrics = BenchmarkEvaluator.EvaluateHistorical(cases, predictions);
        var qualityMetrics = BenchmarkEvaluator.EvaluateQuality(cases, predictions, dispositions);
        var rawMetrics = BenchmarkEvaluator.EvaluateHistorical(cases, rawPredictions);
        var modelHash = await HashFileAsync(args[2]);
        var datasetHash = await HashFileAsync(args[1]);
        var report = new
        {
            modelFile = Path.GetFileName(args[2]),
            modelSha256 = modelHash,
            modelRevision = runOptions.GetValueOrDefault("model-revision", "unspecified-local-file"),
            quantization = runOptions.GetValueOrDefault("quantization", "unspecified; inspect GGUF metadata"),
            datasetSha256 = datasetHash,
            traceSha256 = runOptions.TryGetValue("trace", out var emittedTracePath) ? await HashFileAsync(emittedTracePath) : null,
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
            generationTerminations = terminationCounts,
            rejectedModelOutputsByReason = rejectionCounts,
            rejectedButRawCorrectErrors,
            dispositionsAvailable = true,
            dispositionCounts = dispositions.Values.GroupBy(value => value.ToString())
                .ToDictionary(group => group.Key, group => group.Count()),
            semanticAlterations = "Require human review; not inferred by this runner",
            decoding = new { sampler = "GreedySamplingPipeline", temperature = 0.0, maximumOutputTokens = engine.MaximumOutputTokens },
            caseCount = cases.Length,
            metrics,
            qualityMetrics,
            rawCandidateMetrics = rawMetrics
        };
        await BenchmarkArtifactPolicy.WriteTextNewAsync(args[4], JsonSerializer.Serialize(report, options));
        Console.WriteLine(JsonSerializer.Serialize(metrics, options));
        return 0;
    }

    if (args[0] == "compare")
    {
        if (args.Length < 4) throw new ArgumentException("Compare requires an output path and at least two run reports.");
        RequireNewOutputs(args[1]);
        using var first = JsonDocument.Parse(await File.ReadAllTextAsync(args[2]));
        if (first.RootElement.TryGetProperty("kind", out var firstKind) && firstKind.GetString() == "deterministic-replay")
            throw new ArgumentException("Compare original inference reports; replay source metadata is recorded separately.");
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
                if (report.RootElement.TryGetProperty("kind", out var kind) && kind.GetString() == "deterministic-replay")
                    throw new ArgumentException("Compare original inference reports; replay source metadata is recorded separately.");
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
                    peakWorkingSetBytes = report.RootElement.GetProperty("peakWorkingSetBytes").GetInt64(),
                    peakWorkingSetBytesByLanguage = report.RootElement.GetProperty("peakWorkingSetBytesByLanguage"),
                    generationTerminations = report.RootElement.GetProperty("generationTerminations"),
                    rejectedModelOutputs = report.RootElement.GetProperty("rejectedModelOutputs"),
                    rejectedButRawCorrectErrors = report.RootElement.GetProperty("rejectedButRawCorrectErrors"),
                    rejectedModelOutputsByReason = report.RootElement.GetProperty("rejectedModelOutputsByReason"),
                    rawCandidateMetrics = report.RootElement.GetProperty("rawCandidateMetrics"),
                    acceptedCorrectionMetrics = report.RootElement.GetProperty("metrics")
                }).ToArray()
            };
            await BenchmarkArtifactPolicy.WriteTextNewAsync(args[1], JsonSerializer.Serialize(comparison, options));
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
    Console.WriteLine(JsonSerializer.Serialize(BenchmarkEvaluator.EvaluateHistorical(evalCases, predictionsFromFile), options));
    return 0;
}
catch (Exception exception) when (exception is IOException or JsonException or ArgumentException or InvalidDataException or DllNotFoundException or BadImageFormatException)
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}

static void RequireNewOutputs(params string[] paths) => BenchmarkArtifactPolicy.RequireNewOutputs(paths);

static BenchmarkCase[] ReadCases(string path, JsonSerializerOptions options) => DatasetCommands.ReadCases(path, options);

static async Task<string> HashFileAsync(string path)
{
    await using var stream = File.OpenRead(path);
    return Convert.ToHexString(await SHA256.HashDataAsync(stream)).ToLowerInvariant();
}

static int ReadTraceMetric(JsonElement trace, string arrayProperty, string metricProperty)
{
    if (!trace.TryGetProperty(arrayProperty, out var values) || values.ValueKind != JsonValueKind.Array) return 0;
    foreach (var value in values.EnumerateArray())
        if (value.TryGetProperty("category", out var category) && category.GetString() == "all"
            && value.TryGetProperty(metricProperty, out var metric) && metric.TryGetInt32(out var result))
            return result;
    return 0;
}

static string ClassifyReplayChange(int historicalEditCount, int currentEditCount,
    CorrectionDisposition disposition, string? rejectionReason, int historicalCorrectErrors,
    int currentCorrectErrors, int currentFalsePositiveSuggestions)
{
    if (disposition == CorrectionDisposition.Suppressed) return "SuppressedCosmeticOutput";
    if (disposition == CorrectionDisposition.Unchanged) return "UnchangedOutput";
    if (disposition == CorrectionDisposition.NeedsReview) return "NeedsHumanReview";
    if (historicalEditCount > 0 && currentEditCount == 0 && disposition == CorrectionDisposition.Rejected)
    {
        var safetyReason = rejectionReason?.Contains("protected", StringComparison.OrdinalIgnoreCase) == true
            || rejectionReason?.Contains("negation", StringComparison.OrdinalIgnoreCase) == true
            || rejectionReason?.Contains("speech act", StringComparison.OrdinalIgnoreCase) == true
            || rejectionReason?.Contains("omits", StringComparison.OrdinalIgnoreCase) == true
            || rejectionReason?.Contains("symbol", StringComparison.OrdinalIgnoreCase) == true;
        return safetyReason ? "PreviouslyAcceptedUnsafeModificationBlocked"
            : historicalCorrectErrors > 0 ? "PreviouslyAcceptedValidCorrectionBlocked"
            : "PreviouslyAcceptedUncreditedChangeBlocked";
    }
    if (historicalEditCount == 0 && currentEditCount > 0 && currentCorrectErrors > 0)
        return "ValidCorrectionNewlyAccepted";
    if (disposition == CorrectionDisposition.Rejected && currentEditCount == 0)
        return "RejectedUnverifiableOutput";
    if (disposition == CorrectionDisposition.Accepted && currentFalsePositiveSuggestions > 0)
        return "AcceptedChangeNeedsHumanReview";
    if (historicalEditCount != currentEditCount)
        return "AcceptedEditSetChangedNeedsHumanReview";
    return "NoMaterialReplayChange";
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
    public string LastTerminationReason { get; private set; } = "NotReportedByAdapter";
    public int? LastSampledTokenCount { get; private set; }
    public async ValueTask<InferenceResult> GenerateAsync(InferenceRequest request, CancellationToken cancellationToken = default)
    {
        var result = await inner.GenerateAsync(request, cancellationToken).ConfigureAwait(false);
        LastOutput = result.Text;
        LastTerminationReason = result.Termination.ToString();
        LastSampledTokenCount = result.SampledTokens;
        if (inner is LlamaSharpInferenceAdapter native)
        {
            LastTerminationReason = native.LastTermination.ToString();
            LastSampledTokenCount = native.LastSampledTokenCount;
        }
        return result;
    }
}
