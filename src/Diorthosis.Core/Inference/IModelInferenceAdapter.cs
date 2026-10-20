using Diorthosis.Core.Corrections;

namespace Diorthosis.Core.Inference;

public enum GenerationTermination
{
    Unknown,
    EndOfGeneration,
    MaximumTokens,
    Cancelled
}

public sealed record InferenceResult(string Text, GenerationTermination Termination, int SampledTokens);

public interface IModelInferenceAdapter
{
    ValueTask<InferenceResult> GenerateAsync(InferenceRequest request, CancellationToken cancellationToken = default);
}
