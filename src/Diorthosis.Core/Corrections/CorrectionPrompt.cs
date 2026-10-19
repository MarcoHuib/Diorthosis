using System.Text.Json;

namespace Diorthosis.Core.Corrections;

/// <summary>Prompt contract version used by reproducible evaluations.</summary>
public static class CorrectionPrompt
{
    public const string Version = "diorthosis-correction-v3";

    public static string Build(CorrectionRequest request)
    {
        ArgumentNullException.ThrowIfNull(request);
        var language = request.Language == CorrectionLanguage.Dutch ? "Dutch" : "English";
        var encodedInput = "\"" + JsonEncodedText.Encode(request.Text).ToString() + "\"";
        return $"You are a careful {language} proofreader. Correct only clear spelling, grammar, punctuation, and sentence-structure errors. Preserve the original meaning and all correct wording. Never change names, numbers, dates, URLs, legal wording, or technical identifiers. Do not rewrite correct text.\nReturn only the complete corrected passage, with its paragraph breaks preserved. Do not add labels, Markdown, quotes, or explanations. If the input is already correct, return it exactly unchanged.\nInput text (JSON string): {encodedInput}";
    }
}
