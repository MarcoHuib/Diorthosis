using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;
using Diorthosis.Experiments;
using Xunit;

namespace Diorthosis.Tests;

public sealed class LinguisticExperimentTests
{
    private static VotingResult Vote(string source, params string[] outputs) => EditVoting.Aggregate(new(CorrectionLanguage.English, source),
        outputs.Select((text, index) => new VotingCandidate(index.ToString(), new(text, GenerationTermination.EndOfGeneration, 12))).ToArray());

    [Fact]
    public void StrictMajorityAndUnchangedMinority()
    {
        var result = Vote("We recieved it.", "We received it.", "We received it.", "We recieved it.");
        Assert.Equal(2, result.Threshold);
        Assert.Equal("We received it.", result.Analysis.ExtractedCorrection);
        Assert.Single(result.Support);
        Assert.Equal(2, result.Support[0].GenerationIds.Count);
    }
    [Fact]
    public void MinorityValidRepairIsLost()
    {
        var result = Vote("We recieved it.", "We received it.", "We recieved it.", "We recieved it.");
        Assert.Empty(result.Analysis.Edits);
        Assert.Equal(CorrectionDisposition.Unchanged, result.Analysis.Disposition);
    }
    [Fact]
    public void FiveRequiresThreeAndDoesNotCountTwo()
    {
        Assert.Equal(CorrectionDisposition.Unchanged, Vote("We recieved it.", "We received it.", "We received it.", "We recieved it.", "We recieved it.", "We recieved it.").Analysis.Disposition);
        Assert.Equal(CorrectionDisposition.Accepted, Vote("We recieved it.", "We received it.", "We received it.", "We received it.", "We recieved it.", "We recieved it.").Analysis.Disposition);
    }
    [Fact]
    public void DifferentReplacementsAbstain()
    {
        var result = Vote("The cat sit here.", "The cat sits here.", "The cat sat here.", "The cat sit here.");
        Assert.Empty(result.Analysis.Edits);
    }
    [Fact]
    public void MajorityCanRepeatWrongSpelling()
    {
        var request = new CorrectionRequest(CorrectionLanguage.Dutch, "De vergadering start om negen uur.");
        var candidates = Enumerable.Range(0, 3).Select(i => new VotingCandidate(i.ToString(), new("De vergadering startt om negen uur.", GenerationTermination.EndOfGeneration, 12))).ToArray();
        var result = EditVoting.Aggregate(request, candidates);
        Assert.Equal(1, result.UniqueOutputs);
        Assert.Contains("no observed diversity", result.DiversityStatus);
        // No claim that agreement makes a typo safe; dictionary regression tests detect it separately.
        Assert.Equal(CorrectionDisposition.Accepted, result.Analysis.Disposition);
    }
    [Theory]
    [InlineData("Keep `node-04` unchanged.", "Keep `node-40` unchanged.")]
    [InlineData("Do not remove it.", "Do remove it.")]
    [InlineData("Keep the amount at 42.", "Keep the amount at 43.")]
    public void MajorityCannotOverrideSemanticAndIdentifierGuards(string source, string candidate)
    {
        var result = Vote(source, candidate, candidate, candidate);
        Assert.Empty(result.Analysis.Edits);
        Assert.Equal(CorrectionDisposition.Rejected, result.Analysis.Disposition);
    }
    [Fact]
    public void TruncationAndRejectedCandidatesDoNotLowerThreshold()
    {
        var request = new CorrectionRequest(CorrectionLanguage.English, "We recieved 42 items.");
        var result = EditVoting.Aggregate(request, [
            new("a", new("We received 42 items.", GenerationTermination.EndOfGeneration, 10)),
            new("b", new("We received 42 items.", GenerationTermination.MaximumTokens, 10)),
            new("c", new("We received 43 items.", GenerationTermination.EndOfGeneration, 10))]);
        Assert.Equal(2, result.Threshold);
        Assert.Equal(1, result.EligibleCandidates);
        Assert.Empty(result.Analysis.Edits);
    }
    [Fact]
    public void UnseenCombinationRequiresDependencyReview()
    {
        var result = Vote("We recieve teh adress.", "We receive the adress.", "We receive teh address.", "We recieve the address.");
        Assert.Equal(CorrectionDisposition.NeedsReview, result.Analysis.Disposition);
        Assert.Empty(result.Analysis.Edits);
        Assert.Contains("dependency", result.Analysis.RejectionReason!);
    }
    [Fact]
    public void DuplicateGenerationIdsRejected()
    {
        var c = new VotingCandidate("same", new("Hello.", GenerationTermination.EndOfGeneration, 2));
        Assert.Throws<ArgumentException>(() => EditVoting.Aggregate(new(CorrectionLanguage.English, "Hello."), [c,c,c]));
    }
    [Fact]
    public void RepeatedWordAmbiguityIsNotConfidence()
    {
        Assert.True(LocalEdits.Derive("We can can go.","We can go.").AlignmentAmbiguous);
        var result=Vote("We can can go.","We can go.","We can go.","We can go.");
        Assert.Empty(result.Analysis.Edits);
        Assert.Equal(0,result.EligibleCandidates);
    }
    [Fact]
    public void MinorityNegationNeverEntersResult()
    {
        var result = Vote("Do not remove it.", "Do remove it.", "Do not remove it.", "Do not remove it.");
        Assert.Empty(result.Analysis.Edits);
        Assert.Equal(2, result.EligibleCandidates);
    }
    [Fact]
    public void EquivalentSegmentationsCanonicalizeAndKeepDependencies()
    {
        const string source = "We recieve teh adress.";
        var local = new[] { new TextEdit(3,7,"recieve","receive"), new TextEdit(11,3,"teh","the") };
        var candidate = SourceEdits.Apply(source, local);
        var full = SourceEdits.Apply(source, [new(0,source.Length,source,candidate)]);
        Assert.Equal(LocalEdits.Derive(source,candidate).Operations, LocalEdits.Derive(source,full).Operations);
        Assert.Contains("dependency-linked", LocalEdits.Derive(source,candidate).DependencyPolicy);
    }
    [Theory]
    [InlineData("Ik bel bel morgen.", "Ik bel morgen.")]
    [InlineData("De lange termijnplanning.", "De langetermijnplanning.")]
    [InlineData("Ik weet dat hij heeft het bestand opgeslagen.", "Ik weet dat hij het bestand heeft opgeslagen.")]
    [InlineData("😀 cafe\u0301 recieve.", "😀 café receive.")]
    [InlineData("First paragraph.\nSecond sentnce.", "First paragraph.\nSecond sentence.")]
    public void LocalOperationsReconstructLiteralUnicodeAndRepeatedWords(string source, string target)
    {
        var group = LocalEdits.Derive(source,target);
        Assert.Equal(target,SourceEdits.Apply(source,group.Operations.Select(o=>o.Edit)));
    }
    [Fact]
    public void UnsafeOverlapsAndGraphemeSplitsRemainBlocked()
    {
        Assert.Throws<InvalidDataException>(()=>SourceEdits.Apply("abcd",[new(0,2,"ab","a"),new(1,2,"bc","b")]));
        Assert.Throws<InvalidDataException>(()=>SourceEdits.Apply("e\u0301",[new(0,1,"e","a")]));
    }
    [Fact]
    public void ExplicitProtectedSpansRemainAuthoritative()
    {
        var request = new CorrectionRequest(CorrectionLanguage.English,"We recieved it.",[new(3,8,"recieved")]);
        var c = Enumerable.Range(0,3).Select(i=>new VotingCandidate(i.ToString(),new("We received it.",GenerationTermination.EndOfGeneration,10))).ToArray();
        Assert.Empty(EditVoting.Aggregate(request,c).Analysis.Edits);
    }
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(float.NaN)]
    public void InvalidSamplingRejected(float temperature) => Assert.Throws<ArgumentOutOfRangeException>(()=>new ExperimentalSampling(101,temperature));
    [Fact]
    public void GreedyRemainsDefaultAndSeedExplicit()
    {
        Assert.Null(new InferenceRequest("Correct this.").Sampling);
        Assert.Equal((uint)101,new InferenceRequest("Correct this.",sampling:new(101)).Sampling!.Seed);
    }

    [Fact]
    public void ManagedDictionaryAffixesDialectsDiagnosticsAndLifetime()
    {
        var root = Path.Combine(Path.GetTempPath(),"diorthosis-dict-test-"+Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            string Pair(string name, string words)
            {
                var path=Path.Combine(root,name+".dic");
                File.WriteAllText(path,words);
                File.WriteAllText(Path.ChangeExtension(path,".aff"),"SET UTF-8\nSFX S Y 1\nSFX S 0 s .\n");
                return path;
            }
            var nl=Pair("nl","5\nstart\nrapport/S\nonmiddellijk\ncoördinator\nopgebeld\n");
            var us=Pair("us","4\ncolor\norganize\ncan't\nreport/S\n");
            var gb=Pair("gb","3\ncolour\norganise\nreport/S\n");
            using var dictionary=new HunspellDiagnostics(nl,us,gb);
            Assert.True(dictionary.Check(CorrectionLanguage.Dutch,"rapports"));
            Assert.True(dictionary.Check(CorrectionLanguage.Dutch,"coördinator"));
            Assert.True(dictionary.Check(CorrectionLanguage.English,"color"));
            Assert.True(dictionary.Check(CorrectionLanguage.English,"colour"));
            using var american=new HunspellDiagnostics(nl,us,gb,EnglishDialect.American);
            Assert.False(american.Check(CorrectionLanguage.English,"colour"));
            var request=new CorrectionRequest(CorrectionLanguage.Dutch,"start");
            var warnings=SpellingDiagnostics.InspectReplacements(dictionary,request,[new(5,0,"","t")]);
            Assert.Equal("recognized-source-to-unrecognized-target",Assert.Single(warnings).Status);
            var unknown=SpellingDiagnostics.Inspect(dictionary,CorrectionLanguage.Dutch,"Diorthosis retry_count start");
            Assert.Equal(2,unknown.Count(w=>!w.Recognized));
            Assert.Contains("untrusted",SpellingDiagnostics.HintPrompt(request,unknown));
            Assert.Contains("authoritative",SpellingDiagnostics.HintPrompt(request,unknown));
            using var cancellation=new CancellationTokenSource();cancellation.Cancel();
            Assert.Throws<OperationCanceledException>(()=>SpellingDiagnostics.Inspect(dictionary,CorrectionLanguage.Dutch,"start",cancellation.Token));
            dictionary.Dispose();
            Assert.Throws<ObjectDisposedException>(()=>dictionary.Check(CorrectionLanguage.Dutch,"start"));
            Assert.Throws<FileNotFoundException>(()=>new HunspellDiagnostics(Path.Combine(root,"missing.dic"),us,gb));
        }
        finally {Directory.Delete(root,true);}
    }
}
