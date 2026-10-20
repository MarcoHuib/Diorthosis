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
    public CorrectionRequest(CorrectionLanguage language, string text)
    {
        if (!Enum.IsDefined(language))
        {
            throw new ArgumentOutOfRangeException(nameof(language));
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(text);
        Language = language;
        Text = text;
    }

    public CorrectionLanguage Language { get; }
    public string Text { get; }
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

public sealed record CorrectionResult(IReadOnlyList<TextEdit> Edits);

public sealed record InferenceRequest
{
    public InferenceRequest(string prompt, int maximumOutputTokens = 512)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(prompt);
        if (maximumOutputTokens <= 0) throw new ArgumentOutOfRangeException(nameof(maximumOutputTokens));
        Prompt = prompt;
        MaximumOutputTokens = maximumOutputTokens;
    }

    public string Prompt { get; }
    public int MaximumOutputTokens { get; }
}
