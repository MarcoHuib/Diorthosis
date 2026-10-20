using Diorthosis.Core.Corrections;
using Xunit;

namespace Diorthosis.Tests;

public sealed class SegmentMappingTests
{
    [Fact]
    public void TargetMappingPreservesNeighboringContextAndOriginalSource()
    {
        const string source = "🌷 Context.\nPlease recieve it.\nKeep OAuth2 unchanged.";
        const string target = "Please recieve it.";
        var mapped = SegmentEditMapper.Map(source, source.IndexOf(target, StringComparison.Ordinal), target, [new(7, 7, "recieve", "receive")]);
        Assert.Equal("🌷 Context.\nPlease receive it.\nKeep OAuth2 unchanged.", SourceEdits.Apply(source, mapped));
        Assert.Contains("recieve", source);
    }
    [Fact]
    public void StaleTargetsCrossBoundaryEditsAndGraphemeSplitsAreRejected()
    {
        Assert.Throws<InvalidDataException>(() => SegmentEditMapper.Map("some text", 0, "stale", []));
        Assert.Throws<InvalidDataException>(() => SegmentEditMapper.Map("🌷 text", 1, "\udf37", []));
        Assert.Throws<InvalidDataException>(() => SegmentEditMapper.Map("prefix text suffix", 7, "text", [new(4, 1, " ", "x")]));
        Assert.Throws<InvalidDataException>(() => SegmentEditMapper.Map("geïnstalleerd", 0, "gei", []));
    }
}
