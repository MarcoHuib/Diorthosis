using Diorthosis.Core.Corrections;
using Xunit;
namespace Diorthosis.Tests;
public sealed class ProtectedSpanImmutabilityTests
{
    [Fact]
    public void CallerCannotMutateValidatedProtectionPolicy()
    {
        var supplied = new[] { new ProtectedSpan(0, 4, "Keep") };
        var request = new CorrectionRequest(CorrectionLanguage.English, "Keep this.", supplied);
        supplied[0] = new(5, 4, "this");
        Assert.Equal("Keep", request.ProtectedSpans[0].OriginalText);
        Assert.IsNotType<ProtectedSpan[]>(request.ProtectedSpans);
        var list = Assert.IsAssignableFrom<IList<ProtectedSpan>>(request.ProtectedSpans);
        Assert.Throws<NotSupportedException>(() => list[0] = new(5, 4, "this"));
    }
}
