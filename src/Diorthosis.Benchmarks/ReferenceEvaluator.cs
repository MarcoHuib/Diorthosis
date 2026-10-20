using System.Text;
using System.Text.Json;
using Diorthosis.Core.Corrections;

namespace Diorthosis.Benchmarks;

public sealed record Proportion(int Numerator, int Denominator, double? Estimate, double? WilsonLower, double? WilsonUpper);
public sealed record ReferenceProposal(string Id, IReadOnlyList<int> AnnotationIndexes, bool ReferenceMatched, IReadOnlyList<TextEdit> Edits);
public sealed record ReferenceCaseResult(string CaseId, string CandidateSha256, bool PredictionAvailable, bool? ReferenceCompletePassage,
    IReadOnlyList<int> RepairedAnnotations, IReadOnlyList<ReferenceProposal> Proposals, int ReferenceUnresolvedGroups,
    CorrectionDisposition? Disposition, string SemanticAssessment);
public sealed record ProposalReview(string CaseId, string CandidateSha256, string ProposalId, bool NecessaryAndCorrect, bool MeaningPreserved);
public sealed record ReferenceMetrics(string Language, string Category, Proportion RepairRecall, Proportion ReferenceGroupPrecision,
    Proportion? AdjudicatedProposalPrecision, Proportion CleanLiteralPreservation, Proportion CompleteReferencePassages,
    Proportion PredictionCoverage, int ReferenceUnresolvedGroups, int ProposedGroups, int ReviewedGroups,
    IReadOnlyDictionary<string, int> Dispositions)
{
    public Proportion? CompletePassageJudgmentCoverage { get; init; }
    public int UnknownCompletePassages { get; init; }
}
public sealed record ReferenceReport(string EvaluatorVersion, string PrecisionPolicy, string IntervalPolicy,
    IReadOnlyList<ReferenceMetrics> Metrics, IReadOnlyList<ReferenceCaseResult> Cases);

/// <summary>Reference-based diagnostic scoring, distinct from independent linguistic adjudication.</summary>
public static class ReferenceEvaluator
{
    public const string Version = "reference-groups-3.1.0";
    private static bool Equal(string a, string b) => a.Normalize(NormalizationForm.FormC) == b.Normalize(NormalizationForm.FormC);

    public static ReferenceReport Evaluate(IReadOnlyList<BenchmarkCase> cases, IReadOnlyList<BenchmarkPrediction> predictions,
        IReadOnlyDictionary<string, CorrectionDisposition>? dispositions = null, IReadOnlyList<ProposalReview>? reviews = null)
    {
        if (cases.Select(item => item.Id).Distinct(StringComparer.Ordinal).Count() != cases.Count
            || predictions.Select(item => item.CaseId).Distinct(StringComparer.Ordinal).Count() != predictions.Count)
            throw new ArgumentException("Case and prediction IDs must be unique.");
        var ids = cases.Select(item => item.Id).ToHashSet(StringComparer.Ordinal);
        if (predictions.Any(item => !ids.Contains(item.CaseId)) || dispositions?.Keys.Any(id => !ids.Contains(id)) == true)
            throw new ArgumentException("Unknown prediction/disposition case ID.");
        var predictionMap = predictions.ToDictionary(item => item.CaseId, StringComparer.Ordinal);
        var rows = cases.Select(item => Score(item, predictionMap.GetValueOrDefault(item.Id),
            dispositions is not null && dispositions.TryGetValue(item.Id, out var disposition) ? disposition : null)).ToArray();
        var reviewMap = (reviews ?? []).ToDictionary(review => (review.CaseId, review.ProposalId));
        foreach (var review in reviewMap.Values)
        {
            var row = rows.SingleOrDefault(row => row.CaseId == review.CaseId);
            if (row is null || row.CandidateSha256 != review.CandidateSha256 || !row.Proposals.Any(proposal => proposal.Id == review.ProposalId))
                throw new ArgumentException("Review does not match this candidate and proposal.");
        }
        var metrics = new List<ReferenceMetrics>();
        foreach (var language in Enum.GetValues<CorrectionLanguage>())
            foreach (var category in new[] { "all" }.Concat(Enum.GetValues<ErrorCategory>().Where(value => value != ErrorCategory.CorrectText).Select(value => value.ToString())))
            {
                var selected = cases.Select((item, index) => (item, row: rows[index])).Where(pair => pair.item.Language == language).ToArray();
                var errors = selected.SelectMany(pair => pair.item.Errors.Select((error, index) => (pair.row, error, index)))
                    .Where(value => category == "all" || value.error.Category.ToString() == category).ToArray();
                var groups = selected.SelectMany(pair => pair.row.Proposals.Select(proposal => (pair.row, pair.item, proposal)))
                    // Category proposal accounting partitions mixed groups into a single explicit Mixed bucket: excluded from category precision rows.
                    .Where(value => category == "all" || value.proposal.AnnotationIndexes.Count > 0
                        && value.proposal.AnnotationIndexes.All(index => value.item.Errors[index].Category.ToString() == category)).ToArray();
                var reviewed = groups.Where(value => reviewMap.ContainsKey((value.row.CaseId, value.proposal.Id))).ToArray();
                var clean = selected.Where(pair => pair.item.Errors.Count == 0 && pair.row.PredictionAvailable).ToArray();
                var observed = selected.Where(pair => pair.row.PredictionAvailable).ToArray();
                metrics.Add(new ReferenceMetrics(language.ToString(), category,
                    Ratio(errors.Count(value => value.row.RepairedAnnotations.Contains(value.index)), errors.Length),
                    Ratio(groups.Count(value => value.proposal.ReferenceMatched), groups.Length),
                    reviewed.Length == 0 ? null : Ratio(reviewed.Count(value => reviewMap[(value.row.CaseId, value.proposal.Id)] is { NecessaryAndCorrect: true, MeaningPreserved: true }), reviewed.Length),
                    Ratio(clean.Count(pair => pair.row.Proposals.Count == 0 && pair.row.CandidateSha256 == DatasetAudit.Hash(pair.item.InputText)), clean.Length),
                    Ratio(observed.Count(pair => pair.row.ReferenceCompletePassage == true), observed.Count(pair => pair.row.ReferenceCompletePassage is not null)),
                    Ratio(observed.Length, selected.Length), groups.Count(value => !value.proposal.ReferenceMatched), groups.Length, reviewed.Length,
                    selected.GroupBy(pair => pair.row.Disposition?.ToString() ?? "Unknown", StringComparer.Ordinal).OrderBy(group => group.Key, StringComparer.Ordinal).ToDictionary(group => group.Key, group => group.Count()))
                {
                    CompletePassageJudgmentCoverage = Ratio(observed.Count(pair => pair.row.ReferenceCompletePassage is not null), observed.Length),
                    UnknownCompletePassages = observed.Count(pair => pair.row.ReferenceCompletePassage is null)
                });
            }
        return new ReferenceReport(Version, "Reference-group precision is a proxy. Linguistic precision is available only for explicitly adjudicated proposals; review coverage is reported. Residual groups require review.",
            "Wilson 95% intervals are descriptive only; dependence/template repetition and unreviewed labels prevent population inference. No hunk-based independence or model-superiority test is assumed.", metrics, rows);
    }

    private static ReferenceCaseResult Score(BenchmarkCase item, BenchmarkPrediction? prediction, CorrectionDisposition? disposition)
    {
        if (string.IsNullOrWhiteSpace(item.Id) || string.IsNullOrWhiteSpace(item.InputText) || DatasetAudit.InspectAnnotations(item).Count != 0)
            throw new ArgumentException("Invalid benchmark annotations; run dataset audit.");
        SourceEdits.EnsureUnicode(item.InputText);
        foreach (var error in item.Errors) foreach (var accepted in error.AcceptedCorrections) SourceEdits.EnsureUnicode(accepted.ReplacementText);
        if (prediction is null) return new(item.Id, DatasetAudit.Hash(item.InputText), false, null, [], [], 0, disposition, "unassessed");
        var candidate = SourceEdits.Apply(item.InputText, prediction.Edits);
        // Canonicalize the *comparison*, leaving both source and delivered output untouched.
        var canonicalEdits = ModelCorrectionEngine.DeriveDiagnosticEdits(item.InputText, candidate, canonicalEquivalence: true);
        var used = new HashSet<int>();
        var repaired = new HashSet<int>();
        var proposals = new List<ReferenceProposal>();
        var initial = item.Errors.Select((error, index) => (error, index))
            .GroupBy(value => value.error.DependencyGroup is { Length: > 0 } dependency ? "dependency:" + dependency : "annotation:" + value.index)
            .Select(group => group.Select(value => value.index).ToArray()).ToArray();
        var parents = Enumerable.Range(0, initial.Length).ToArray();
        int Root(int index) { while (parents[index] != index) { parents[index] = parents[parents[index]]; index = parents[index]; } return index; }
        foreach (var edit in canonicalEdits)
        {
            var touching = Enumerable.Range(0, initial.Length).Where(index => initial[index].Any(annotation => Touches(item.Errors[annotation], edit))).ToArray();
            foreach (var index in touching.Skip(1)) parents[Root(index)] = Root(touching[0]);
        }
        var groups = Enumerable.Range(0, initial.Length).GroupBy(Root).Select(group => group.SelectMany(index => initial[index]).Order().ToArray());
        foreach (var indexes in groups)
        {
            var related = Enumerable.Range(0, canonicalEdits.Count).Where(index => indexes.Any(annotation => Touches(item.Errors[annotation], canonicalEdits[index]))).ToArray();
            if (related.Length == 0) continue;
            var localCandidate = SourceEdits.Apply(item.InputText, related.Select(index => canonicalEdits[index]));
            var references = BuildReferences(item, indexes);
            var matched = references is not null && references.Any(reference => Equal(reference, localCandidate));
            foreach (var index in related) used.Add(index);
            if (matched) foreach (var index in indexes) repaired.Add(index);
            var proposalEdits = related.Select(index => canonicalEdits[index]).ToArray();
            proposals.Add(new(ProposalId(item, indexes, proposalEdits), indexes, matched, proposalEdits));
        }
        // A compound canonical edit can touch several annotations. Full-group matching resolves these consistently.
        var allReferences = BuildReferences(item, Enumerable.Range(0, item.Errors.Count).ToArray());
        var complete = allReferences is null ? (bool?)null : allReferences.Any(reference => Equal(reference, candidate));
        if (item.Errors.Count == 0) complete = candidate == item.InputText;
        if (complete == true && item.Errors.Count > 0)
        {
            repaired.UnionWith(Enumerable.Range(0, item.Errors.Count));
            proposals = proposals.Select(proposal => proposal with { ReferenceMatched = true }).ToList();
        }
        for (var index = 0; index < canonicalEdits.Count; index++)
            if (!used.Contains(index)) proposals.Add(new(ProposalId(item, [], [canonicalEdits[index]]), [], false, [canonicalEdits[index]]));
        return new(item.Id, DatasetAudit.Hash(candidate), true, complete, repaired.Order().ToArray(), proposals,
            proposals.Count(proposal => !proposal.ReferenceMatched), disposition, "unassessed; deterministic reference matching is not semantic validation");
    }

    private static string ProposalId(BenchmarkCase item, IReadOnlyList<int> members, IReadOnlyList<TextEdit> edits)
    {
        static object Annotation(ErrorAnnotation error) => new { error.Start, error.Length, error.OriginalText, error.Category, error.DependencyGroup,
            alternatives = error.AcceptedCorrections.Select(correction => correction.ReplacementText).Distinct(StringComparer.Ordinal).Order(StringComparer.Ordinal).ToArray() };
        var ordered = item.Errors.Select(Annotation).Select(value => JsonSerializer.Serialize(value)).Order(StringComparer.Ordinal).ToArray();
        var group = members.Select(index => JsonSerializer.Serialize(Annotation(item.Errors[index]))).Order(StringComparer.Ordinal).ToArray();
        return "proposal:" + DatasetAudit.Hash(JsonSerializer.Serialize(new { item.Id, item.Language, item.InputText, annotations = ordered, group, edits }));
    }

    private static IReadOnlyList<string>? BuildReferences(BenchmarkCase item, IReadOnlyList<int> indexes)
    {
        var variants = new List<List<TextEdit>> { new() };
        foreach (var index in indexes)
        {
            var error = item.Errors[index];
            if (error.AcceptedCorrections.Count > 32 || (long)variants.Count * error.AcceptedCorrections.Count > 256) return null;
            variants = variants.SelectMany(edits => error.AcceptedCorrections.Select(correction => new List<TextEdit>(edits)
                { new(error.Start, error.Length, error.OriginalText, correction.ReplacementText) })).ToList();
        }
        return variants.Select(edits => SourceEdits.Apply(item.InputText, edits)).ToArray();
    }

    private static bool Touches(ErrorAnnotation error, TextEdit edit) => edit.Length == 0
        ? edit.Start >= error.Start && edit.Start <= error.Start + error.Length
        : error.Length == 0 ? error.Start >= edit.Start && error.Start <= edit.Start + edit.Length
        : edit.Start < error.Start + error.Length && edit.Start + edit.Length > error.Start;

    public static Proportion Ratio(int numerator, int denominator)
    {
        if (numerator < 0 || denominator < numerator) throw new ArgumentOutOfRangeException(nameof(numerator));
        if (denominator == 0) return new(0, 0, null, null, null);
        const double z = 1.959963984540054;
        var p = (double)numerator / denominator;
        var factor = 1 + z * z / denominator;
        var center = (p + z * z / (2 * denominator)) / factor;
        var half = z * Math.Sqrt((p * (1 - p) + z * z / (4 * denominator)) / denominator) / factor;
        return new(numerator, denominator, p, Math.Max(0, center - half), Math.Min(1, center + half));
    }
}
