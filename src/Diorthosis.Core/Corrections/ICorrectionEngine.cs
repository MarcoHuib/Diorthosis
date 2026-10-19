namespace Diorthosis.Core.Corrections;

public interface ICorrectionEngine
{
    ValueTask<CorrectionResult> CorrectAsync(CorrectionRequest request, CancellationToken cancellationToken = default);
}
