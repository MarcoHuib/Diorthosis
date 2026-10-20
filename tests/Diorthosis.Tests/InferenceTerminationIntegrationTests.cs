using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;
using Xunit;

namespace Diorthosis.Tests;

public sealed class InferenceTerminationIntegrationTests
{
    [LocalModelFact]
    public async Task StatelessExecutorStopsOnEogAndKeepsRequestsIsolated()
    {
        var modelPath = Environment.GetEnvironmentVariable("DIORTHOSIS_TEST_MODEL")!;
        using var adapter = new LlamaSharpInferenceAdapter(modelPath, contextSize: 4096, threads: 9);
        var engine = new ModelCorrectionEngine(adapter, maximumOutputTokens: 128);
        var cases = new[]
        {
            (CorrectionLanguage.English, "The informations are included in the report."),
            (CorrectionLanguage.English, "The user succesfully logged in."),
            (CorrectionLanguage.English, "They was waiting outside the office."),
            (CorrectionLanguage.Dutch, "Als hij morgen kom, beginnen we om negen uur.")
        };
        string? firstEnglishOutput = null;

        foreach (var (language, source) in cases)
        {
            var result = await adapter.GenerateAsync(new InferenceRequest(
                CorrectionPrompt.Build(new CorrectionRequest(language, source)), 128));
            var raw = result.Text;

            Assert.Equal(LlamaSharpInferenceAdapter.GenerationTermination.EndOfGeneration, adapter.LastTermination);
            Assert.InRange(adapter.LastSampledTokenCount, 1, 128);
            Assert.DoesNotContain("<s>", raw, StringComparison.Ordinal);
            Assert.DoesNotContain("</s>", raw, StringComparison.Ordinal);
            Assert.DoesNotContain("\n", raw, StringComparison.Ordinal);
            Assert.DoesNotContain("Explanation:", raw, StringComparison.OrdinalIgnoreCase);
            Assert.DoesNotContain("Alternative:", raw, StringComparison.OrdinalIgnoreCase);

            if (source == cases[0].Item2) firstEnglishOutput = raw;
        }

        var repeatedEnglish = await adapter.GenerateAsync(new InferenceRequest(
            CorrectionPrompt.Build(new CorrectionRequest(cases[0].Item1, cases[0].Item2)), 128));
        var repeatedEnglishText = repeatedEnglish.Text;
        Assert.Equal(LlamaSharpInferenceAdapter.GenerationTermination.EndOfGeneration, adapter.LastTermination);
        Assert.Equal(firstEnglishOutput, repeatedEnglishText);

        var limited = await adapter.GenerateAsync(new InferenceRequest("Write one word and stop.", 1));
        Assert.InRange(adapter.LastSampledTokenCount, 1, 1);
        Assert.NotNull(limited.Text);
        Assert.Contains(adapter.LastTermination, new[]
        {
            LlamaSharpInferenceAdapter.GenerationTermination.MaximumTokens,
            LlamaSharpInferenceAdapter.GenerationTermination.EndOfGeneration
        });

        using (var cancellation = new CancellationTokenSource(TimeSpan.FromMilliseconds(100)))
        {
            await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await adapter.GenerateAsync(
                new InferenceRequest(CorrectionPrompt.Build(new CorrectionRequest(cases[0].Item1, cases[0].Item2)), 128),
                cancellation.Token));
            Assert.Equal(LlamaSharpInferenceAdapter.GenerationTermination.Cancelled, adapter.LastTermination);
        }
        var afterCancellation = await adapter.GenerateAsync(new InferenceRequest(
            CorrectionPrompt.Build(new CorrectionRequest(cases[0].Item1, cases[0].Item2)), 128));
        Assert.Equal(LlamaSharpInferenceAdapter.GenerationTermination.EndOfGeneration, adapter.LastTermination);
        Assert.Equal(repeatedEnglishText, afterCancellation.Text);

        var correct = "De server is gisteren succesvol opnieuw opgestart.";
        var correctResult = await engine.CorrectAsync(new CorrectionRequest(CorrectionLanguage.Dutch, correct));
        Assert.Empty(correctResult.Edits);
        Assert.NotNull(repeatedEnglish);
    }
}

public sealed class LocalModelFactAttribute : FactAttribute
{
    public LocalModelFactAttribute()
    {
        var modelPath = Environment.GetEnvironmentVariable("DIORTHOSIS_TEST_MODEL");
        if (string.IsNullOrWhiteSpace(modelPath) || !File.Exists(modelPath))
            Skip = "Set DIORTHOSIS_TEST_MODEL to an existing local GGUF to run real-model inference tests.";
    }
}
