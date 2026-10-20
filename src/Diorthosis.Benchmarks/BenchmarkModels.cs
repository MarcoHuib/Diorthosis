using Diorthosis.Core.Corrections;

namespace Diorthosis.Benchmarks;

public sealed record AcceptedCorrection(string ReplacementText);

public sealed record ErrorAnnotation(int Start, int Length, string OriginalText, ErrorCategory Category,
    IReadOnlyList<AcceptedCorrection> AcceptedCorrections);

public sealed record BenchmarkCase(string Id, CorrectionLanguage Language, string InputText,
    IReadOnlyList<ErrorAnnotation> Errors);

public sealed record BenchmarkPrediction(string CaseId, IReadOnlyList<TextEdit> Edits);

public sealed record MetricResult(string Language, string Category, int AnnotatedErrors, int CorrectlyResolvedErrors,
    int SuggestedEdits, int CorrectSuggestions, int FalsePositiveSuggestions, int CorrectInputs,
    int CorrectInputsChanged, double Recall, double Precision, double FalsePositiveRate,
    double CorrectTextPreservation);

public sealed record QualityMetricResult(string EvaluatorVersion, string Language, int AnnotatedErrors,
    int CorrectlyResolvedErrors, int SuggestedEdits, int CorrectSuggestions, int CorrectInputs,
    int CorrectInputsChanged, int StrictlyAcceptedCases, int StrictCaseDenominator,
    int UnnecessaryModifications, bool DispositionsAvailable, int? AcceptedChanges, int? RejectedChanges,
    int? SuppressedChanges, int? UnchangedChanges, int? NeedsReviewChanges,
    double? ErrorLevelRecall, double? StrictCompletePassageAcceptability,
    double? HistoricalHunkPrecision, double? CleanInputPreservation);
