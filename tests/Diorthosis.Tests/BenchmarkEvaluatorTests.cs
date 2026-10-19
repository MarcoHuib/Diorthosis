using Diorthosis.Benchmarks;
using Diorthosis.Core.Corrections;
using Xunit;

namespace Diorthosis.Tests;

public sealed class BenchmarkEvaluatorTests
{
    [Fact]
    public void EvaluatorCountsAcceptedAlternativesPrecisionAndCleanInputChanges()
    {
        var cases = new[]
        {
            new BenchmarkCase("nl-1", CorrectionLanguage.Dutch, "Ik loopd.", [
                new ErrorAnnotation(3, 5, "loopd", ErrorCategory.Spelling,
                    [new AcceptedCorrection("loop"), new AcceptedCorrection("liep")])
            ]),
            new BenchmarkCase("en-clean", CorrectionLanguage.English, "This is correct.", [])
        };
        var predictions = new[]
        {
            new BenchmarkPrediction("nl-1", [new TextEdit(3, 5, "loopd", "liep"), new TextEdit(0, 2, "Ik", "ik")]),
            new BenchmarkPrediction("en-clean", [new TextEdit(0, 4, "This", "That")])
        };

        var metrics = BenchmarkEvaluator.Evaluate(cases, predictions);
        var dutch = Assert.Single(metrics, result => result.Language == "Dutch" && result.Category == "all");
        var cleanEnglish = Assert.Single(metrics, result => result.Language == "English" && result.Category == "CorrectText");

        Assert.Equal(1, dutch.CorrectlyResolvedErrors);
        Assert.Equal(2, dutch.SuggestedEdits);
        Assert.Equal(1, dutch.FalsePositiveSuggestions);
        Assert.Equal(0.5, dutch.Precision);
        Assert.Equal(1.0, dutch.Recall);
        Assert.Equal(1, cleanEnglish.CorrectInputsChanged);
        Assert.Equal(1.0, cleanEnglish.FalsePositiveRate);
        Assert.Equal(0.0, cleanEnglish.CorrectTextPreservation);
    }

    [Fact]
    public void EvaluatorRejectsEditWhoseOriginalSpanDoesNotMatchInput()
    {
        var cases = new[] { new BenchmarkCase("en-1", CorrectionLanguage.English, "hello", []) };
        var predictions = new[] { new BenchmarkPrediction("en-1", [new TextEdit(0, 5, "world", "there")]) };

        Assert.Throws<ArgumentException>(() => BenchmarkEvaluator.Evaluate(cases, predictions));
    }

    [Fact]
    public void EvaluatorIsStableAcrossRepeatedRuns()
    {
        var cases = new[] { new BenchmarkCase("en-1", CorrectionLanguage.English, "teh", [
            new ErrorAnnotation(0, 3, "teh", ErrorCategory.Spelling, [new AcceptedCorrection("the")])
        ]) };
        var predictions = new[] { new BenchmarkPrediction("en-1", [new TextEdit(0, 3, "teh", "the")]) };

        Assert.Equal(BenchmarkEvaluator.Evaluate(cases, predictions), BenchmarkEvaluator.Evaluate(cases, predictions));
    }

    [Fact]
    public void EvaluatorAcceptsDifferentEditBoundariesOnlyWhenWholeAnnotatedRepairMatches()
    {
        var item = new BenchmarkCase("nl-compound", CorrectionLanguage.Dutch, "De vergader ruimte is klaar.", [
            new ErrorAnnotation(3, 15, "vergader ruimte", ErrorCategory.Spelling,
                [new AcceptedCorrection("vergaderruimte")])
        ]);
        var correct = new BenchmarkPrediction(item.Id, [new TextEdit(3, 15, "vergader ruimte", "vergaderruimte")]);
        var partial = new BenchmarkPrediction(item.Id, [new TextEdit(3, 8, "vergader", "vergader")]);
        var meaningChanging = new BenchmarkPrediction(item.Id, [new TextEdit(3, 15, "vergader ruimte", "vergaderruimte verloren")]);

        var metrics = BenchmarkEvaluator.Evaluate([item], [correct]);
        var all = Assert.Single(metrics, result => result.Language == "Dutch" && result.Category == "all");
        Assert.Equal(1, all.CorrectlyResolvedErrors);
        Assert.Equal(1, all.Precision);

        var partialMetrics = BenchmarkEvaluator.Evaluate([item], [partial]);
        var partialAll = Assert.Single(partialMetrics, result => result.Language == "Dutch" && result.Category == "all");
        Assert.Equal(0, partialAll.CorrectlyResolvedErrors);
        Assert.Equal(1, partialAll.FalsePositiveSuggestions);

        var changedMetrics = BenchmarkEvaluator.Evaluate([item], [meaningChanging]);
        var changedAll = Assert.Single(changedMetrics, result => result.Language == "Dutch" && result.Category == "all");
        Assert.Equal(0, changedAll.CorrectlyResolvedErrors);
    }

    [Fact]
    public void EvaluatorAcceptsMultipleFragmentsOnlyWhenTogetherTheyMakeExactRepair()
    {
        var item = new BenchmarkCase("en-1", CorrectionLanguage.English, "She have recieved it.", [
            new ErrorAnnotation(4, 4, "have", ErrorCategory.Grammar, [new AcceptedCorrection("has")]),
            new ErrorAnnotation(9, 8, "recieved", ErrorCategory.Spelling, [new AcceptedCorrection("received")])
        ]);
        var prediction = new BenchmarkPrediction(item.Id, [
            new TextEdit(4, 4, "have", "has"),
            new TextEdit(9, 8, "recieved", "received")
        ]);
        var all = Assert.Single(BenchmarkEvaluator.Evaluate([item], [prediction]),
            result => result.Language == "English" && result.Category == "all");
        Assert.Equal(2, all.CorrectlyResolvedErrors);
        Assert.Equal(0, all.FalsePositiveSuggestions);
    }

    [Fact]
    public void EvaluatorDoesNotCreditLocalRepairInsideUnannotatedLexicalRewrite()
    {
        var item = new BenchmarkCase("nl-translation", CorrectionLanguage.Dutch,
            "Er zijn een probleem met de verbinding.", [
                new ErrorAnnotation(3, 4, "zijn", ErrorCategory.Grammar, [new AcceptedCorrection("is")])
            ]);
        var prediction = new BenchmarkPrediction(item.Id, [
            new TextEdit(0, 2, "Er", "There"),
            new TextEdit(3, 4, "zijn", "is"),
            new TextEdit(28, 10, "verbinding", "connection")
        ]);
        var all = Assert.Single(BenchmarkEvaluator.Evaluate([item], [prediction]),
            result => result.Language == "Dutch" && result.Category == "all");
        Assert.Equal(0, all.CorrectlyResolvedErrors);
        Assert.Equal(0, all.CorrectSuggestions);
    }
}
