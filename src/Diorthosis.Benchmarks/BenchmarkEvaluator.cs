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
            foreach (var error in item.Errors)
            {
                var match = Enumerable.Range(0, edits.Count).FirstOrDefault(index => !matchedEdits.Contains(index)
                    && IsAccepted(error, edits[index]), -1);
                if (match >= 0)
                {
                    matchedEdits.Add(match);
                    correctSuggestions++;
                    resolvedErrors++;
                }
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

    private static bool IsAccepted(ErrorAnnotation error, TextEdit edit) =>
        edit.Start == error.Start && edit.Length == error.Length && edit.OriginalText == error.OriginalText
        && error.AcceptedCorrections.Any(accepted => accepted.ReplacementText == edit.ReplacementText);

    private static void ValidateEdits(BenchmarkCase item, IReadOnlyList<TextEdit> edits)
    {
        foreach (var edit in edits)
        {
            if (edit.Start + edit.Length > item.InputText.Length
                || !item.InputText.AsSpan(edit.Start, edit.Length).SequenceEqual(edit.OriginalText.AsSpan()))
                throw new ArgumentException($"Prediction for case '{item.Id}' has a span that does not match its input.");
        }
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
