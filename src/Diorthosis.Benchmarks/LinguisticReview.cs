using System.Globalization;
using System.Text;
using System.Text.Json;
using Diorthosis.Core.Corrections;

namespace Diorthosis.Benchmarks;

public sealed record ReviewSubject(string SubjectId, string BindingSha256, string CaseId, CorrectionLanguage Language,
    string Layer, string Kind, string SourceText, string? CandidateText, string AnnotationsJson, string MetadataJson,
    string? SourceFamily, string? DocumentId, bool ProposedClean);
public sealed record LinguisticJudgment(bool? Justified, bool? MeaningPreserved, bool? AllDefectsRepaired,
    bool? NoNewErrors, bool? CleanPreserved, bool? CriticalSemanticFailure, bool? RejectionAppropriate,
    IReadOnlyList<string> AcceptedAlternatives, IReadOnlyList<string> ErrorCategories, string Ambiguity, string Rationale);
public sealed record ReviewDecision(string SubjectId, string BindingSha256, string ReviewerId, string ReviewerKind,
    CorrectionLanguage Language, string Qualifications, bool IndependenceDeclared, int Version, LinguisticJudgment Judgment);
public sealed record ReviewAdjudication(string SubjectId, string BindingSha256, string AdjudicatorId, string ReviewerKind,
    CorrectionLanguage Language, string Qualifications, bool IndependenceDeclared, int Version, string Status,
    IReadOnlyDictionary<string, int> ReviewerVersions, LinguisticJudgment Judgment);
public sealed record AdjudicatedLayerMetrics(string Language, string Layer, int Subjects, int UniqueSourceCases,
    int KnownSourceFamilies, int UnknownSourceFamilies, int? KnownDocuments, int ProposedCleanPassages,
    int Finalized, int Pending, int Disagreements, Proportion ProposalPrecision, Proportion CompletePassageSuccess,
    Proportion CleanPreservation, Proportion CriticalSemanticFailures, Proportion RejectionAppropriateness,
    Proportion AdjudicationCoverage);
public sealed record LinguisticReviewReport(string Version, string Policy, IReadOnlyList<AdjudicatedLayerMetrics> Metrics,
    IReadOnlyList<ReviewDecision> DecisionHistory, IReadOnlyList<ReviewAdjudication> AdjudicationHistory);

/// <summary>Imports external judgments without changing source predictions. Human roles are declared, not authenticated.</summary>
public static class LinguisticReview
{
    public const string Version = "linguistic-review-1.0.0";
    public static string Binding(ReviewSubject subject)
    {
        var fields = new[] { subject.CaseId, subject.Language.ToString(), subject.Layer, subject.Kind, subject.SourceText,
            subject.CandidateText ?? "", subject.AnnotationsJson, subject.MetadataJson, subject.SourceFamily ?? "", subject.DocumentId ?? "", subject.ProposedClean ? "true" : "false" };
        return DatasetAudit.Hash(string.Concat(fields.Select(value => Encoding.UTF8.GetByteCount(value).ToString(CultureInfo.InvariantCulture) + ":" + value)));
    }

    public static LinguisticReviewReport Evaluate(IReadOnlyList<ReviewSubject> subjects,
        IReadOnlyList<ReviewDecision> decisions, IReadOnlyList<ReviewAdjudication> adjudications)
    {
        if (subjects.Count == 0 || subjects.Select(item => item.SubjectId).Distinct(StringComparer.Ordinal).Count() != subjects.Count)
            throw new InvalidDataException("Review subjects must be nonempty and unique.");
        foreach (var item in subjects)
        {
            if (!Enum.IsDefined(item.Language) || item.BindingSha256 != Binding(item) || item.SubjectId != "review-" + item.BindingSha256
                || string.IsNullOrWhiteSpace(item.CaseId) || string.IsNullOrWhiteSpace(item.SourceText)
                || item.Layer is not ("annotations" or "raw" or "parsed" or "delivered" or "rejected")
                || item.Kind is not ("case" or "proposal" or "passage" or "rejection")) throw new InvalidDataException("Invalid or stale review subject.");
            SourceEdits.EnsureUnicode(item.SourceText);
            if (item.CandidateText is not null) SourceEdits.EnsureUnicode(item.CandidateText);
            using var annotations = JsonDocument.Parse(item.AnnotationsJson);
            using var metadata = JsonDocument.Parse(item.MetadataJson);
        }
        var map = subjects.ToDictionary(item => item.SubjectId, StringComparer.Ordinal);
        var seen = new HashSet<(string, string, int)>();
        foreach (var vote in decisions)
        {
            Validate(vote.SubjectId, vote.BindingSha256, vote.Language, vote.ReviewerId, vote.ReviewerKind, vote.Qualifications, vote.Version);
            if (!seen.Add((vote.SubjectId, Identity(vote.ReviewerId), vote.Version))) throw new InvalidDataException("Duplicate decision version.");
            ValidateJudgment(vote.Judgment);
        }
        var latest = decisions.GroupBy(vote => (vote.SubjectId, Id: Identity(vote.ReviewerId)))
            .ToDictionary(group => group.Key, group => group.MaxBy(vote => vote.Version)!);
        var finals = new Dictionary<string, ReviewAdjudication>(StringComparer.Ordinal);
        var adjudicationVersions = new HashSet<(string, int)>();
        foreach (var final in adjudications)
        {
            Validate(final.SubjectId, final.BindingSha256, final.Language, final.AdjudicatorId, final.ReviewerKind, final.Qualifications, final.Version);
            ValidateJudgment(final.Judgment);
            if (!adjudicationVersions.Add((final.SubjectId, final.Version)) || final.Status is not ("pending" or "disputed" or "approved"))
                throw new InvalidDataException("Invalid adjudication status/version.");
            if (final.Status == "approved")
            {
                if (final.ReviewerKind != "human-declared" || !final.IndependenceDeclared
                    || final.ReviewerVersions.Keys.Select(Identity).Distinct(StringComparer.Ordinal).Count() < 2
                    || final.ReviewerVersions.Keys.Any(id => Identity(id) == Identity(final.AdjudicatorId)))
                    throw new InvalidDataException("Approval requires a separate declared human adjudicator and two distinct reviewers.");
                foreach (var reviewer in final.ReviewerVersions.Keys)
                {
                    if (!latest.TryGetValue((final.SubjectId, Identity(reviewer)), out var vote)
                        || vote.Version != final.ReviewerVersions[reviewer] || vote.ReviewerKind != "human-declared" || !vote.IndependenceDeclared)
                        throw new InvalidDataException("Approval lacks two independent declared human reviews for this bound subject.");
                }
            }
            if (!finals.TryGetValue(final.SubjectId, out var prior) || prior.Version < final.Version) finals[final.SubjectId] = final;
        }
        var metrics = subjects.GroupBy(item => (item.Language, item.Layer)).OrderBy(group => group.Key.Language).ThenBy(group => group.Key.Layer, StringComparer.Ordinal).Select(group =>
        {
            var approved = group.Where(item => finals.TryGetValue(item.SubjectId, out var final) && final.Status == "approved")
                .Select(item => (item, judgment: finals[item.SubjectId].Judgment)).ToArray();
            Proportion Measure(string kind, Func<LinguisticJudgment, bool?> select, bool cleanOnly = false)
            {
                var judged = approved.Where(pair => pair.item.Kind == kind && (!cleanOnly || pair.item.ProposedClean)).Select(pair => select(pair.judgment)).Where(value => value is not null).ToArray();
                return Ratio(judged.Count(value => value == true), judged.Length);
            }
            var disagreements = group.Count(item => latest.Values.Where(vote => vote.SubjectId == item.SubjectId)
                .Select(vote => JsonSerializer.Serialize(vote.Judgment with { Rationale = "" })).Distinct(StringComparer.Ordinal).Count() > 1);
            return new AdjudicatedLayerMetrics(group.Key.Language.ToString(), group.Key.Layer, group.Count(), group.Select(item => item.CaseId).Distinct(StringComparer.Ordinal).Count(),
                group.Where(item => item.SourceFamily is not null).Select(item => item.SourceFamily).Distinct(StringComparer.Ordinal).Count(),
                group.Where(item => item.SourceFamily is null).Select(item => item.CaseId).Distinct(StringComparer.Ordinal).Count(),
                group.All(item => item.DocumentId is null) ? null : group.Where(item => item.DocumentId is not null).Select(item => item.DocumentId).Distinct(StringComparer.Ordinal).Count(),
                group.Count(item => item.Kind == "passage" && item.ProposedClean), approved.Length, group.Count() - approved.Length, disagreements,
                Measure("proposal", value => value.Justified is null || value.MeaningPreserved is null ? null : value.Justified == true && value.MeaningPreserved == true),
                Measure("passage", value => value.AllDefectsRepaired is null || value.NoNewErrors is null || value.MeaningPreserved is null || value.Justified is null ? null
                    : value.AllDefectsRepaired == true && value.NoNewErrors == true && value.MeaningPreserved == true && value.Justified == true),
                Measure("passage", value => value.CleanPreserved, cleanOnly: true), Measure("passage", value => value.CriticalSemanticFailure),
                Measure("rejection", value => value.RejectionAppropriate), Ratio(approved.Length, group.Count()));
        }).ToArray();
        return new(Version, "Only approved, content-bound external human-declared judgments enter linguistic metrics. Identities/qualifications/independence are declarations requiring external verification. Unknowns excluded with coverage; descriptive Wilson intervals do not establish independence. Cases/families/documents are counts, not inferred independent observations. AI decisions remain recorded and cannot approve. Existing predictions are unchanged.", metrics, decisions, adjudications);

        void Validate(string id, string hash, CorrectionLanguage language, string reviewer, string kind, string qualifications, int version)
        {
            if (!map.TryGetValue(id, out var item) || item.BindingSha256 != hash || item.Language != language || version < 1
                || string.IsNullOrWhiteSpace(reviewer) || string.IsNullOrWhiteSpace(qualifications) || kind is not ("human-declared" or "ai"))
                throw new InvalidDataException("Stale subject binding or invalid reviewer metadata.");
        }
    }
    private static string Identity(string value) => value.Trim().ToUpperInvariant();
    private static void ValidateJudgment(LinguisticJudgment value)
    {
        if (value is null || value.AcceptedAlternatives is null || value.ErrorCategories is null
            || string.IsNullOrWhiteSpace(value.Rationale) || value.Ambiguity is null) throw new InvalidDataException("Review needs rationale, alternatives, categories and ambiguity fields.");
        if (value.CriticalSemanticFailure == true && value.MeaningPreserved == true) throw new InvalidDataException("Contradictory semantic judgments.");
    }
    private static Proportion Ratio(int numerator, int denominator)
    {
        if (denominator == 0) return new(numerator, denominator, null, null, null);
        var p = (double)numerator / denominator; const double z = 1.959963984540054;
        var scale = 1 + z * z / denominator;
        var center = (p + z * z / (2 * denominator)) / scale;
        var margin = z * Math.Sqrt(p * (1 - p) / denominator + z * z / (4.0 * denominator * denominator)) / scale;
        return new(numerator, denominator, p, Math.Max(0, center - margin), Math.Min(1, center + margin));
    }
}
