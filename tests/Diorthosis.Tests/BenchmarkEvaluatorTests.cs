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
    public void EvaluatorCreditsOneCompleteEditAcrossMultipleAnnotations()
    {
        var item = new BenchmarkCase("en-combined", CorrectionLanguage.English, "She have recieved it.", [
            new ErrorAnnotation(4, 4, "have", ErrorCategory.Grammar, [new AcceptedCorrection("has")]),
            new ErrorAnnotation(9, 8, "recieved", ErrorCategory.Spelling, [new AcceptedCorrection("received")])
        ]);
        var prediction = new BenchmarkPrediction(item.Id,
            [new TextEdit(4, 13, "have recieved", "has received")]);

        var all = Assert.Single(BenchmarkEvaluator.Evaluate([item], [prediction]),
            result => result.Language == "English" && result.Category == "all");
        Assert.Equal(2, all.CorrectlyResolvedErrors);
        Assert.Equal(1, all.CorrectSuggestions);

        var grammar = Assert.Single(BenchmarkEvaluator.Evaluate([item], [prediction]),
            result => result.Language == "English" && result.Category == "Grammar");
        var spelling = Assert.Single(BenchmarkEvaluator.Evaluate([item], [prediction]),
            result => result.Language == "English" && result.Category == "Spelling");
        Assert.Equal(1, grammar.CorrectlyResolvedErrors);
        Assert.Equal(1, spelling.CorrectlyResolvedErrors);
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

    [Fact]
    public void CategoryRowsAttributeOnlyEditsForThatCategory()
    {
        var item = new BenchmarkCase("mixed", CorrectionLanguage.English, "She have recieved it.", [
            new ErrorAnnotation(4, 4, "have", ErrorCategory.Grammar, [new AcceptedCorrection("has")]),
            new ErrorAnnotation(9, 8, "recieved", ErrorCategory.Spelling, [new AcceptedCorrection("received")])
        ]);
        var prediction = new BenchmarkPrediction(item.Id, [
            new TextEdit(4, 4, "have", "has"),
            new TextEdit(9, 8, "recieved", "received")
        ]);

        var rows = BenchmarkEvaluator.Evaluate([item], [prediction]);
        var grammar = Assert.Single(rows, row => row.Language == "English" && row.Category == "Grammar");
        var spelling = Assert.Single(rows, row => row.Language == "English" && row.Category == "Spelling");
        Assert.Equal(1, grammar.SuggestedEdits);
        Assert.Equal(1, grammar.CorrectSuggestions);
        Assert.Equal(1, spelling.SuggestedEdits);
        Assert.Equal(1, spelling.CorrectSuggestions);
    }

    [Fact]
    public void CategoryRecallSurvivesAnIncorrectRepairInAnotherCategory()
    {
        var item = new BenchmarkCase("mixed-partial", CorrectionLanguage.English, "She have recieved it.", [
            new ErrorAnnotation(4, 4, "have", ErrorCategory.Grammar, [new AcceptedCorrection("has")]),
            new ErrorAnnotation(9, 8, "recieved", ErrorCategory.Spelling, [new AcceptedCorrection("received")])
        ]);
        var prediction = new BenchmarkPrediction(item.Id, [
            new TextEdit(4, 4, "have", "has"),
            new TextEdit(9, 8, "recieved", "recovered")
        ]);

        var grammar = Assert.Single(BenchmarkEvaluator.Evaluate([item], [prediction]),
            row => row.Language == "English" && row.Category == "Grammar");
        Assert.Equal(1, grammar.CorrectlyResolvedErrors);
        Assert.Equal(1, grammar.CorrectSuggestions);
    }

    [Fact]
    public void LegacyEvaluatorRemainsAvailableForHistoricalRows()
    {
        var item = new BenchmarkCase("legacy", CorrectionLanguage.English, "She have it.", [
            new ErrorAnnotation(4, 4, "have", ErrorCategory.Grammar, [new AcceptedCorrection("has")])
        ]);
        var prediction = new BenchmarkPrediction(item.Id, [new TextEdit(4, 4, "have", "has")]);
        var current = Assert.Single(BenchmarkEvaluator.Evaluate([item], [prediction]), row => row.Language == "English" && row.Category == "all");
        var legacy = Assert.Single(BenchmarkEvaluator.EvaluateHistorical([item], [prediction]), row => row.Language == "English" && row.Category == "all");
        Assert.Equal(BenchmarkEvaluator.HistoricalEvaluatorVersion, BenchmarkEvaluator.HistoricalEvaluatorVersion);
        Assert.Equal(legacy, current);
    }

    [Fact]
    public void QualityMetricsDoNotTurnUnavailableDispositionsIntoZero()
    {
        var item = new BenchmarkCase("quality", CorrectionLanguage.English, "Correct text.", []);
        var unavailable = Assert.Single(BenchmarkEvaluator.EvaluateQuality([item], []), row => row.Language == "English");
        Assert.False(unavailable.DispositionsAvailable);
        Assert.Null(unavailable.AcceptedChanges);
        Assert.Null(unavailable.RejectedChanges);
        Assert.Null(unavailable.SuppressedChanges);
        Assert.Null(unavailable.UnchangedChanges);
        Assert.Null(unavailable.NeedsReviewChanges);

        var available = Assert.Single(BenchmarkEvaluator.EvaluateQuality([item], [],
            new Dictionary<string, CorrectionDisposition> { [item.Id] = CorrectionDisposition.Unchanged }),
            row => row.Language == "English");
        Assert.True(available.DispositionsAvailable);
        Assert.Equal(1, available.UnchangedChanges);
    }
}
