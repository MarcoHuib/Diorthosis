using Xunit;
using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;

namespace Diorthosis.Tests;

public sealed class ContextCorrectionTests
{
    [Theory]
    [InlineData(CorrectionLanguage.English, "Please recieve it.", "Please receive it.")]
    [InlineData(CorrectionLanguage.Dutch, "Dit is onmiddelijk nodig.", "Dit is onmiddellijk nodig.")]
    public async Task OnlyTargetMapsWithUtf16AndOpaqueMetadata(CorrectionLanguage language, string target, string candidate)
    {
        const string before = "🌷 Jose\u0301: Keep OAuth2 unchanged.";
        const string after = "Niet vertalen. Do not translate. 3 mei 2026.";
        var source = before + "\n" + target + "\n" + after;
        var metadata = new Dictionary<string, string> { ["structure"] = "table-cell", ["runs"] = "bold/hyperlink/footnote/track-change:opaque" };
        var segment = new ContextSegment("target", "paragraph-2", before.Length + 1, target, metadata);
        var request = new ContextCorrectionRequest("revision-1", source, language, segment,
            [new("before", "paragraph-1", 0, before, new Dictionary<string, string>()), new("after", "paragraph-3", before.Length + target.Length + 2, after, new Dictionary<string, string>())]);
        metadata["structure"] = "mutated outside snapshot";
        var adapter = new Fake(candidate);
        var result = await new ContextCorrectionEngine(adapter).CorrectAsync(request);
        Assert.Contains("read-only context", adapter.Prompt);
        Assert.Contains("sole editable target", adapter.Prompt);
        Assert.Equal("table-cell", result.FormattingMetadata["structure"]);
        Assert.Equal("paragraph-2", result.ParagraphId);
        var edits = ContextCorrectionEngine.MapToCurrentSnapshot(request, result, "revision-1", source);
        Assert.Equal(before + "\n" + candidate + "\n" + after, SourceEdits.Apply(source, edits));
        Assert.All(edits, edit => Assert.InRange(edit.Start, before.Length + 1, before.Length + 1 + target.Length));
        Assert.Throws<InvalidDataException>(() => ContextCorrectionEngine.MapToCurrentSnapshot(request, result, "revision-2", source));
        Assert.Throws<InvalidDataException>(() => ContextCorrectionEngine.MapToCurrentSnapshot(request, result with { TargetId = "different" }, "revision-1", source));
        Assert.Throws<InvalidDataException>(() => ContextCorrectionEngine.MapToCurrentSnapshot(request, result, "revision-1", source + " "));
    }
    [Fact]
    public void DuplicateStaleOverlappingAndGraphemeSplitSegmentsFail()
    {
        var empty = new Dictionary<string, string>(); var target = new ContextSegment("id", "p", 3, "text", empty);
        Assert.Throws<ArgumentException>(() => new ContextCorrectionRequest("r", "🌷 text", CorrectionLanguage.English, target, [target]));
        Assert.Throws<InvalidDataException>(() => new ContextCorrectionRequest("r", "🌷 text", CorrectionLanguage.English, target with { Start = 1 }, []));
        Assert.Throws<InvalidDataException>(() => new ContextCorrectionRequest("r", "🌷 text", CorrectionLanguage.English, target, [target with { SegmentId = "other" }]));
    }
    [Fact]
    public async Task ProtectedTargetAndCancelledGenerationProduceNoOutsideEdits()
    {
        var target = new ContextSegment("t", "p", 0, "The applicant is eligible.", new Dictionary<string, string>());
        var request = new ContextCorrectionRequest("r", target.Text, CorrectionLanguage.English, target, [], [new(17, 8, "eligible")]);
        var result = await new ContextCorrectionEngine(new Fake("The applicant is ineligible.")).CorrectAsync(request);
        Assert.Empty(result.DocumentEdits);
        Assert.Equal(CorrectionDisposition.Rejected, result.Correction.Disposition);
        var cancelled = await new ContextCorrectionEngine(new Fake("The applicant is eligible.", GenerationTermination.Cancelled)).CorrectAsync(request);
        Assert.Empty(cancelled.DocumentEdits);
        Assert.Equal(CorrectionDisposition.Rejected, cancelled.Correction.Disposition);
    }
    [Fact]
    public async Task ResultsCannotReplayAcrossProtectionLanguageOrMetadataPolicies()
    {
        var segment = new ContextSegment("t", "p", 0, "The applicant is eligible.", new Dictionary<string, string> { ["run"] = "bold" });
        var open = new ContextCorrectionRequest("r", segment.Text, CorrectionLanguage.English, segment, []);
        var result = await new ContextCorrectionEngine(new Fake("The applicant is ineligible.")).CorrectAsync(open);
        Assert.NotEmpty(result.DocumentEdits);
        var protectedRequest = new ContextCorrectionRequest("r", segment.Text, CorrectionLanguage.English, segment, [], [new(17, 8, "eligible")]);
        Assert.Throws<InvalidDataException>(() => ContextCorrectionEngine.MapToCurrentSnapshot(protectedRequest, result, "r", segment.Text));
        var italic = new ContextCorrectionRequest("r", segment.Text, CorrectionLanguage.English, segment with { FormattingMetadata = new Dictionary<string, string> { ["run"] = "italic" } }, []);
        Assert.Throws<InvalidDataException>(() => ContextCorrectionEngine.MapToCurrentSnapshot(italic, result, "r", segment.Text));
        var dutch = new ContextCorrectionRequest("r", segment.Text, CorrectionLanguage.Dutch, segment, []);
        Assert.Throws<InvalidDataException>(() => ContextCorrectionEngine.MapToCurrentSnapshot(dutch, result, "r", segment.Text));
        var rejected = result with { Correction = result.Correction with { Disposition = CorrectionDisposition.Rejected } };
        Assert.Throws<InvalidDataException>(() => ContextCorrectionEngine.MapToCurrentSnapshot(open, rejected, "r", segment.Text));
        var forged = result with { RequestSha256 = protectedRequest.RequestSha256 };
        Assert.Throws<InvalidDataException>(() => ContextCorrectionEngine.MapToCurrentSnapshot(protectedRequest, forged, "r", segment.Text));
    }
    [Theory]
    [InlineData(CorrectionLanguage.Dutch, "Dit verslag bevat een duidelijke uitleg.", "This report contains a clear explanation.")]
    [InlineData(CorrectionLanguage.English, "Please send the complete report today.", "Stuur vandaag het volledige verslag.")]
    public async Task WholeTargetTranslationIsNotDelivered(CorrectionLanguage language, string source, string translated)
    {
        var target = new ContextSegment("t", "p", 0, source, new Dictionary<string, string>());
        var request = new ContextCorrectionRequest("r", source, language, target, []);
        var result = await new ContextCorrectionEngine(new Fake(translated)).CorrectAsync(request);
        Assert.Empty(result.DocumentEdits);
        Assert.Equal(CorrectionDisposition.Rejected, result.Correction.Disposition);
    }
    private sealed class Fake(string candidate, GenerationTermination termination = GenerationTermination.EndOfGeneration) : IModelInferenceAdapter
    {
        public string Prompt { get; private set; } = "";
        public ValueTask<InferenceResult> GenerateAsync(InferenceRequest request, CancellationToken cancellationToken = default)
        { Prompt = request.Prompt; return ValueTask.FromResult(new InferenceResult(candidate, termination, 1)); }
    }
}
