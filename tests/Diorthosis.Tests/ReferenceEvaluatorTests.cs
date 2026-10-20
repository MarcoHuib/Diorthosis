using System.Text.Json;
using System.Text.Json.Serialization;
using Diorthosis.Benchmarks;
using Diorthosis.Core.Corrections;
using Xunit;

namespace Diorthosis.Tests;

public sealed class ReferenceEvaluatorTests
{
    private static readonly BenchmarkCase Mixed = new("mixed", CorrectionLanguage.English, "She have recieved it.", [
        new(4, 4, "have", ErrorCategory.Grammar, [new("has")]), new(9, 8, "recieved", ErrorCategory.Spelling, [new("received")])]);
    private static ReferenceMetrics All(ReferenceReport report) => Assert.Single(report.Metrics, row => row.Language == "English" && row.Category == "all");

    [Fact]
    public void ScoresOutputIdenticallyAcrossLocalCompoundAndFullPassageHunks()
    {
        IReadOnlyList<TextEdit>[] representations = [
            [new(4, 4, "have", "has"), new(9, 8, "recieved", "received")],
            [new(4, 13, "have recieved", "has received")],
            [new(0, Mixed.InputText.Length, Mixed.InputText, "She has received it.")]];
        var results = representations.Select(edits => ReferenceEvaluator.Evaluate([Mixed], [new(Mixed.Id, edits)])).ToArray();
        Assert.All(results, report => { Assert.Equal(2, All(report).RepairRecall.Numerator); Assert.Equal(2, All(report).ProposedGroups); });
        Assert.All(results, report => Assert.Equal(JsonSerializer.Serialize(results[0]), JsonSerializer.Serialize(report)));
        Assert.Equal(1, Assert.Single(results[0].Metrics, row => row.Language == "English" && row.Category == "Grammar").RepairRecall.Numerator);
        Assert.Equal(1, Assert.Single(results[0].Metrics, row => row.Language == "English" && row.Category == "Spelling").RepairRecall.Numerator);
    }

    [Fact]
    public void LocalRecallSurvivesUnrelatedLexicalEditAndSpeechActChange()
    {
        var item = new BenchmarkCase("a", CorrectionLanguage.English, "Please recieve the form.", [new(7, 7, "recieve", ErrorCategory.Spelling, [new("receive")])]);
        foreach (var output in new[] { "Kindly receive the form.", "Please receive the form?" })
        {
            var report = ReferenceEvaluator.Evaluate([item], [new(item.Id, [new(0, item.InputText.Length, item.InputText, output)])]);
            Assert.Equal(1, All(report).RepairRecall.Numerator);
            Assert.Equal(1, All(report).ReferenceUnresolvedGroups);
            Assert.Equal(0, All(report).CompleteReferencePassages.Numerator);
            Assert.Null(All(report).AdjudicatedProposalPrecision);
        }
    }

    [Fact]
    public void PartialDependencyRepairReceivesNoCredit()
    {
        var item = Mixed with { Errors = Mixed.Errors.Select(error => error with { DependencyGroup = "agreement" }).ToArray() };
        var partial = ReferenceEvaluator.Evaluate([item], [new(item.Id, [new(4, 4, "have", "has")])]);
        Assert.Equal(0, All(partial).RepairRecall.Numerator);
        var full = ReferenceEvaluator.Evaluate([item], [new(item.Id, [new(0, item.InputText.Length, item.InputText, "She has received it.")])]);
        Assert.Equal(2, All(full).RepairRecall.Numerator);
        Assert.Equal(1, All(full).ProposedGroups);
    }

    [Fact]
    public void CanonicalEquivalenceIsForScoringAndDoesNotRewriteSource()
    {
        var item = new BenchmarkCase("a", CorrectionLanguage.English, "It is naive.", [new(6, 5, "naive", ErrorCategory.Spelling, [new("naïve")])]);
        var report = ReferenceEvaluator.Evaluate([item], [new(item.Id, [new(6, 5, "naive", "naïve")])]);
        Assert.Equal(1, All(report).RepairRecall.Numerator);
        Assert.Equal("It is naive.", item.InputText);
        Assert.Equal(1, All(report).CompleteReferencePassages.Numerator);
    }

    [Fact]
    public void MissingPredictionsAreMissingCoverageAndCannotInflateCleanPreservation()
    {
        var clean = new BenchmarkCase("clean", CorrectionLanguage.English, "Correct text.", []);
        var report = ReferenceEvaluator.Evaluate([Mixed, clean], []);
        Assert.Equal(0, All(report).PredictionCoverage.Numerator);
        Assert.Equal(2, All(report).PredictionCoverage.Denominator);
        Assert.Null(All(report).CleanLiteralPreservation.Estimate);
        Assert.Null(All(report).CompleteReferencePassages.Estimate);
        Assert.Null(All(report).ReferenceGroupPrecision.Estimate);
        Assert.Null(ReferenceEvaluator.Ratio(0, 0).WilsonUpper);
        Assert.InRange(ReferenceEvaluator.Ratio(7, 7).WilsonLower!.Value, 0.64, 0.65);
    }

    [Fact]
    public void CleanLiteralChangeIsCountedEvenIfCanonicallyEquivalent()
    {
        var item = new BenchmarkCase("clean", CorrectionLanguage.English, "naïve", []);
        var report = ReferenceEvaluator.Evaluate([item], [new(item.Id, [new(0, 5, "naïve", "naïve")])]);
        Assert.Equal(0, All(report).CleanLiteralPreservation.Numerator);
    }

    [Fact]
    public void ReviewsMustBindToExactCandidateAndProposal()
    {
        var prediction = new BenchmarkPrediction(Mixed.Id, [new(4, 4, "have", "has")]);
        var unreviewed = ReferenceEvaluator.Evaluate([Mixed], [prediction]);
        var candidate = unreviewed.Cases[0];
        var review = new ProposalReview(Mixed.Id, candidate.CandidateSha256, candidate.Proposals[0].Id, true, true);
        Assert.Equal(1, All(ReferenceEvaluator.Evaluate([Mixed], [prediction], reviews: [review])).AdjudicatedProposalPrecision!.Numerator);
        Assert.Throws<ArgumentException>(() => ReferenceEvaluator.Evaluate([Mixed], [prediction], reviews: [review with { CandidateSha256 = "wrong" }]));
    }

    [Fact]
    public void RejectedNoEditPredictionIsVisibleAndNotAnAcceptedRepair()
    {
        var report = ReferenceEvaluator.Evaluate([Mixed], [new(Mixed.Id, [])], new Dictionary<string, CorrectionDisposition> { [Mixed.Id] = CorrectionDisposition.Rejected });
        Assert.Equal(1, All(report).Dispositions["Rejected"]);
        Assert.Equal(0, All(report).RepairRecall.Numerator);
        Assert.Equal(1, All(report).PredictionCoverage.Numerator);
    }

    [Fact]
    public void PreservesHistoricalScoresAndRecognizesProposedRestructuringAndApostrophes()
    {
        var root = new DirectoryInfo(AppContext.BaseDirectory);
        while (root is not null && !File.Exists(Path.Combine(root.FullName, "AGENTS.md"))) root = root.Parent;
        Assert.NotNull(root);
        var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        T[] Read<T>(string path) => File.ReadLines(Path.Combine(root.FullName, path)).Where(line => !string.IsNullOrWhiteSpace(line)).Select(line => JsonSerializer.Deserialize<T>(line, options)!).ToArray();
        var cases = Read<BenchmarkCase>("benchmarks/model-validation/smoke-44-reviewed-v2.jsonl");
        var proposed = Read<BenchmarkCase>("benchmarks/model-validation/audits/quality-v1/smoke-44-proposed-v3.jsonl");
        foreach (var model in new[] { "ministral-3b", "ministral-3-8b", "ministral-3-14b" })
        {
            var predictions = Read<BenchmarkPrediction>($"benchmarks/model-validation/results/v4/{model}-predictions.jsonl");
            using var historical = JsonDocument.Parse(File.ReadAllText(Path.Combine(root.FullName, $"benchmarks/model-validation/results/v4/{model}-report.json")));
            var expected = JsonSerializer.Deserialize<MetricResult[]>(historical.RootElement.GetProperty("metrics"), options)!;
            Assert.Equal(expected, BenchmarkEvaluator.EvaluateHistorical(cases, predictions));
            var sensitivity = ReferenceEvaluator.Evaluate(proposed, predictions);
            Assert.True(sensitivity.Cases.Single(item => item.CaseId == "en-04").ReferenceCompletePassage);
            Assert.True(sensitivity.Cases.Single(item => item.CaseId == "en-05").ReferenceCompletePassage);
            Assert.True(sensitivity.Cases.Single(item => item.CaseId == "en-09").ReferenceCompletePassage);
            if (model != "ministral-3b") Assert.True(sensitivity.Cases.Single(item => item.CaseId == "nl-12").ReferenceCompletePassage);
        }
    }
}
