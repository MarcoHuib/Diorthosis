using Diorthosis.Core.Corrections;
using Xunit;

namespace Diorthosis.Tests;

public sealed class CoreValidationTests
{
    [Fact]
    public void CorrectionRequestRequiresTextAndSupportedLanguage()
    {
        Assert.Throws<ArgumentException>(() => new CorrectionRequest(CorrectionLanguage.English, " "));
        Assert.Throws<ArgumentOutOfRangeException>(() => new CorrectionRequest((CorrectionLanguage)99, "text"));
    }

    [Fact]
    public void TextEditRequiresOriginalTextLengthToMatchSpan()
    {
        Assert.Throws<ArgumentException>(() => new TextEdit(0, 3, "ab", "cd"));
        Assert.Throws<ArgumentOutOfRangeException>(() => new TextEdit(-1, 0, "", "x"));
    }

    [Fact]
    public void InferenceRequestRequiresPositiveOutputLimit()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new InferenceRequest("prompt", 0));
    }
}
