using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using Diorthosis.Core.Inference;

namespace Diorthosis.Core.Corrections;

public sealed class ModelCorrectionEngine(IModelInferenceAdapter adapter, int maximumOutputTokens = 512) : ICorrectionEngine
{
    private static readonly Regex UrlToken = new(@"(?:https?://|www\.)[^\s<>]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex NumberToken = new(@"\d+(?:[.,:/-]\d+)*", RegexOptions.CultureInvariant);
    private static readonly Regex ProtectedIdentifier = new(@"\b(?:[A-Za-z0-9]*[A-Z][a-z]+[A-Za-z0-9]*|[A-Z]{2,}[A-Z0-9]*|[A-Za-z0-9]*\d+[A-Za-z0-9]*|[A-Za-z0-9]+(?:_[A-Za-z0-9]+)+)\b", RegexOptions.CultureInvariant);
    private static readonly Regex CapitalizedWord = new(@"\b\p{Lu}[\p{L}'’.-]*\b", RegexOptions.CultureInvariant);
    private static readonly Regex WordToken = new(@"[\p{L}\p{N}_]+", RegexOptions.CultureInvariant);
    private readonly IModelInferenceAdapter _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
    public int MaximumOutputTokens => maximumOutputTokens;
    private int _rejectedOutputCount;
    public int RejectedOutputCount => Volatile.Read(ref _rejectedOutputCount);

    public async ValueTask<CorrectionResult> CorrectAsync(CorrectionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (maximumOutputTokens <= 0) throw new ArgumentOutOfRangeException(nameof(maximumOutputTokens));
        var raw = await _adapter.GenerateAsync(new InferenceRequest(CorrectionPrompt.Build(request), maximumOutputTokens), cancellationToken).ConfigureAwait(false);
        try
        {
            return new CorrectionResult(ParseModelResponse(raw, request.Text));
        }
        catch (InvalidDataException)
        {
            Interlocked.Increment(ref _rejectedOutputCount);
            return new CorrectionResult(Array.Empty<TextEdit>());
        }
    }

    /// <summary>Extracts a conservative corrected passage and derives a UTF-16 edit from the source diff.</summary>
    public static IReadOnlyList<TextEdit> ParseModelResponse(string output, string source)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(source);
        var candidate = ExtractCandidate(output, source);
        if (candidate == source || SameNonWhitespaceTokens(candidate, source))
            return Array.Empty<TextEdit>();
        if (HasSentenceEnding(source) && !HasSentenceEnding(candidate))
            throw new InvalidDataException("Correction output appears incomplete at the end of the passage.");
        if (!SameSymbols(source, candidate))
            throw new InvalidDataException("Correction output changes a symbol or emoji from the source.");
        EnsurePlausibleCorrection(candidate, source);

        var prefix = 0;
        while (prefix < source.Length && prefix < candidate.Length && source[prefix] == candidate[prefix])
            prefix++;
        if (prefix > 0 && (char.IsHighSurrogate(source[prefix - 1]) || char.IsHighSurrogate(candidate[prefix - 1])))
            prefix--;

        var suffix = 0;
        while (suffix < source.Length - prefix && suffix < candidate.Length - prefix
               && source[source.Length - suffix - 1] == candidate[candidate.Length - suffix - 1])
            suffix++;
        var sourceSuffixStart = source.Length - suffix;
        if (suffix > 0 && sourceSuffixStart > 0 && char.IsHighSurrogate(source[sourceSuffixStart - 1]))
            suffix--;

        var sourceEnd = source.Length - suffix;
        var candidateEnd = candidate.Length - suffix;
        var sourceWord = WordToken.Matches(source).Cast<Match>()
            .FirstOrDefault(match => match.Index < prefix && prefix < match.Index + match.Length);
        var candidateWord = WordToken.Matches(candidate).Cast<Match>()
            .FirstOrDefault(match => match.Index < prefix && prefix < match.Index + match.Length);
        if (sourceWord is not null) prefix = Math.Min(prefix, sourceWord.Index);
        if (candidateWord is not null) prefix = Math.Min(prefix, candidateWord.Index);
        sourceEnd = source.Length - suffix;
        candidateEnd = candidate.Length - suffix;
        sourceWord = WordToken.Matches(source).Cast<Match>()
            .FirstOrDefault(match => match.Index < sourceEnd && sourceEnd < match.Index + match.Length);
        candidateWord = WordToken.Matches(candidate).Cast<Match>()
            .FirstOrDefault(match => match.Index < candidateEnd && candidateEnd < match.Index + match.Length);
        if (sourceWord is not null) suffix = Math.Min(suffix, source.Length - (sourceWord.Index + sourceWord.Length));
        if (candidateWord is not null) suffix = Math.Min(suffix, candidate.Length - (candidateWord.Index + candidateWord.Length));

        var original = source.Substring(prefix, source.Length - prefix - suffix);
        var replacement = candidate.Substring(prefix, candidate.Length - prefix - suffix);
        return ValidateEdits([new TextEdit(prefix, original.Length, original, replacement)], source);
    }

    private static string ExtractCandidate(string output, string source)
    {
        var text = output.Trim();
        if (text.Length == 0) throw new InvalidDataException("Correction output is empty.");

        if (text.StartsWith("```", StringComparison.Ordinal))
        {
            var firstLineEnd = text.IndexOf('\n');
            var closingFence = text.LastIndexOf("```", StringComparison.Ordinal);
            if (firstLineEnd < 0 || closingFence <= firstLineEnd)
                throw new InvalidDataException("Correction output contains an incomplete Markdown code fence.");
            text = text[(firstLineEnd + 1)..closingFence].Trim();
        }

        // Some chat models append explanations after the requested text. Recognize explicit note
        // markers while preserving blank lines that belong to a multi-paragraph input.
        var noteStart = Regex.Match(text,
            @"(?:^|\r?\n)\s*(?:\*+\s*\(|(?:Explanation|Note|Reasoning|Output|Final output|Corrected (?:text|output))\s*:)",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (noteStart.Success)
            text = text[..noteStart.Index].Trim();

        var responseLines = text.Split('\n').Select(line => line.Trim()).Where(line => line.Length > 0).ToArray();
        var repeatedCompleteLineCount = responseLines.Count(line => string.Equals(line, responseLines[0], StringComparison.Ordinal));
        var endsWithPartialRepeat = responseLines.Length > 1
            && responseLines[^1].Length < responseLines[0].Length
            && responseLines[0].StartsWith(responseLines[^1], StringComparison.Ordinal);
        if (!source.Contains('\n') && responseLines.Length > 1 && repeatedCompleteLineCount >= 2
            && responseLines.Take(responseLines.Length - (endsWithPartialRepeat ? 1 : 0))
                .All(line => string.Equals(line, responseLines[0], StringComparison.Ordinal))
            && (endsWithPartialRepeat || responseLines.All(line => string.Equals(line, responseLines[0], StringComparison.Ordinal))))
            text = responseLines[0];

        text = Regex.Replace(text, @"\*\*(.+?)\*\*|__(.+?)__|`([^`]+)`", match =>
            match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Success ? match.Groups[2].Value : match.Groups[3].Value,
            RegexOptions.CultureInvariant);
        text = Regex.Replace(text, @"^(?:Corrected (?:text|passage)|Correction|Output)\s*:\s*", "", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        text = text.Trim();
        if (text.Length == 0 || text.Contains("```", StringComparison.Ordinal)
            || Regex.IsMatch(text, @"(?:^|\n)\s*(?:Explanation|Note|Reasoning|Output|Final output)\s*:", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
            || text.StartsWith("Here is", StringComparison.OrdinalIgnoreCase)
            || text.StartsWith("The corrected", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Correction output does not contain a verifiable corrected passage.");
        if (text.Length > Math.Max(8192, source.Length * 2 + 256))
            throw new InvalidDataException("Correction output is too long to validate safely.");
        return text;
    }

    private static bool SameNonWhitespaceTokens(string first, string second)
    {
        static string Normalize(string value) => Regex.Replace(value, @"\s+", "", RegexOptions.CultureInvariant);
        return string.Equals(Normalize(first), Normalize(second), StringComparison.Ordinal);
    }

    private static bool HasSentenceEnding(string value)
    {
        var index = value.TrimEnd().Length - 1;
        while (index >= 0 && value[index] is '\"' or '\'' or '’' or '”' or ')' or ']' or '}') index--;
        return index >= 0 && value[index] is '.' or '!' or '?' or '…';
    }

    private static void EnsurePlausibleCorrection(string candidate, string source)
    {
        var sourceWords = WordToken.Matches(source).Select(match => match.Value).ToArray();
        var candidateWords = WordToken.Matches(candidate).Select(match => match.Value).ToArray();
        if (sourceWords.Length <= 1 && candidateWords.Length == 1)
            return;
        if (sourceWords.Length == 0 || candidateWords.Length == 0
            || candidateWords.Length * 2 < sourceWords.Length
            || candidateWords.Length > sourceWords.Length * 2)
            throw new InvalidDataException("Correction output cannot be verified against the source text.");

        var sourceTerms = sourceWords.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var candidateTerms = candidateWords.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var shared = sourceTerms.Intersect(candidateTerms, StringComparer.OrdinalIgnoreCase).Count();
        if (shared < Math.Min(sourceTerms.Count, candidateTerms.Count) * 0.5)
            throw new InvalidDataException("Correction output changes too much source wording to validate safely.");
    }

    public static IReadOnlyList<TextEdit> ParseAndValidate(string output, string source)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(source);
        try
        {
            using var document = JsonDocument.Parse(output);
            if (document.RootElement.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException("Correction output must be a JSON array.");
            var edits = new List<TextEdit>(document.RootElement.GetArrayLength());
            foreach (var item in document.RootElement.EnumerateArray())
            {
                if (!item.TryGetProperty("start", out var startValue) || !startValue.TryGetInt32(out var start)
                    || !item.TryGetProperty("length", out var lengthValue) || !lengthValue.TryGetInt32(out var length))
                    throw new InvalidDataException("Correction output is missing a valid source span.");
                if (start < 0 || length < 0 || start > source.Length - length)
                    throw new InvalidDataException("Correction output contains an out-of-range source span.");
                if (!item.TryGetProperty("originalText", out var originalValue) || originalValue.ValueKind != JsonValueKind.String
                    || !item.TryGetProperty("replacementText", out var replacementValue) || replacementValue.ValueKind != JsonValueKind.String)
                    throw new InvalidDataException("Correction output is missing edit text.");
                var original = originalValue.GetString()!;
                var replacement = replacementValue.GetString()!;
                if (original.Length != length)
                    throw new InvalidDataException("Correction output contains a span length that does not match its source text.");
                edits.Add(new TextEdit(start, length, original, replacement));
            }
            return ValidateEdits(edits, source);
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("Correction output is not valid JSON edit data.", exception);
        }
    }

    private static IReadOnlyList<TextEdit> ValidateEdits(IEnumerable<TextEdit> edits, string source)
    {
        var validated = new List<TextEdit>();
        foreach (var edit in edits)
        {
            if (edit.Start < 0 || edit.Length < 0 || edit.Start > source.Length - edit.Length)
                throw new InvalidDataException("Correction output contains an out-of-range source span.");
            if (!source.AsSpan(edit.Start, edit.Length).SequenceEqual(edit.OriginalText.AsSpan()))
                throw new InvalidDataException("Correction output source text does not match its span.");
            if (edit.OriginalText == edit.ReplacementText)
                throw new InvalidDataException("Correction output contains an unchanged edit.");
            if (!SameMatches(NumberToken, edit.OriginalText, edit.ReplacementText))
                throw new InvalidDataException("Correction output changes numeric content.");
            if (!SameMatches(UrlToken, edit.OriginalText, edit.ReplacementText))
                throw new InvalidDataException("Correction output changes a URL.");
            if (!SameMatches(ProtectedIdentifier, edit.OriginalText, edit.ReplacementText))
                throw new InvalidDataException("Correction output changes a likely technical identifier.");
            if (!SameMatches(CapitalizedWord, edit.OriginalText, edit.ReplacementText))
                throw new InvalidDataException("Correction output changes a capitalized name or term.");
            validated.Add(edit);
        }

        var ordered = validated.OrderBy(edit => edit.Start).ThenBy(edit => edit.Length).ToArray();
        for (var i = 1; i < ordered.Length; i++)
            if (ordered[i].Start < ordered[i - 1].Start + ordered[i - 1].Length
                || ordered[i].Start == ordered[i - 1].Start && (ordered[i].Length == 0 || ordered[i - 1].Length == 0))
                throw new InvalidDataException("Correction output contains overlapping edits.");
        return ordered;
    }

    private static bool SameMatches(Regex pattern, string original, string replacement) =>
        pattern.Matches(original).Select(match => match.Value).SequenceEqual(pattern.Matches(replacement).Select(match => match.Value), StringComparer.Ordinal);

    private static bool SameSymbols(string original, string replacement) =>
        ProtectedSymbols(original).SequenceEqual(ProtectedSymbols(replacement), StringComparer.Ordinal);

    private static IEnumerable<string> ProtectedSymbols(string value)
    {
        foreach (var rune in value.EnumerateRunes())
            if (Rune.GetUnicodeCategory(rune) is UnicodeCategory.MathSymbol or UnicodeCategory.CurrencySymbol
                or UnicodeCategory.ModifierSymbol or UnicodeCategory.OtherSymbol)
                yield return rune.ToString();
    }

}
