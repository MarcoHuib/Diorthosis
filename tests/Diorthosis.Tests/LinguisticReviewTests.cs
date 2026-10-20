using Diorthosis.Benchmarks;
using Diorthosis.Core.Corrections;

namespace Diorthosis.Tests;

public sealed class LinguisticReviewTests
{
    private static ReviewSubject Subject(string kind = "proposal")
    {
        var value = new ReviewSubject("", "", "case", CorrectionLanguage.Dutch, "delivered", kind, "Dit werkt.", "Dit werkt.", "[]", "{\"status\":\"development\"}", null, null, true);
        var hash = LinguisticReview.Binding(value);
        return value with { SubjectId = "review-" + hash, BindingSha256 = hash };
    }
    private static readonly LinguisticJudgment Good = new(true, true, true, true, true, false, true, [], [], "none declared", "Test fixture only; no real human identity or approval.");
    private static ReviewDecision Vote(ReviewSubject subject, string id, string kind = "human-declared") => new(subject.SubjectId, subject.BindingSha256, id, kind, subject.Language, "Synthetic test reviewer", true, 1, Good);
    private static ReviewAdjudication Final(ReviewSubject subject) => new(subject.SubjectId, subject.BindingSha256, "test-adjudicator", "human-declared", subject.Language,
        "Synthetic test adjudicator", true, 1, "approved", new Dictionary<string, int> { ["test-A"] = 1, ["test-B"] = 1 }, Good);

    [Fact]
    public void NoReviewsOrAiReviewsCannotProduceLinguisticPrecision()
    {
        var subject = Subject();
        var report = LinguisticReview.Evaluate([subject], [Vote(subject, "AI", "ai")], []);
        Assert.Null(Assert.Single(report.Metrics).ProposalPrecision.Estimate);
        Assert.Equal(1, report.Metrics[0].Pending);
        Assert.Null(report.Metrics[0].KnownDocuments);
        Assert.Equal(1, report.Metrics[0].UnknownSourceFamilies);
        Assert.Throws<InvalidDataException>(() => LinguisticReview.Evaluate([subject], [Vote(subject, "test-A", "ai"), Vote(subject, "test-B")], [Final(subject)]));
    }
    [Fact]
    public void TwoHumanDeclarationsAndSeparateAdjudicatorRequiredAndDisagreementRetained()
    {
        var subject = Subject(); var first = Vote(subject, "test-A"); var second = Vote(subject, "test-B") with { Judgment = Good with { Justified = false } };
        Assert.Throws<InvalidDataException>(() => LinguisticReview.Evaluate([subject], [first], [Final(subject)]));
        Assert.Throws<InvalidDataException>(() => LinguisticReview.Evaluate([subject], [first, second], [Final(subject) with { AdjudicatorId = " TEST-a " }]));
        var report = LinguisticReview.Evaluate([subject], [first, second], [Final(subject)]);
        Assert.Equal(1, report.Metrics[0].Disagreements);
        Assert.Equal(1.0, report.Metrics[0].ProposalPrecision.Estimate);
        Assert.Equal(2, report.DecisionHistory.Count);
    }
    [Fact]
    public void StaleSourceCandidateAnnotationAndDecisionVersionFail()
    {
        var subject = Subject(); var a = Vote(subject, "test-A"); var b = Vote(subject, "test-B");
        foreach (var altered in new[] { subject with { SourceText = "Anders." }, subject with { CandidateText = "Andere kandidaat." }, subject with { AnnotationsJson = "[{}]" }, subject with { SourceFamily = "changed" } })
            Assert.Throws<InvalidDataException>(() => LinguisticReview.Evaluate([altered], [], []));
        Assert.Throws<InvalidDataException>(() => LinguisticReview.Evaluate([subject], [a, b, b with { Version = 2 }], [Final(subject)]));
        Assert.Throws<InvalidDataException>(() => LinguisticReview.Evaluate([subject], [a, a], []));
        Assert.Throws<InvalidDataException>(() => LinguisticReview.Evaluate([subject], [a with { Language = CorrectionLanguage.English }], []));
    }
    [Fact]
    public void UnknownPassageJudgmentsStayOutOfSuccessDenominator()
    {
        var subject = Subject("passage");
        var report = LinguisticReview.Evaluate([subject], [Vote(subject, "test-A"), Vote(subject, "test-B")], [Final(subject) with { Judgment = Good with { AllDefectsRepaired = null } }]);
        Assert.Null(report.Metrics[0].CompletePassageSuccess.Estimate);
        Assert.Equal(0, report.Metrics[0].CompletePassageSuccess.Denominator);
        Assert.Equal(1, report.Metrics[0].AdjudicationCoverage.Numerator);
        Assert.Equal(1, report.Metrics[0].CleanPreservation.Numerator);
    }
}
