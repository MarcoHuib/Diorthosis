using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Diorthosis.Core.Corrections;

namespace Diorthosis.Benchmarks;

public sealed record AnnotationIssue(string CaseId, string Code, int? AnnotationIndex);
public sealed record AnnotationVariant(string Signature, IReadOnlyList<string> SourceIds, IReadOnlyList<ErrorAnnotation> Errors);
public sealed record DatasetGroup(string InputSha256, string Language, string InputText, int Occurrences,
    IReadOnlyList<string> CaseIds, IReadOnlyList<string> Categories, bool AnnotationsEquivalent,
    IReadOnlyList<AnnotationVariant> AnnotationVariants);
public sealed record NearDuplicate(string LeftSha256, string RightSha256, string Method, double Similarity);
public sealed record AuditLanguage(string Language, int Records, int UniqueInputs, int UniqueCleanInputs,
    IReadOnlyDictionary<string, int> UniqueInputsByCategory);
public sealed record DatasetAuditReport(string Version, int Records, int UniqueInputs, int DuplicateRecords,
    int DuplicateGroups, int RecordsInDuplicateGroups, int MaximumOccurrences, double SimilarityThreshold,
    IReadOnlyList<AuditLanguage> Languages, IReadOnlyList<DatasetGroup> Groups,
    IReadOnlyList<NearDuplicate> NearDuplicates, IReadOnlyList<AnnotationIssue> AnnotationIssues);
public sealed record SplitEntry(string Partition, BenchmarkCase Case, string? SourceFamily = null);
public sealed record SplitOverlap(string LeftPartition, string LeftId, string RightPartition, string RightId, string Reason, double? Similarity);

/// <summary>Development-data diagnostics; no linguistic disagreements are automatically adjudicated.</summary>
public static class DatasetAudit
{
    public const string Version = "dataset-audit-1.0.0";
    public static string Hash(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    public static DatasetAuditReport Analyze(IReadOnlyList<BenchmarkCase> cases, double threshold = 0.85)
    {
        CheckThreshold(threshold);
        ValidateIds(cases);
        var issues = cases.SelectMany(InspectAnnotations).ToArray();
        var groups = cases.GroupBy(item => (item.Language, item.InputText))
            .Select(group =>
            {
                var variants = group.GroupBy(item => Signature(item.Errors), StringComparer.Ordinal)
                    .OrderBy(variant => variant.Key, StringComparer.Ordinal)
                    .Select(variant => new AnnotationVariant(variant.Key, variant.Select(item => item.Id).Order(StringComparer.Ordinal).ToArray(),
                        variant.OrderBy(item => item.Id, StringComparer.Ordinal).First().Errors)).ToArray();
                return new DatasetGroup(Hash(group.Key.InputText), group.Key.Language.ToString(), group.Key.InputText, group.Count(),
                    group.Select(item => item.Id).Order(StringComparer.Ordinal).ToArray(),
                    group.SelectMany(item => item.Errors).Select(error => error.Category.ToString()).Distinct().Order(StringComparer.Ordinal).ToArray(),
                    variants.Length == 1, variants);
            }).OrderBy(group => group.Language, StringComparer.Ordinal).ThenBy(group => group.InputSha256, StringComparer.Ordinal).ToArray();
        var uniqueTexts = groups.Select(group => group.InputText).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray();
        if (uniqueTexts.Length > 5000) throw new ArgumentException("Audit pairwise comparison is limited to 5000 unique inputs.");
        var near = new List<NearDuplicate>();
        for (var i = 0; i < uniqueTexts.Length; i++)
            for (var j = i + 1; j < uniqueTexts.Length; j++)
            {
                var (method, score) = Compare(uniqueTexts[i], uniqueTexts[j]);
                if (score >= threshold) near.Add(new NearDuplicate(Hash(uniqueTexts[i]), Hash(uniqueTexts[j]), method, score));
            }
        var languages = Enum.GetValues<CorrectionLanguage>().Select(language =>
        {
            var selected = cases.Where(item => item.Language == language).ToArray();
            return new AuditLanguage(language.ToString(), selected.Length, selected.Select(item => item.InputText).Distinct(StringComparer.Ordinal).Count(),
                selected.Where(item => item.Errors.Count == 0).Select(item => item.InputText).Distinct(StringComparer.Ordinal).Count(),
                Enum.GetValues<ErrorCategory>().ToDictionary(category => category.ToString(), category => selected
                    .Where(item => category == ErrorCategory.CorrectText ? item.Errors.Count == 0 : item.Errors.Any(error => error.Category == category))
                    .Select(item => item.InputText).Distinct(StringComparer.Ordinal).Count()));
        }).ToArray();
        var exact = cases.GroupBy(item => item.InputText, StringComparer.Ordinal).ToArray();
        return new DatasetAuditReport(Version, cases.Count, exact.Length, cases.Count - exact.Length,
            exact.Count(group => group.Count() > 1), exact.Where(group => group.Count() > 1).Sum(group => group.Count()),
            exact.Select(group => group.Count()).DefaultIfEmpty(0).Max(), threshold, languages, groups,
            near.OrderBy(pair => pair.LeftSha256, StringComparer.Ordinal).ThenBy(pair => pair.RightSha256, StringComparer.Ordinal).ToArray(),
            issues.OrderBy(issue => issue.CaseId, StringComparer.Ordinal).ThenBy(issue => issue.AnnotationIndex).ThenBy(issue => issue.Code, StringComparer.Ordinal).ToArray());
    }

    public static IReadOnlyList<SplitOverlap> CheckSplits(IReadOnlyList<SplitEntry> entries, double threshold = 0.85)
    {
        CheckThreshold(threshold);
        if (entries.Count > 5000) throw new ArgumentException("Split comparison is limited to 5000 entries.");
        if (entries.Any(entry => string.IsNullOrWhiteSpace(entry.Partition))) throw new ArgumentException("Partition names must be nonempty.");
        foreach (var partition in entries.GroupBy(entry => entry.Partition, StringComparer.Ordinal)) ValidateIds(partition.Select(entry => entry.Case).ToArray());
        var result = new List<SplitOverlap>();
        var ordered = entries.OrderBy(entry => entry.Partition, StringComparer.Ordinal).ThenBy(entry => entry.Case.Id, StringComparer.Ordinal).ToArray();
        for (var i = 0; i < ordered.Length; i++)
            for (var j = i + 1; j < ordered.Length; j++)
            {
                var a = ordered[i]; var b = ordered[j];
                if (a.Partition == b.Partition) continue;
                if (a.Case.InputText == b.Case.InputText) Add("exact-text", 1);
                else
                {
                    var (method, score) = Compare(a.Case.InputText, b.Case.InputText);
                    if (score >= threshold) Add(method, score);
                }
                if (!string.IsNullOrWhiteSpace(a.SourceFamily) && a.SourceFamily == b.SourceFamily) Add("source-family", null);
                void Add(string reason, double? score) => result.Add(new SplitOverlap(a.Partition, a.Case.Id, b.Partition, b.Case.Id, reason, score));
            }
        return result;
    }

    public static (string Method, double Score) Compare(string first, string second)
    {
        static string Normalize(string text) => Regex.Replace(text.Normalize(NormalizationForm.FormC), @"\s+", " ").Trim();
        var a = Normalize(first); var b = Normalize(second);
        if (a == b) return ("NFC-whitespace", 1);
        if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) return ("case-folded-diagnostic", 1);
        static HashSet<string> Bigrams(string text)
        {
            var tokens = Regex.Matches(text.ToUpperInvariant(), @"[\p{L}\p{M}\p{N}_]+|[^\s]", RegexOptions.CultureInvariant).Select(match => match.Value).ToArray();
            return tokens.Length < 2 ? tokens.ToHashSet(StringComparer.Ordinal)
                : tokens.Zip(tokens.Skip(1), (left, right) => left + "\0" + right).ToHashSet(StringComparer.Ordinal);
        }
        var aa = Bigrams(a); var bb = Bigrams(b);
        return ("token-bigram-dice", aa.Count + bb.Count == 0 ? 0 : 2.0 * aa.Intersect(bb, StringComparer.Ordinal).Count() / (aa.Count + bb.Count));
    }

    public static IReadOnlyList<AnnotationIssue> InspectAnnotations(BenchmarkCase item)
    {
        var issues = new List<AnnotationIssue>();
        if (!Enum.IsDefined(item.Language)) issues.Add(new(item.Id, "unsupported-language", null));
        var boundaries = StringInfo.ParseCombiningCharacters(item.InputText).Append(item.InputText.Length).ToHashSet();
        for (var i = 0; i < item.Errors.Count; i++)
        {
            var error = item.Errors[i];
            if (!Enum.IsDefined(error.Category) || error.Category == ErrorCategory.CorrectText) issues.Add(new(item.Id, "invalid-error-category", i));
            if (error.Start < 0 || error.Length < 0 || error.Start > item.InputText.Length - error.Length
                || error.OriginalText.Length != error.Length
                || !item.InputText.AsSpan(error.Start, error.Length).SequenceEqual(error.OriginalText))
                issues.Add(new(item.Id, "source-alignment", i));
            else if (!boundaries.Contains(error.Start) || !boundaries.Contains(error.Start + error.Length)) issues.Add(new(item.Id, "grapheme-boundary", i));
            if (error.AcceptedCorrections.Count == 0) issues.Add(new(item.Id, "missing-alternatives", i));
            if (error.AcceptedCorrections.Any(correction => correction.ReplacementText == error.OriginalText)) issues.Add(new(item.Id, "no-op-alternative", i));
            for (var j = 0; j < i; j++)
            {
                var other = item.Errors[j];
                if ((long)error.Start < (long)other.Start + other.Length && (long)other.Start < (long)error.Start + error.Length
                    || error.Start == other.Start && (error.Length == 0 || other.Length == 0)) issues.Add(new(item.Id, "overlapping-annotations", i));
            }
        }
        return issues;
    }

    // Comparison retains literal apostrophes, punctuation, offsets and category; only annotation/alternative order is immaterial.
    private static string Signature(IReadOnlyList<ErrorAnnotation> errors) => Hash(JsonSerializer.Serialize(errors
        .OrderBy(error => error.Start).ThenBy(error => error.Length).ThenBy(error => error.Category)
        .Select(error => new { error.Start, error.Length, error.OriginalText, error.Category,
            alternatives = error.AcceptedCorrections.Select(correction => correction.ReplacementText).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray() })));
    private static void CheckThreshold(double threshold)
    {
        if (!double.IsFinite(threshold) || threshold <= 0 || threshold > 1) throw new ArgumentOutOfRangeException(nameof(threshold));
    }
    private static void ValidateIds(IReadOnlyList<BenchmarkCase> cases)
    {
        if (cases.Any(item => string.IsNullOrWhiteSpace(item.Id)) || cases.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count() != cases.Count)
            throw new ArgumentException("Dataset IDs must be nonempty and unique within a partition.");
    }
}
