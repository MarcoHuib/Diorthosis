using System.Text.Json;
using Diorthosis.Benchmarks;
using Diorthosis.Core.Corrections;
using Xunit;

namespace Diorthosis.Tests;

public sealed class DatasetAuditTests
{
    private static BenchmarkCase Case(string id, string input, params ErrorAnnotation[] errors) => new(id, CorrectionLanguage.English, input, errors);
    private static ErrorAnnotation Error(int start, string original, string replacement) => new(start, original.Length, original, ErrorCategory.Spelling, [new(replacement)]);

    [Fact]
    public void ExactAuditPreservesConflictingAnnotationsAndEveryIdDeterministically()
    {
        var cases = new[] { Case("b", "teh.", Error(0, "teh", "the")), Case("a", "teh.", Error(0, "teh", "the")), Case("c", "teh.") };
        var report = DatasetAudit.Analyze(cases);
        Assert.Equal(2, report.DuplicateRecords);
        var group = Assert.Single(report.Groups);
        Assert.False(group.AnnotationsEquivalent);
        Assert.Equal(2, group.AnnotationVariants.Count);
        Assert.Equal(new[] { "a", "b", "c" }, group.CaseIds);
        Assert.Equal(JsonSerializer.Serialize(report), JsonSerializer.Serialize(DatasetAudit.Analyze(cases.Reverse().ToArray())));
    }

    [Fact]
    public void AlternativeOrderDoesNotCreateAConflictButLiteralApostrophesDo()
    {
        var annotation = new ErrorAnnotation(0, 4, "lets", ErrorCategory.Punctuation, [new("let's"), new("let’s")]);
        var cases = new[] { Case("a", "lets", annotation), Case("b", "lets", annotation with { AcceptedCorrections = annotation.AcceptedCorrections.Reverse().ToArray() }) };
        Assert.True(Assert.Single(DatasetAudit.Analyze(cases).Groups).AnnotationsEquivalent);
        Assert.False(Assert.Single(DatasetAudit.Analyze([cases[0], cases[1] with { Errors = [annotation with { AcceptedCorrections = [new("let's")] }] }]).Groups).AnnotationsEquivalent);
    }

    [Theory]
    [InlineData("De geïnstalleerde server.", "De geïnstalleerde server.", "NFC-whitespace")]
    [InlineData("Some text.", "Some   text.", "NFC-whitespace")]
    [InlineData("Some text.", "some text.", "case-folded-diagnostic")]
    public void NormalizedMatchesRemainDiagnostics(string a, string b, string method)
    {
        var report = DatasetAudit.Analyze([Case("a", a), Case("b", b)]);
        Assert.Equal(2, report.UniqueInputs);
        Assert.Equal(method, Assert.Single(report.NearDuplicates).Method);
    }

    [Fact]
    public void ThresholdIsConfigurableAndMeaningChangesAreNotCollapsed()
    {
        var cases = new[] { Case("a", "The service is available on this computer today."), Case("b", "The service is not available on this computer today.") };
        Assert.NotEmpty(DatasetAudit.Analyze(cases, 0.5).NearDuplicates);
        Assert.Empty(DatasetAudit.Analyze(cases, 1).NearDuplicates);
        Assert.Equal(2, DatasetAudit.Analyze(cases).Groups.Count);
        Assert.Throws<ArgumentOutOfRangeException>(() => DatasetAudit.Analyze(cases, double.NaN));
    }

    [Fact]
    public void SplitCheckerDetectsCrossLanguageTextNearMatchesAndDeclaredFamilies()
    {
        var entries = new[] { new SplitEntry("development", Case("a", "Some text."), "template-1"),
            new SplitEntry("held-out", Case("b", "Some text.") with { Language = CorrectionLanguage.Dutch }),
            new SplitEntry("selection", Case("c", "Some  text.")), new SplitEntry("held-out", Case("d", "Completely different."), "template-1") };
        var overlaps = DatasetAudit.CheckSplits(entries);
        Assert.Contains(overlaps, overlap => overlap.Reason == "exact-text");
        Assert.Contains(overlaps, overlap => overlap.Reason == "NFC-whitespace");
        Assert.Contains(overlaps, overlap => overlap.Reason == "source-family");
    }

    [Fact]
    public void AnnotationAuditReportsMisalignmentOverlapAndGraphemeSplitting()
    {
        var item = Case("a", "🌷 geïnstalleerd", Error(1, "\udf37", "x"), Error(3, "x", "y"), Error(3, "ge", "xx"), Error(3, "ge", "zz"));
        var issues = DatasetAudit.InspectAnnotations(item);
        Assert.Contains(issues, issue => issue.Code == "grapheme-boundary");
        Assert.Contains(issues, issue => issue.Code == "source-alignment");
        Assert.Contains(issues, issue => issue.Code == "overlapping-annotations");
    }

    [Fact]
    public void InsertionsAndEmptyDatasetAreSupportedButDuplicateIdsAreRejected()
    {
        Assert.Empty(DatasetAudit.InspectAnnotations(Case("a", "text", Error(0, "", "A "))));
        Assert.Equal(0, DatasetAudit.Analyze([]).MaximumOccurrences);
        Assert.Throws<ArgumentException>(() => DatasetAudit.Analyze([Case("a", "one"), Case("a", "two")]));
    }
}
