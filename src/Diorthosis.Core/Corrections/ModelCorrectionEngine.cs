using System.Text.Json;
using System.Text.RegularExpressions;
using Diorthosis.Core.Inference;

namespace Diorthosis.Core.Corrections;

public sealed class ModelCorrectionEngine(IModelInferenceAdapter adapter, int maximumOutputTokens = 512) : ICorrectionEngine
{
    private static readonly Regex UrlToken = new(@"(?:https?://|www\.)[^\s<>]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex NumberToken = new(@"\d+(?:[.,:/-]\d+)*", RegexOptions.CultureInvariant);
    private static readonly Regex ProtectedIdentifier = new(@"\b(?:[A-Za-z0-9]*[A-Z][a-z]+[A-Za-z0-9]*|[A-Z]{2,}[A-Z0-9]*|[A-Za-z0-9]*\d+[A-Za-z0-9]*|[A-Za-z0-9]+(?:_[A-Za-z0-9]+)+)\b", RegexOptions.CultureInvariant);
    private static readonly Regex CapitalizedWord = new(@"\b\p{Lu}[\p{L}'’.-]*\b", RegexOptions.CultureInvariant);
    private readonly IModelInferenceAdapter _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));

    public async ValueTask<CorrectionResult> CorrectAsync(CorrectionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (maximumOutputTokens <= 0) throw new ArgumentOutOfRangeException(nameof(maximumOutputTokens));
        var raw = await _adapter.GenerateAsync(new InferenceRequest(CorrectionPrompt.Build(request), maximumOutputTokens), cancellationToken).ConfigureAwait(false);
        return new CorrectionResult(ParseAndValidate(raw, request.Text));
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
                if (original.Length != length || !source.AsSpan(start, length).SequenceEqual(original.AsSpan()))
                    throw new InvalidDataException("Correction output source text does not match its span.");
                if (original == replacement)
                    throw new InvalidDataException("Correction output contains an unchanged edit.");
                if (!SameMatches(NumberToken, original, replacement))
                    throw new InvalidDataException("Correction output changes numeric content.");
                if (!SameMatches(UrlToken, original, replacement))
                    throw new InvalidDataException("Correction output changes a URL.");
                if (!SameMatches(ProtectedIdentifier, original, replacement))
                    throw new InvalidDataException("Correction output changes a likely technical identifier.");
                if (!SameMatches(CapitalizedWord, original, replacement))
                    throw new InvalidDataException("Correction output changes a capitalized name or term.");
                edits.Add(new TextEdit(start, length, original, replacement));
            }

            var ordered = edits.OrderBy(edit => edit.Start).ThenBy(edit => edit.Length).ToArray();
            for (var i = 1; i < ordered.Length; i++)
                if (ordered[i].Start < ordered[i - 1].Start + ordered[i - 1].Length
                    || ordered[i].Start == ordered[i - 1].Start && (ordered[i].Length == 0 || ordered[i - 1].Length == 0))
                    throw new InvalidDataException("Correction output contains overlapping edits.");
            return ordered;
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("Correction output is not valid JSON edit data.", exception);
        }
    }

    private static bool SameMatches(Regex pattern, string original, string replacement) =>
        pattern.Matches(original).Select(match => match.Value).SequenceEqual(pattern.Matches(replacement).Select(match => match.Value), StringComparer.Ordinal);

}
