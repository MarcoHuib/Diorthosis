using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;
using Xunit;

namespace Diorthosis.Tests;

public sealed class CorrectionPipelineTests
{
    [Fact]
    public void PromptPinsVersionLanguageAndEscapedInput()
    {
        var prompt = CorrectionPrompt.Build(new CorrectionRequest(CorrectionLanguage.Dutch, "regel\n\"tekst\""));
        Assert.Equal("diorthosis-correction-v3", CorrectionPrompt.Version);
        Assert.Contains("Dutch", prompt);
        Assert.Contains("\\n", prompt);
        Assert.Contains("Return only the complete corrected passage", prompt);
        Assert.DoesNotContain("Example input", prompt);
    }

    [Fact]
    public void ParsesUtf16PositionsAndChecksExactUnicodeSource()
    {
        const string source = "🌷 Ik heb het document verstuurt.";
        var offset = source.IndexOf("verstuurt", StringComparison.Ordinal);
        var json = $$"""[{"start":{{offset}},"length":9,"originalText":"verstuurt","replacementText":"verstuurd"}]""";
        var edits = ModelCorrectionEngine.ParseAndValidate(json, source);
        Assert.Equal(offset, edits[0].Start);
        Assert.Equal("verstuurt", edits[0].OriginalText);
    }

    [Fact]
    public void DerivesUtf16ProposalFromCorrectedPassageAndRemovesModelNote()
    {
        const string source = "🌷 Ik heb het document gisteren verstuurt.";
        const string output = "🌷 Ik heb het document gisteren **verstuurd**.\n*(Note: the participle is corrected.)*";
        var edits = ModelCorrectionEngine.ParseModelResponse(output, source);
        var offset = source.IndexOf("verstuurt", StringComparison.Ordinal);
        Assert.Single(edits);
        Assert.Equal(offset, edits[0].Start);
        Assert.Equal("verstuurt", edits[0].OriginalText);
        Assert.Equal("verstuurd", edits[0].ReplacementText);
    }

    [Fact]
    public void RemovesRepeatedOutputBlocksFromChattyModel()
    {
        const string source = "Ik heb het document gisteren verstuurt.";
        var edits = ModelCorrectionEngine.ParseModelResponse(
            "Ik heb het document gisteren verstuurd.\n\nOutput:\n\n\"Ik heb het document gisteren verstuurd.\"",
            source);
        Assert.Single(edits);
        Assert.Equal("verstuurd", edits[0].ReplacementText);
    }

    [Fact]
    public void UsesCompletePassageBeforeCorrectedTextAndExplanationMarkers()
    {
        const string source = "Please recieve the signed form.";
        var edits = ModelCorrectionEngine.ParseModelResponse(
            "Please receive the signed form.\n\nCorrected text: \"Please receive the signed form.\"\n\nExplanation: the typo was repaired.",
            source);
        Assert.Single(edits);
        Assert.Equal("receive", edits[0].ReplacementText);
    }

    [Fact]
    public void KeepsExactOrWhitespaceOnlyCorrectTextUnchanged()
    {
        const string source = "De server start opnieuw.";
        Assert.Empty(ModelCorrectionEngine.ParseModelResponse(source, source));
        Assert.Empty(ModelCorrectionEngine.ParseModelResponse("De   server start opnieuw.", source));
        Assert.Empty(ModelCorrectionEngine.ParseModelResponse("De server start opnieuw.\nDe server start opnieuw.", source));
    }

    [Fact]
    public void RejectsModelOutputThatDropsEmojiOrSymbol()
    {
        const string source = "🌷 Ik heb het document gisteren verstuurt.";
        Assert.Throws<InvalidDataException>(() => ModelCorrectionEngine.ParseModelResponse(
            "Ik heb het document gisteren verstuurd.", source));
    }

    [Theory]
    [InlineData("")]
    [InlineData("[]")]
    [InlineData("This is a completely unrelated explanation.")]
    [InlineData("She sent all the data to a different server.")]
    [InlineData("Ik heb het document gisteren verstuurd")]
    public void RejectsMalformedOrUnverifiableModelPassages(string output) =>
        Assert.Throws<InvalidDataException>(() => ModelCorrectionEngine.ParseModelResponse(output, "Ik heb het document gisteren verstuurt."));

    [Theory]
    [InlineData("[{\"start\":99,\"length\":1,\"originalText\":\"x\",\"replacementText\":\"y\"}]")]
    [InlineData("[{\"start\":0,\"length\":2,\"originalText\":\"zz\",\"replacementText\":\"xx\"}]")]
    [InlineData("[{\"start\":0,\"length\":1,\"originalText\":\"a\",\"replacementText\":\"b\"},{\"start\":0,\"length\":1,\"originalText\":\"a\",\"replacementText\":\"c\"}]")]
    [InlineData("[{\"start\":0,\"length\":1,\"originalText\":\"1\",\"replacementText\":\"2\"}]")]
    [InlineData("[{\"start\":0,\"length\":4,\"originalText\":\"2024\",\"replacementText\":\"2025\"}]")]
    [InlineData("[{\"start\":0,\"length\":14,\"originalText\":\"https://a.test\",\"replacementText\":\"https://b.test\"}]")]
    [InlineData("[{\"start\":0,\"length\":6,\"originalText\":\"AcmeID\",\"replacementText\":\"AcneID\"}]")]
    [InlineData("[{\"start\":0,\"length\":0,\"originalText\":\"\",\"replacementText\":\"x\"},{\"start\":0,\"length\":0,\"originalText\":\"\",\"replacementText\":\"y\"}]")]
    public void RejectsUnsafeEdits(string json) => Assert.Throws<InvalidDataException>(() => ModelCorrectionEngine.ParseAndValidate(json, "a 12"));

    [Fact]
    public async Task CorrectTextCanBePreservedAndCancellationFlowsToAdapter()
    {
        var source = "Correct text.";
        var adapter = new FakeAdapter(source);
        var engine = new ModelCorrectionEngine(adapter);
        var result = await engine.CorrectAsync(new CorrectionRequest(CorrectionLanguage.English, source));
        Assert.Empty(result.Edits);
        Assert.Equal(source, adapter.LastResponse);

        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var cancelAdapter = new FakeAdapter("", cancel: true);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await new ModelCorrectionEngine(cancelAdapter)
            .CorrectAsync(new CorrectionRequest(CorrectionLanguage.English, "Text."), cts.Token));
    }

    [Fact]
    public async Task UnverifiableInferenceOutputLeavesSourceUnchanged()
    {
        var engine = new ModelCorrectionEngine(new FakeAdapter(""));
        var result = await engine.CorrectAsync(new CorrectionRequest(CorrectionLanguage.Dutch, "Ik heb het document verstuurt."));
        Assert.Empty(result.Edits);
        Assert.Equal(1, engine.RejectedOutputCount);
    }

    [Fact]
    public void RejectsMissingModelFileWithoutLeakingContent()
    {
        var error = Assert.Throws<FileNotFoundException>(() => new LlamaSharpInferenceAdapter(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".gguf")));
        Assert.Contains("local GGUF", error.Message);
    }

    [Fact]
    public void ValidatesModelConfigurationBeforeNativeInitialization()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".gguf");
        File.WriteAllBytes(path, []);
        try
        {
            Assert.Throws<InvalidDataException>(() => new LlamaSharpInferenceAdapter(path));
            File.WriteAllBytes(path, "GGUF"u8.ToArray());
            Assert.Throws<ArgumentOutOfRangeException>(() => new LlamaSharpInferenceAdapter(path, contextSize: 128));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LlamaSharpInferenceAdapter(path, threads: 0));
            Assert.Throws<ArgumentException>(() => new LlamaSharpInferenceAdapter(path + ".txt"));
        }
        finally { File.Delete(path); }
    }

    private sealed class FakeAdapter(string response, bool cancel = false) : IModelInferenceAdapter
    {
        public string LastResponse { get; private set; } = string.Empty;
        public ValueTask<string> GenerateAsync(InferenceRequest request, CancellationToken cancellationToken = default)
        {
            LastResponse = response;
            if (cancel) cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(response);
        }
    }
}
