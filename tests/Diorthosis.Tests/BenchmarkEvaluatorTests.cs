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
            new BenchmarkCase("nl-1", CorrectionLanguage.Dutch, "Ik loopd.", new[]
            {
                new ErrorAnnotation(3, 5, "loopd", ErrorCategory.Spelling,
                    new[] { new AcceptedCorrection("loop"), new AcceptedCorrection("liep") })
            }),
            new BenchmarkCase("en-clean", CorrectionLanguage.English, "This is correct.", Array.Empty<ErrorAnnotation>())
        };
        var predictions = new[]
        {
            new BenchmarkPrediction("nl-1", new[] { new TextEdit(3, 5, "loopd", "liep"), new TextEdit(0, 2, "Ik", "ik") }),
            new BenchmarkPrediction("en-clean", new[] { new TextEdit(0, 4, "This", "That") })
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
        var cases = new[] { new BenchmarkCase("en-1", CorrectionLanguage.English, "hello", Array.Empty<ErrorAnnotation>()) };
        var predictions = new[] { new BenchmarkPrediction("en-1", new[] { new TextEdit(0, 5, "world", "there") }) };

        Assert.Throws<ArgumentException>(() => BenchmarkEvaluator.Evaluate(cases, predictions));
    }

    [Fact]
    public void EvaluatorIsStableAcrossRepeatedRuns()
    {
        var cases = new[] { new BenchmarkCase("en-1", CorrectionLanguage.English, "teh", new[]
        {
            new ErrorAnnotation(0, 3, "teh", ErrorCategory.Spelling, new[] { new AcceptedCorrection("the") })
        }) };
        var predictions = new[] { new BenchmarkPrediction("en-1", new[] { new TextEdit(0, 3, "teh", "the") }) };

        Assert.Equal(BenchmarkEvaluator.Evaluate(cases, predictions), BenchmarkEvaluator.Evaluate(cases, predictions));
    }
}
