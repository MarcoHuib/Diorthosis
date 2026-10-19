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
        Assert.Equal("diorthosis-correction-v1", CorrectionPrompt.Version);
        Assert.Contains("Dutch", prompt);
        Assert.Contains("\\n", prompt);
        Assert.Contains("UTF-16", prompt);
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
        var adapter = new FakeAdapter("[]");
        var engine = new ModelCorrectionEngine(adapter);
        var source = "Correct text.";
        var result = await engine.CorrectAsync(new CorrectionRequest(CorrectionLanguage.English, source));
        Assert.Empty(result.Edits);
        Assert.Equal("[]", adapter.LastResponse);

        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var cancelAdapter = new FakeAdapter("", cancel: true);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await new ModelCorrectionEngine(cancelAdapter)
            .CorrectAsync(new CorrectionRequest(CorrectionLanguage.English, "Text."), cts.Token));
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
