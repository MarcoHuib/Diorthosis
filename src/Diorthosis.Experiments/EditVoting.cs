using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;
using System.Text;

namespace Diorthosis.Experiments;

public sealed record LocalOperation(string Kind, TextEdit Edit);
public sealed record LocalEditGroup(string DependencyPolicy, IReadOnlyList<LocalOperation> Operations, bool AlignmentAmbiguous, bool IntroducesNonLatinLetters);
public sealed record VotingCandidate(string GenerationId, InferenceResult Inference);
public sealed record EditSupport(TextEdit Edit, IReadOnlyList<string> GenerationIds);
public sealed record VotingResult(ModelResponseAnalysis Analysis, IReadOnlyList<EditSupport> Support,
    int Threshold, int UniqueOutputs, int EligibleCandidates, string DiversityStatus);

/// <summary>Diagnostic token-oriented view over the existing grapheme/source diff; not a trained tagger.</summary>
public static class LocalEdits
{
    public const string Version = "source-local-operations-1.0.0";
    public static LocalEditGroup Derive(string source, string candidate)
    {
        var edits = SourceEdits.Validate(source, ModelCorrectionEngine.DeriveDiagnosticEdits(source, candidate));
        if (SourceEdits.Apply(source, edits) != candidate) throw new InvalidDataException("Alignment reconstruction failed.");
        var ambiguous = false;
        // Repeated whole-word deletion/replacement can yield the same final passage at different source spans.
        // Detect that observable ambiguity rather than pretending the canonical tie-break proves ownership.
        foreach (var edit in edits.Where(e => e.Length > 0 && e.OriginalText.Any(char.IsWhiteSpace) && e.OriginalText.Any(char.IsLetter)))
        {
            for (var start = source.IndexOf(edit.OriginalText, StringComparison.Ordinal); start >= 0;
                start = source.IndexOf(edit.OriginalText, start + 1, StringComparison.Ordinal))
            {
                if (start == edit.Start) continue;
                var alternate = edits.Select(e => ReferenceEquals(e, edit) ? new TextEdit(start, e.Length, e.OriginalText, e.ReplacementText) : e);
                try { if (SourceEdits.Apply(source, alternate) == candidate) ambiguous = true; }
                catch (InvalidDataException) { }
            }
        }
        var originalLetters = source.EnumerateRunes().Where(Rune.IsLetter).ToHashSet();
        // Dutch/English correction cannot justify newly invented foreign-script letters. Existing international
        // names/content remain literal; this flag is experimental and never normalizes or rewrites characters.
        var foreignLetters = candidate.EnumerateRunes().Any(r => Rune.IsLetter(r) && !originalLetters.Contains(r)
            && !(r.Value <= 0x02af || r.Value is >= 0x1e00 and <= 0x1eff || r.Value is >= 0xa720 and <= 0xa7ff || r.Value is >= 0xab30 and <= 0xab6f));
        return new("all passage edits remain dependency-linked; no grammatical independence inferred", edits.Select(edit =>
        {
            var kind = edit.Length == 0 ? "INSERT" : edit.ReplacementText.Length == 0 ? "DELETE"
                : edit.OriginalText.Any(char.IsWhiteSpace) && !edit.ReplacementText.Any(char.IsWhiteSpace) ? "MERGE"
                : !edit.OriginalText.Any(char.IsWhiteSpace) && edit.ReplacementText.Any(char.IsWhiteSpace) ? "SPLIT" : "REPLACE";
            return new LocalOperation(kind, edit);
        }).ToArray(), ambiguous, foreignLetters);
    }
}

public static class EditVoting
{
    public const string Version = "guarded-edit-voting-1.1.0";
    public static VotingResult Aggregate(CorrectionRequest request, IReadOnlyList<VotingCandidate> candidates)
    {
        if (candidates.Count is not (3 or 5) || candidates.Any(c => string.IsNullOrWhiteSpace(c.GenerationId))
            || candidates.Select(c => c.GenerationId).Distinct(StringComparer.Ordinal).Count() != candidates.Count)
            throw new ArgumentException("Voting requires three or five distinct generation IDs.");
        var threshold = candidates.Count / 2 + 1;
        var count = new Dictionary<TextEdit, HashSet<string>>();
        var eligible = new List<ModelResponseAnalysis>();
        foreach (var candidate in candidates)
        {
            var analysis = ModelCorrectionEngine.AnalyzeInference(candidate.Inference, request);
            if (analysis.Disposition is not (CorrectionDisposition.Accepted or CorrectionDisposition.Unchanged)) continue;
            var group = LocalEdits.Derive(request.Text, analysis.ExtractedCorrection!);
            if (group.AlignmentAmbiguous || group.IntroducesNonLatinLetters) continue;
            eligible.Add(analysis);
            foreach (var operation in group.Operations.Distinct())
            {
                if (!count.TryGetValue(operation.Edit, out var voters)) count[operation.Edit] = voters = new(StringComparer.Ordinal);
                voters.Add(candidate.GenerationId);
            }
        }
        var support = count.OrderBy(p => p.Key.Start).ThenBy(p => p.Key.Length).ThenBy(p => p.Key.ReplacementText, StringComparer.Ordinal)
            .Select(p => new EditSupport(p.Key, p.Value.Order(StringComparer.Ordinal).ToArray())).ToArray();
        var selected = support.Where(p => p.GenerationIds.Count >= threshold).Select(p => p.Edit).ToArray();
        var unique = candidates.Select(c => (c.Inference.Text, c.Inference.Termination)).Distinct().Count();
        var diversity = unique == 1 ? "identical outputs; no observed diversity or confidence gain" : "distinct outputs observed; same-model samples remain correlated";
        VotingResult Result(ModelResponseAnalysis analysis) => new(analysis, support, threshold, unique, eligible.Count, diversity);
        if (eligible.Count < threshold)
            return Result(ModelResponseAnalysis.Rejected("", null, [], "voting-coverage", "Insufficient complete validated candidates; threshold never shrinks after rejection."));
        string combined;
        try { combined = SourceEdits.Apply(request.Text, selected); }
        catch (InvalidDataException)
        {
            return Result(ModelResponseAnalysis.Rejected("", null, [], "voting-conflict", "Supported edits overlap or have ambiguous source positions."));
        }
        // No grammar analyzer can prove arbitrary subsets independent. Prevent unseen mosaics and partial repairs.
        // This deliberately sacrifices paper-style salvage; retain counts for diagnosing the lost opportunity.
        if (selected.Length > 0 && !eligible.Any(a => a.ExtractedCorrection == combined))
            return Result(ModelResponseAnalysis.NeedsReview(combined, combined, selected, "Voted subset/combination was never validated as a complete candidate; dependency independence is unknown."));
        return Result(ModelCorrectionEngine.AnalyzeLiteralCandidate(combined, request));
    }
}
