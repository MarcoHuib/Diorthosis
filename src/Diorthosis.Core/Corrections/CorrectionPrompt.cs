using System.Text.Json;

namespace Diorthosis.Core.Corrections;

/// <summary>Prompt contract version used by reproducible evaluations.</summary>
public static class CorrectionPrompt
{
    public const string Version = "diorthosis-correction-v1";

    public static string Build(CorrectionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var language = request.Language == CorrectionLanguage.Dutch ? "Dutch" : "English";
        var encodedInput = "\"" + JsonEncodedText.Encode(request.Text).ToString() + "\"";
        return $"You are a careful {language} proofreader. Correct only clear spelling, grammar, punctuation, and sentence-structure errors. Preserve meaning, names, numbers, dates, URLs, legal wording, and technical identifiers. Leave correct text unchanged. Return only a JSON array of edits, each with integer start and length measured in UTF-16 code units, exact originalText copied from the input, and replacementText. Use [] when no correction is needed. Do not return Markdown or explanations.\nInput text (JSON string): {encodedInput}";
    }
}
