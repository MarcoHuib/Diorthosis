namespace Diorthosis.Core.Corrections;

public enum CorrectionLanguage
{
    Dutch,
    English
}

public enum ErrorCategory
{
    Spelling,
    Grammar,
    SentenceStructure,
    Punctuation,
    CorrectText
}

public enum CorrectionDisposition
{
    Unchanged,
    Accepted,
    Suppressed,
    Rejected,
    NeedsReview
}

public sealed record CorrectionRequest
{
    public CorrectionRequest(CorrectionLanguage language, string text, IReadOnlyList<ProtectedSpan>? protectedSpans = null)
    {
        if (!Enum.IsDefined(language))
        {
            throw new ArgumentOutOfRangeException(nameof(language));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        if (text.Length > 32768) throw new ArgumentException("Correction input exceeds the 32768 UTF-16 unit safety limit.", nameof(text));
        SourceEdits.EnsureUnicode(text);
        ProtectedSpans = (protectedSpans ?? []).ToArray();
        SourceEdits.Validate(text, ProtectedSpans.Select(span => new TextEdit(span.Start, span.Length, span.OriginalText, span.OriginalText)), allowNoOps: true);
        if (ProtectedSpans.Any(span => span.Length == 0)) throw new ArgumentException("Protected spans must be nonempty.", nameof(protectedSpans));
        Language = language;
        Text = text;
    }

    public CorrectionLanguage Language { get; }
    public string Text { get; }
    public IReadOnlyList<ProtectedSpan> ProtectedSpans { get; }
}

public sealed record TextEdit
{
    public TextEdit(int start, int length, string originalText, string replacementText)
    {
        if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));
        if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
        ArgumentNullException.ThrowIfNull(originalText);
        ArgumentNullException.ThrowIfNull(replacementText);
        if (originalText.Length != length)
            throw new ArgumentException("The original text length must match the edit span length.", nameof(originalText));

        Start = start;
        Length = length;
        OriginalText = originalText;
        ReplacementText = replacementText;
    }

    public int Start { get; }
    public int Length { get; }
    public string OriginalText { get; }
    public string ReplacementText { get; }
}

public sealed record ProtectedSpan(int Start, int Length, string OriginalText);

public sealed record CorrectionResult(IReadOnlyList<TextEdit> Edits)
{
    public const string ContractVersion = "correction-result-2.0.0";
    public CorrectionDisposition Disposition { get; init; } = Edits.Count == 0 ? CorrectionDisposition.Unchanged : CorrectionDisposition.Accepted;
    // Diagnostic candidate data is never an authorization to apply it. Normal CLI output omits raw data.
    public ModelResponseAnalysis? Analysis { get; init; }
    public Diorthosis.Core.Inference.GenerationTermination? Termination { get; init; }
}

public sealed record InferenceRequest
{
    public InferenceRequest(string prompt, int maximumOutputTokens = 512, string? gbnfGrammar = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);
        if (maximumOutputTokens <= 0) throw new ArgumentOutOfRangeException(nameof(maximumOutputTokens));
        Prompt = prompt;
        MaximumOutputTokens = maximumOutputTokens;
        GbnfGrammar = gbnfGrammar;
    }

    public string Prompt { get; }
    public int MaximumOutputTokens { get; }
    public string? GbnfGrammar { get; }
}
