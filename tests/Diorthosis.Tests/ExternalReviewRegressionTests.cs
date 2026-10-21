using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;
using Diorthosis.Experiments;
using Xunit;

namespace Diorthosis.Tests;

public sealed class ExternalReviewRegressionTests
{
    [Fact]
    public void SingleWordModelEmphasisDoesNotReachDeliveredPassage()
    {
        var analysis=ModelCorrectionEngine.AnalyzeModelResponse("She is an *experienced* engineer.","She is a experienced engineer.");
        Assert.Equal(CorrectionDisposition.Accepted,analysis.Disposition);
        Assert.Equal("She is an experienced engineer.",analysis.ExtractedCorrection);
        Assert.Equal("She is an *experienced* engineer.",analysis.RawOutput);
    }
    [Theory]
    [InlineData("Use *wildcard* exactly.")]
    [InlineData("The answer is 2 * 3 * 4.")]
    [InlineData("Keep `retry_count` and `OAuth2` unchanged.")]
    [InlineData("De coördinator bespreekt de reünie.")]
    [InlineData("The organisation will analyse the colour samples.")]
    [InlineData("Het Griekse woord κόσμος blijft staan.")]
    public void CorrectSourceFormattingUnicodeAndRegionalTextRemainLiteral(string source)
    {
        var analysis=ModelCorrectionEngine.AnalyzeModelResponse(source,source);
        Assert.Equal(CorrectionDisposition.Unchanged,analysis.Disposition);
        Assert.Equal(source,analysis.ExtractedCorrection);
    }
    [Fact]
    public void RealRepairPreservesExistingLiteralAsterisks()
    {
        var source="She is a *experienced* engineer.";
        var candidate="She is an *experienced* engineer.";
        var analysis=ModelCorrectionEngine.AnalyzeModelResponse(candidate,source);
        Assert.Equal(CorrectionDisposition.Accepted,analysis.Disposition);
        Assert.Equal(candidate,analysis.ExtractedCorrection);
    }
    [Theory]
    [InlineData("Set *retry_count* to 3. If *OAuth2* returns an error, do not retry.")]
    [InlineData("Set `__retry_count__` to 3. If `__OAuth2__` returns an error, do not retry.")]
    [InlineData("Set retry\\_count to 3. If OAuth2 returns an error, do not retry.")]
    [InlineData("Set **\\u0060retry_count\\u0060** to 3. If **\\u0060OAuth2\\u0060** returns an error, do not retry.")]
    public void TechnicalCorruptionIsAlreadyWithheld(string candidate)
    {
        const string source="Set `retry_count` to 3. If `OAuth2` returns an error, do not retry.";
        var analysis=ModelCorrectionEngine.AnalyzeModelResponse(candidate,source);
        Assert.Equal(CorrectionDisposition.Rejected,analysis.Disposition);
        Assert.Empty(analysis.Edits);
        Assert.Equal(source,SourceEdits.Apply(source,analysis.Edits));
    }
    [Theory]
    [InlineData("Please explain me the procedure.","Please explain the procedure.")]
    [InlineData("De kinderen loop naar de biblioteek.","De kinderen gaan de bibliotheek in.")]
    [InlineData("The neighborhood theater canceled the evening performance while staff repaired the elevator.","The neighborhood theater had plans to cancel the evening performance, however, staff were busy repairing the elevator before the scheduled showtime.")]
    public void AlreadyBlockedSemanticFailuresStayBlocked(string source,string candidate)
    {
        var analysis=ModelCorrectionEngine.AnalyzeModelResponse(candidate,source);
        Assert.Empty(analysis.Edits);
        Assert.Contains(analysis.Disposition,new[]{CorrectionDisposition.Rejected,CorrectionDisposition.NeedsReview});
    }
    [Fact]
    public void ExistingMixedScriptCorruptionGuardCannotOverrideSafeUnicodeContent()
    {
        const string source="De coördinator besprak de reünie in een café waar ook geïmproviseerde muziek klonk.";
        const string candidate="De coordinator besprak deոց een café waar ook improviserende muziek klonk.";
        var request=new CorrectionRequest(CorrectionLanguage.Dutch,source);
        var samples=Enumerable.Range(0,3).Select(i=>new VotingCandidate(i.ToString(),new(candidate,GenerationTermination.EndOfGeneration,30))).ToArray();
        Assert.Empty(EditVoting.Aggregate(request,samples).Analysis.Edits);
        Assert.False(LocalEdits.Derive("Het woord κόσμος blijft.","Het woord κόσμος blijft.").IntroducesNonLatinLetters);
        // A new Greek accent is legitimate Unicode but falls outside the experimental Latin allowlist.
        // This known scope limitation is why that filter is not promoted to production.
        Assert.True(LocalEdits.Derive("Het woord κοσμος blijft.","Het woord κόσμος blijft.").IntroducesNonLatinLetters);
    }
}
