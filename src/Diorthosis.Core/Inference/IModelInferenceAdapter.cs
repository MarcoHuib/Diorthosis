using Diorthosis.Core.Corrections;

namespace Diorthosis.Core.Inference;

public interface IModelInferenceAdapter
{
    ValueTask<string> GenerateAsync(InferenceRequest request, CancellationToken cancellationToken = default);
}
