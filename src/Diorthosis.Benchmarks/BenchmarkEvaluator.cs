using Diorthosis.Core.Corrections;

namespace Diorthosis.Benchmarks;

public static class BenchmarkEvaluator
{
    public static IReadOnlyList<MetricResult> Evaluate(IReadOnlyList<BenchmarkCase> cases,
        IReadOnlyList<BenchmarkPrediction> predictions)
    {
        ArgumentNullException.ThrowIfNull(cases);
        ArgumentNullException.ThrowIfNull(predictions);
        var caseMap = cases.ToDictionary(item => item.Id, StringComparer.Ordinal);
        if (caseMap.Count != cases.Count) throw new ArgumentException("Benchmark case IDs must be unique.", nameof(cases));
        foreach (var item in cases) ValidateCase(item);
        var predictionMap = predictions.ToDictionary(item => item.CaseId, StringComparer.Ordinal);
        if (predictionMap.Count != predictions.Count) throw new ArgumentException("Prediction case IDs must be unique.", nameof(predictions));
        if (predictionMap.Keys.Any(key => !caseMap.ContainsKey(key))) throw new ArgumentException("Predictions contain an unknown case ID.", nameof(predictions));

        var results = new List<MetricResult>();
        foreach (var language in Enum.GetValues<CorrectionLanguage>())
        {
            var languageCases = cases.Where(item => item.Language == language).ToArray();
            results.Add(EvaluateGroup(language, "all", languageCases, predictionMap));
            foreach (var category in Enum.GetValues<ErrorCategory>().Where(category => category != ErrorCategory.CorrectText))
            {
                var categoryCases = languageCases.Where(item => item.Errors.Any(error => error.Category == category))
                    .Select(item => item with { Errors = [.. item.Errors.Where(error => error.Category == category)] }).ToArray();
                results.Add(EvaluateGroup(language, category.ToString(), categoryCases, predictionMap));
            }
            var correctCases = languageCases.Where(item => item.Errors.Count == 0).ToArray();
            results.Add(EvaluateGroup(language, ErrorCategory.CorrectText.ToString(), correctCases, predictionMap));
        }
        return results;
    }

    private static MetricResult EvaluateGroup(CorrectionLanguage language, string category, BenchmarkCase[] cases,
        IReadOnlyDictionary<string, BenchmarkPrediction> predictions)
    {
        var errors = cases.SelectMany(item => item.Errors.Select(error => (item, error))).ToArray();
        var suggestions = cases.Sum(item => predictions.TryGetValue(item.Id, out var prediction) ? prediction.Edits.Count : 0);
        var correctSuggestions = 0;
        var resolvedErrors = 0;
        var correctInputs = 0;
        var changedCorrectInputs = 0;

        foreach (var item in cases)
        {
            var edits = predictions.TryGetValue(item.Id, out var prediction) ? prediction.Edits : [];
            ValidateEdits(item, edits);
            var matchedEdits = new HashSet<int>();
            var caseCorrectSuggestions = 0;
            var caseResolvedErrors = 0;
            foreach (var error in item.Errors)
            {
                var related = Enumerable.Range(0, edits.Count).Where(index => !matchedEdits.Contains(index)
                    && Touches(error, edits[index])).ToArray();
                if (related.Length == 0) continue;
                var expected = error.AcceptedCorrections.Select(correction =>
                    item.InputText[..error.Start] + correction.ReplacementText + item.InputText[(error.Start + error.Length)..]);
                if (expected.Contains(ApplyEdits(item.InputText, related.Select(index => edits[index])), StringComparer.Ordinal))
                {
                    foreach (var index in related) matchedEdits.Add(index);
                    caseCorrectSuggestions += related.Length;
                    caseResolvedErrors++;
                }
            }

            var unmatchedEdits = Enumerable.Range(0, edits.Count).Where(index => !matchedEdits.Contains(index))
                .Select(index => edits[index]).ToArray();
            // Do not give recall credit when an otherwise matching local repair is embedded in
            // an unannotated lexical rewrite, translation, or other potentially meaning-changing
            // passage edit. Cosmetic case/spacing/punctuation false positives remain measurable.
            if (unmatchedEdits.Length == 0 || unmatchedEdits.All(IsCosmeticEdit))
            {
                correctSuggestions += caseCorrectSuggestions;
                resolvedErrors += caseResolvedErrors;
            }

            if (item.Errors.Count == 0)
            {
                correctInputs++;
                if (edits.Count > 0) changedCorrectInputs++;
            }
        }

        var falsePositives = suggestions - correctSuggestions;
        return new MetricResult(language.ToString(), category, errors.Length, resolvedErrors, suggestions, correctSuggestions,
            falsePositives, correctInputs, changedCorrectInputs,
            Ratio(resolvedErrors, errors.Length), Ratio(correctSuggestions, suggestions),
            Ratio(changedCorrectInputs, correctInputs), Ratio(correctInputs - changedCorrectInputs, correctInputs));
    }

    private static bool IsCosmeticEdit(TextEdit edit)
    {
        if (string.Equals(edit.OriginalText, edit.ReplacementText, StringComparison.OrdinalIgnoreCase)) return true;
        static bool HasWordCharacters(string value) => value.EnumerateRunes().Any(rune =>
        {
            var category = System.Text.Rune.GetUnicodeCategory(rune);
            return category is System.Globalization.UnicodeCategory.UppercaseLetter or System.Globalization.UnicodeCategory.LowercaseLetter
                or System.Globalization.UnicodeCategory.TitlecaseLetter or System.Globalization.UnicodeCategory.ModifierLetter
                or System.Globalization.UnicodeCategory.OtherLetter or System.Globalization.UnicodeCategory.DecimalDigitNumber
                or System.Globalization.UnicodeCategory.LetterNumber or System.Globalization.UnicodeCategory.OtherNumber;
        });
        return !HasWordCharacters(edit.OriginalText) && !HasWordCharacters(edit.ReplacementText);
    }

    private static bool Touches(ErrorAnnotation error, TextEdit edit)
    {
        var errorEnd = error.Start + error.Length;
        var editEnd = edit.Start + edit.Length;
        return edit.Length == 0 ? edit.Start >= error.Start && edit.Start <= errorEnd
            : edit.Start < errorEnd && editEnd > error.Start;
    }

    private static string ApplyEdits(string source, IEnumerable<TextEdit> edits)
    {
        var result = new System.Text.StringBuilder(source);
        foreach (var edit in edits.OrderByDescending(edit => edit.Start))
            result.Remove(edit.Start, edit.Length).Insert(edit.Start, edit.ReplacementText);
        return result.ToString();
    }

    private static void ValidateEdits(BenchmarkCase item, IReadOnlyList<TextEdit> edits)
    {
        foreach (var edit in edits)
        {
            if (edit.Start < 0 || edit.Length < 0 || edit.Start > item.InputText.Length - edit.Length
                || !item.InputText.AsSpan(edit.Start, edit.Length).SequenceEqual(edit.OriginalText.AsSpan()))
                throw new ArgumentException($"Prediction for case '{item.Id}' has a span that does not match its input.");
        }
        var ordered = edits.OrderBy(edit => edit.Start).ThenBy(edit => edit.Length).ToArray();
        for (var i = 1; i < ordered.Length; i++)
            if (ordered[i].Start < ordered[i - 1].Start + ordered[i - 1].Length
                || ordered[i].Start == ordered[i - 1].Start && (ordered[i].Length == 0 || ordered[i - 1].Length == 0))
                throw new ArgumentException($"Prediction for case '{item.Id}' contains overlapping edits.");
    }

    private static void ValidateCase(BenchmarkCase item)
    {
        if (string.IsNullOrWhiteSpace(item.Id)) throw new ArgumentException("Benchmark case IDs must be non-empty.");
        if (!Enum.IsDefined(item.Language)) throw new ArgumentException($"Case '{item.Id}' has an unsupported language.");
        ArgumentException.ThrowIfNullOrWhiteSpace(item.InputText);
        ArgumentNullException.ThrowIfNull(item.Errors);
        foreach (var error in item.Errors)
        {
            if (!Enum.IsDefined(error.Category)) throw new ArgumentException($"Case '{item.Id}' has an unsupported category.");
            if (error.Start < 0 || error.Length < 0 || error.Start + error.Length > item.InputText.Length
                || !item.InputText.AsSpan(error.Start, error.Length).SequenceEqual(error.OriginalText.AsSpan()))
                throw new ArgumentException($"Case '{item.Id}' has an annotation span that does not match its input.");
            if (error.AcceptedCorrections.Count == 0)
                throw new ArgumentException($"Case '{item.Id}' has an annotation without an accepted correction.");
        }
    }

    private static double Ratio(int numerator, int denominator) => denominator == 0 ? 0 : (double)numerator / denominator;
}
