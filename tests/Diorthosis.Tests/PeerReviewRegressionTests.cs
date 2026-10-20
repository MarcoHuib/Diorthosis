using System.Diagnostics;
using System.Text.Json;
using Diorthosis.Benchmarks;
using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;
using Xunit;

namespace Diorthosis.Tests;

public sealed class PeerReviewRegressionTests
{
    [Theory]
    [InlineData("The café recieved it.", "The cafe\u0301 received it.", 9)]
    [InlineData("The cafe\u0301 recieved it.", "The café received it.", 10)]
    public void CanonicalPrefixLengthDoesNotCorruptCorrectionAlignment(string input, string candidate, int start)
    {
        var item = new BenchmarkCase("nfc", CorrectionLanguage.English, input, [new(start, 8, "recieved", ErrorCategory.Spelling, [new("received")])]);
        var report = ReferenceEvaluator.Evaluate([item], [new(item.Id, [new(0, input.Length, input, candidate)])]);
        Assert.Single(report.Cases[0].RepairedAnnotations);
        Assert.True(report.Cases[0].ReferenceCompletePassage);
    }

    [Fact]
    public void CompoundCanonicalEditRepairsTwoAnnotationsDespiteUnrelatedPunctuation()
    {
        var item = new BenchmarkCase("compound", CorrectionLanguage.English, "He recieve it.", [
            new(6, 2, "ie", ErrorCategory.Spelling, [new("ei")]), new(10, 0, "", ErrorCategory.Grammar, [new("d")])]);
        var report = ReferenceEvaluator.Evaluate([item], [new(item.Id, [new(0, item.InputText.Length, item.InputText, "He received it?")])]);
        Assert.Equal(2, report.Cases[0].RepairedAnnotations.Count);
        Assert.Equal(2, report.Cases[0].Proposals.Count);
        Assert.Single(report.Cases[0].Proposals, proposal => proposal.ReferenceMatched);
        Assert.False(report.Cases[0].ReferenceCompletePassage);
    }

    [Fact]
    public void UnknownCompletePassageIsNotScoredAsFailure()
    {
        var source = string.Join(" ", Enumerable.Repeat("a", 9));
        var item = new BenchmarkCase("cap", CorrectionLanguage.English, source, Enumerable.Range(0, 9)
            .Select(index => new ErrorAnnotation(index * 2, 1, "a", ErrorCategory.Spelling, [new("b"), new("c")])).ToArray());
        var report = ReferenceEvaluator.Evaluate([item], [new(item.Id, [new(0, source.Length, source, source.Replace('a', 'b'))])]);
        var row = Assert.Single(report.Metrics, metric => metric.Language == "English" && metric.Category == "all");
        Assert.Equal(9, row.RepairRecall.Numerator);
        Assert.Null(row.CompleteReferencePassages.Estimate);
        Assert.Equal(1, row.UnknownCompletePassages);
        Assert.Equal(0, row.CompletePassageJudgmentCoverage!.Numerator);
    }

    [Fact]
    public void ReviewIdentityTracksProposalContentAcrossAnnotationReorderingAndRevision()
    {
        var item = new BenchmarkCase("review", CorrectionLanguage.English, "She have recieved it.", [new(4, 4, "have", ErrorCategory.Grammar, [new("has")]), new(9, 8, "recieved", ErrorCategory.Spelling, [new("received")])]);
        var prediction = new BenchmarkPrediction(item.Id, [new(0, item.InputText.Length, item.InputText, "She has receive it.")]);
        var original = ReferenceEvaluator.Evaluate([item], [prediction]).Cases[0];
        var correct = Assert.Single(original.Proposals, proposal => proposal.ReferenceMatched);
        var review = new ProposalReview(item.Id, original.CandidateSha256, correct.Id, true, true);
        var reversed = item with { Errors = item.Errors.Reverse().ToArray() };
        var scored = ReferenceEvaluator.Evaluate([reversed], [prediction], reviews: [review]);
        var grammar = Assert.Single(scored.Metrics, row => row.Language == "English" && row.Category == "Grammar");
        var spelling = Assert.Single(scored.Metrics, row => row.Language == "English" && row.Category == "Spelling");
        Assert.Equal(1, grammar.AdjudicatedProposalPrecision!.Numerator);
        Assert.Null(spelling.AdjudicatedProposalPrecision);
        var revised = item with { Errors = [item.Errors[0] with { AcceptedCorrections = [new("had")] }, item.Errors[1]] };
        Assert.Throws<ArgumentException>(() => ReferenceEvaluator.Evaluate([revised], [prediction], reviews: [review]));
    }

    [Theory]
    [InlineData("ineligible", "eligible", 17)]
    [InlineData("eligibleish", "eligible", 17)]
    public void ExplicitProtectedSpanIncludesBoundaryInsertions(string replacement, string protectedText, int start)
    {
        const string source = "The applicant is eligible.";
        var request = new CorrectionRequest(CorrectionLanguage.English, source, [new(start, protectedText.Length, protectedText)]);
        var output = source.Replace(protectedText, replacement, StringComparison.Ordinal);
        var result = ModelCorrectionEngine.AnalyzeInference(new(output, GenerationTermination.EndOfGeneration, 10), request);
        Assert.Equal(CorrectionDisposition.Rejected, result.Disposition);
        Assert.Empty(result.Edits);
    }

    [Theory]
    [InlineData("Is the file ready? The task is done. Wait.", "Is the file ready. The task is done? Wait.")]
    [InlineData("The server is ready. The server is running.", "The server is ready.")]
    [InlineData("Jose\u0301 approved the file.", "Jose\u0300 approved the file.")]
    [InlineData("Keep 'recieve' as quoted.", "Keep 'receive' as quoted.")]
    [InlineData("Keep ‘recieve’ as quoted.", "Keep ‘receive’ as quoted.")]
    [InlineData("Het start op 3 mei.", "Het start op 3 juni.")]
    [InlineData("It starts on May 3rd.", "It starts on June 3rd.")]
    [InlineData("There is no risk.", "There is risk.")]
    [InlineData("We sent it yesterday.", "We sent it tomorrow.")]
    [InlineData("She gave him the book.", "He gave her the book.")]
    [InlineData("Run foo() after bar().", "Run foo() before bar().")]
    [InlineData("You must read but we can write.", "We must read but you can write.")]
    public void VerifiedUnsafeCandidatesHaveNoDeliverableEdits(string source, string candidate)
    {
        var result = ModelCorrectionEngine.AnalyzeModelResponse(candidate, source);
        Assert.NotEqual(CorrectionDisposition.Accepted, result.Disposition);
        Assert.Empty(result.Edits);
    }

    [Fact]
    public void OrdinaryFinalFullStopAndSafeWordOrderAreAllowed()
    {
        Assert.Equal(CorrectionDisposition.Accepted, ModelCorrectionEngine.AnalyzeModelResponse("The service works.", "The service works").Disposition);
        Assert.Equal(CorrectionDisposition.Accepted, ModelCorrectionEngine.AnalyzeModelResponse("I will send the package tomorrow.", "Tomorrow I will the package send.").Disposition);
        Assert.Equal(CorrectionDisposition.Accepted, ModelCorrectionEngine.AnalyzeModelResponse("Please don't receive it.", "Please don't recieve it.").Disposition);
    }

    [Theory]
    [InlineData("originalText")]
    [InlineData("replacementText")]
    public void LoneEscapedSurrogatesArePublicDataErrors(string property)
    {
        var json = "[{\"start\":0,\"length\":1,\"originalText\":\"a\",\"replacementText\":\"b\"}]";
        json = property == "originalText" ? json.Replace("\"a\"", "\"\\ud800\"", StringComparison.Ordinal) : json.Replace("\"b\"", "\"\\ud800\"", StringComparison.Ordinal);
        Assert.Throws<InvalidDataException>(() => ModelCorrectionEngine.ParseAndValidate(json, "a"));
    }

    [Fact]
    public void StructuredCandidateIsNeverReinterpretedAsChatMarkup()
    {
        const string source = "Please recieve it.";
        const string candidate = "Please **receive** it.";
        var edits = new[] { new TextEdit(0, source.Length, source, candidate) };
        var json = JsonSerializer.Serialize(edits, new JsonSerializerOptions(JsonSerializerDefaults.Web));
        var result = ExperimentalCorrectionEngine.Analyze(new(json, GenerationTermination.EndOfGeneration, 20), new(CorrectionLanguage.English, source), CorrectionExperimentVariant.StructuredOffsets);
        Assert.Equal(CorrectionDisposition.Accepted, result.Disposition);
        Assert.Equal(candidate, SourceEdits.Apply(source, result.Edits));
        Assert.Equal(candidate, result.ExtractedCorrection);
    }

    [Fact]
    public async Task ExistingArtifactsAreNeverOverwrittenEvenWithoutPreflight()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".json");
        await File.WriteAllTextAsync(path, "original");
        try
        {
            Assert.Throws<IOException>(() => BenchmarkArtifactPolicy.RequireNewOutputs(path));
            await Assert.ThrowsAsync<IOException>(() => BenchmarkArtifactPolicy.WriteTextNewAsync(path, "replacement"));
            Assert.Equal("original", await File.ReadAllTextAsync(path));
        }
        finally { File.Delete(path); }
    }

    [Fact]
    public void ReplayRejectsMismatchedSourceAndLabelsLegacyIdentityUnverified()
    {
        using var report = JsonDocument.Parse("""{"datasetSha256":"expected","caseCount":1,"modelSha256":"unrelated"}""");
        Assert.Throws<InvalidDataException>(() => BenchmarkArtifactPolicy.ValidateReplay(report.RootElement, "wrong", 1, "trace"));
        Assert.Throws<InvalidDataException>(() => BenchmarkArtifactPolicy.ValidateReplay(report.RootElement, "expected", 2, "trace"));
        Assert.False(BenchmarkArtifactPolicy.ValidateReplay(report.RootElement, "expected", 1, "trace").TraceBoundToRun);
        using var bound = JsonDocument.Parse("""{"datasetSha256":"expected","caseCount":1,"traceSha256":"first-trace"}""");
        Assert.Throws<InvalidDataException>(() => BenchmarkArtifactPolicy.ValidateReplay(bound.RootElement, "expected", 1, "other-trace"));
    }
    [Fact]
    public async Task CompareCommandRefusesItsInputAsDestination()
    {
        var root = RepositoryRoot();
        var directory = Directory.CreateTempSubdirectory("diorthosis-compare-test-");
        try
        {
            var first = Path.Combine(directory.FullName, "first.json");
            var second = Path.Combine(directory.FullName, "second.json");
            var source = await File.ReadAllTextAsync(Path.Combine(root, "benchmarks/model-validation/results/v4/ministral-3b-report.json"));
            await File.WriteAllTextAsync(first, source);
            await File.WriteAllTextAsync(second, source);
            Assert.Equal(1, await RunBenchmark("compare", first, first, second));
            Assert.Equal(source, await File.ReadAllTextAsync(first));
            Assert.Equal(source, await File.ReadAllTextAsync(second));
            Assert.Equal(0, await RunBenchmark("compare", Path.Combine(directory.FullName, "new.json"), first, second));
        }
        finally { directory.Delete(recursive: true); }
    }

    [Fact]
    public async Task ReplayCommandRejectsWrongMetadataAndSeparatesUnboundLegacyClaims()
    {
        var directory = Directory.CreateTempSubdirectory("diorthosis-replay-test-");
        try
        {
            var cases = Path.Combine(directory.FullName, "cases.jsonl");
            var trace = Path.Combine(directory.FullName, "trace.jsonl");
            var report = Path.Combine(directory.FullName, "report.json");
            await File.WriteAllTextAsync(cases, """{"id":"clean","language":"English","inputText":"Correct text.","errors":[]}""" + "\n");
            await File.WriteAllTextAsync(trace, """{"caseId":"clean","input":"Correct text.","rawModelOutput":"Correct text.","terminationReason":"endOfGeneration"}""" + "\n");
            var output = new[] { "predictions.jsonl", "replayed.jsonl", "replayed-report.json" }.Select(name => Path.Combine(directory.FullName, name)).ToArray();
            await File.WriteAllTextAsync(report, """{"datasetSha256":"WRONG","caseCount":1,"modelSha256":"unrelated"}""");
            Assert.Equal(1, await RunBenchmark(["reparse", cases, trace, report, .. output]));
            Assert.All(output, path => Assert.False(File.Exists(path)));
            var hash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(await File.ReadAllBytesAsync(cases))).ToLowerInvariant();
            await File.WriteAllTextAsync(report, JsonSerializer.Serialize(new { datasetSha256 = hash, caseCount = 1, modelSha256 = "unrelated", metrics = Array.Empty<object>(), rawCandidateMetrics = Array.Empty<object>() }));
            Assert.Equal(0, await RunBenchmark(["reparse", cases, trace, report, .. output]));
            using var result = JsonDocument.Parse(await File.ReadAllTextAsync(output[2]));
            Assert.False(result.RootElement.TryGetProperty("modelSha256", out _));
            Assert.Equal("unrelated", result.RootElement.GetProperty("originalRunReport").GetProperty("modelSha256").GetString());
            Assert.False(result.RootElement.GetProperty("replayProvenance").GetProperty("binding").GetProperty("traceBoundToRun").GetBoolean());
            using var replayed = JsonDocument.Parse((await File.ReadAllLinesAsync(output[1]))[0]);
            Assert.Equal("endOfGeneration", replayed.RootElement.GetProperty("terminationReason").GetString());
            var emittedHash = Convert.ToHexString(System.Security.Cryptography.SHA256.HashData(await File.ReadAllBytesAsync(output[1]))).ToLowerInvariant();
            Assert.Equal(emittedHash, result.RootElement.GetProperty("traceSha256").GetString());
            var boundOutput = new[] { "bound-predictions.jsonl", "bound-trace.jsonl", "bound-report.json" }.Select(name => Path.Combine(directory.FullName, name)).ToArray();
            Assert.Equal(0, await RunBenchmark(["reparse", cases, output[1], output[2], .. boundOutput]));
            using var boundResult = JsonDocument.Parse(await File.ReadAllTextAsync(boundOutput[2]));
            Assert.True(boundResult.RootElement.GetProperty("replayProvenance").GetProperty("binding").GetProperty("traceBoundToRun").GetBoolean());
        }
        finally { directory.Delete(recursive: true); }
    }

    [Fact]
    public async Task AdjudicationCommandProjectsSharedJudgmentsWithoutPoolingModels()
    {
        var directory = Directory.CreateTempSubdirectory("diorthosis-review-projection-");
        try
        {
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
            options.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            ReviewSubject Subject(string id)
            {
                var value = new ReviewSubject("", "", id, CorrectionLanguage.English, "delivered", "proposal", "Please recieve it.", "Please receive it.", "[]", "{}", null, null, false);
                var hash = LinguisticReview.Binding(value);
                return value with { SubjectId = "review-" + hash, BindingSha256 = hash };
            }
            var one = Subject("one"); var two = Subject("two");
            var good = new LinguisticJudgment(true, true, true, true, true, false, true, [], [], "none", "Synthetic test fixture only.");
            var votes = new[] { one, two }.SelectMany(subject => new[] { "fixture-A", "fixture-B" }.Select(id => new ReviewDecision(subject.SubjectId, subject.BindingSha256,
                id, "human-declared", subject.Language, "Synthetic test role", true, 1, good))).ToArray();
            var finals = new[] { one, two }.Select(subject => new ReviewAdjudication(subject.SubjectId, subject.BindingSha256, "fixture-adjudicator", "human-declared", subject.Language,
                "Synthetic test role", true, 1, "approved", new Dictionary<string, int> { ["fixture-A"] = 1, ["fixture-B"] = 1 },
                subject == one ? good : good with { Justified = false, MeaningPreserved = null })).ToArray();
            var subjectsPath = Path.Combine(directory.FullName, "subjects.jsonl");
            var votesPath = Path.Combine(directory.FullName, "votes.jsonl"); var finalsPath = Path.Combine(directory.FullName, "finals.jsonl");
            var lineagePath = Path.Combine(directory.FullName, "lineage.json"); var outputPath = Path.Combine(directory.FullName, "report.json");
            await File.WriteAllLinesAsync(subjectsPath, new[] { one, two }.Select(value => JsonSerializer.Serialize(value, options)));
            await File.WriteAllLinesAsync(votesPath, votes.Select(value => JsonSerializer.Serialize(value, options)));
            await File.WriteAllLinesAsync(finalsPath, finals.Select(value => JsonSerializer.Serialize(value, options)));
            await File.WriteAllTextAsync(lineagePath, JsonSerializer.Serialize(new Dictionary<string, object> { [one.SubjectId] = new[] { new { model = "model-A" }, new { model = "model-B" } }, [two.SubjectId] = new[] { new { model = "model-A" } } }, options));
            Assert.Equal(0, await RunBenchmark("adjudicate", subjectsPath, votesPath, finalsPath, outputPath, "--lineage", lineagePath));
            using var report = JsonDocument.Parse(await File.ReadAllTextAsync(outputPath));
            var models = report.RootElement.GetProperty("byModel");
            var a = models.GetProperty("model-A").GetProperty("metrics")[0].GetProperty("proposalPrecision");
            var b = models.GetProperty("model-B").GetProperty("metrics")[0].GetProperty("proposalPrecision");
            Assert.Equal(1, a.GetProperty("numerator").GetInt32()); Assert.Equal(2, a.GetProperty("denominator").GetInt32());
            Assert.Equal(1, b.GetProperty("numerator").GetInt32()); Assert.Equal(1, b.GetProperty("denominator").GetInt32());
        }
        finally { directory.Delete(recursive: true); }
    }

    private static string RepositoryRoot()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "AGENTS.md"))) root = root.Parent;
        return root?.FullName ?? throw new InvalidOperationException("Repository root missing.");
    }

    private static async Task<int> RunBenchmark(params string[] arguments)
    {
        var configuration = typeof(PeerReviewRegressionTests).Assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyConfigurationAttribute), false)
            .Cast<System.Reflection.AssemblyConfigurationAttribute>().Single().Configuration;
        var executable = Path.Combine(RepositoryRoot(), "src/Diorthosis.Benchmarks/bin", configuration, "net10.0/Diorthosis.Benchmarks.dll");
        var start = new ProcessStartInfo("dotnet") { RedirectStandardOutput = true, RedirectStandardError = true };
        start.Environment["DOTNET_EnableDiagnostics"] = "0";
        start.ArgumentList.Add(executable);
        foreach (var argument in arguments) start.ArgumentList.Add(argument);
        using var process = Process.Start(start)!;
        var stdout = process.StandardOutput.ReadToEndAsync();
        var stderr = process.StandardError.ReadToEndAsync();
        using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
        await process.WaitForExitAsync(timeout.Token);
        await stdout; await stderr;
        return process.ExitCode;
    }

}
