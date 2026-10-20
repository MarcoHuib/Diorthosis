using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;

if (args.Length == 1 && args[0] is "--version" or "-v")
{
    Console.WriteLine("Diorthosis 0.1.0");
    return 0;
}

if (args.Length == 1 && args[0] == "--check-native-inference")
{
    try
    {
        LlamaSharpInferenceAdapter.ValidateNativeBackend();
        Console.WriteLine("Native CPU inference backend loaded.");
        return 0;
    }
    catch (Exception exception) when (exception is DllNotFoundException or BadImageFormatException or TypeInitializationException or InvalidOperationException)
    {
        Console.Error.WriteLine("The native CPU inference backend could not be loaded for this platform.");
        return 1;
    }
}

if (args.Length > 0 && args[0] == "correct-text")
{
    try
    {
        var values = ParseOptions(args[1..]);
        if (!values.TryGetValue("language", out var languageText) || !values.TryGetValue("model", out var model)
            || !values.TryGetValue("text", out var source))
        {
            Console.Error.WriteLine("Usage: diorthosis correct-text --language nl|en --model <path.gguf> --text <text> [--max-output-tokens <count>] [--json]");
            return 2;
        }
        var language = languageText switch
        {
            "nl" => CorrectionLanguage.Dutch,
            "en" => CorrectionLanguage.English,
            _ => throw new ArgumentException("Language must be 'nl' or 'en'.")
        };
        var maxTokens = 512;
        if (values.TryGetValue("max-output-tokens", out var tokenText) && !int.TryParse(tokenText, out maxTokens))
            throw new ArgumentException("Maximum output tokens must be a positive integer.");
        if (maxTokens <= 0) throw new ArgumentException("Maximum output tokens must be positive.");

        using var adapter = new LlamaSharpInferenceAdapter(model);
        var engine = new ModelCorrectionEngine(adapter, maxTokens);
        var result = await engine.CorrectAsync(new CorrectionRequest(language, source));
        var corrected = ApplyEdits(source, result.Edits);
        if (values.ContainsKey("json"))
        {
            var editsJson = string.Join(",", result.Edits.Select(edit =>
                $"{{\"start\":{edit.Start},\"length\":{edit.Length},\"originalText\":{EncodeJsonString(edit.OriginalText)},\"replacementText\":{EncodeJsonString(edit.ReplacementText)}}}"));
            Console.WriteLine($"{{\"disposition\":{EncodeJsonString(result.Disposition.ToString())},\"reason\":{(result.Analysis?.RejectionReason is { } reason ? EncodeJsonString(reason) : "null")},\"language\":{EncodeJsonString(languageText)},\"correctedText\":{EncodeJsonString(corrected)},\"edits\":[{editsJson}]}}");
        }
        else
        {
            Console.WriteLine($"Outcome: {result.Disposition}");
            if (result.Analysis?.RejectionReason is { } reason) Console.WriteLine(reason);
            foreach (var edit in result.Edits)
                Console.WriteLine($"{edit.Start}:{edit.Length}  {EncodeJsonString(edit.OriginalText)} → {EncodeJsonString(edit.ReplacementText)}");
            Console.WriteLine(corrected);
        }
        return 0;
    }
    catch (Exception exception) when (exception is ArgumentException or IOException or InvalidDataException or DllNotFoundException or BadImageFormatException)
    {
        Console.Error.WriteLine(exception.Message);
        return 1;
    }
}

Console.Error.WriteLine("Usage: diorthosis correct-text ... | --version | --check-native-inference");
return 2;

static Dictionary<string, string> ParseOptions(string[] arguments)
{
    var result = new Dictionary<string, string>(StringComparer.Ordinal);
    for (var index = 0; index < arguments.Length; index++)
    {
        var name = arguments[index];
        if (!name.StartsWith("--", StringComparison.Ordinal)) throw new ArgumentException("Expected a named option.");
        var key = name[2..];
        if (key is not ("language" or "model" or "text" or "max-output-tokens" or "json"))
            throw new ArgumentException($"Unknown option --{key}.");
        if (key == "json") { result.Add(key, "true"); continue; }
        if (++index >= arguments.Length) throw new ArgumentException($"Missing value for --{key}.");
        result.Add(key, arguments[index]);
    }
    return result;
}

static string ApplyEdits(string source, IReadOnlyList<TextEdit> edits)
{
    var result = source;
    foreach (var edit in edits.OrderByDescending(edit => edit.Start))
        result = string.Concat(result.AsSpan(0, edit.Start), edit.ReplacementText, result.AsSpan(edit.Start + edit.Length));
    return result;
}

static string EncodeJsonString(string value) => "\"" + System.Text.Json.JsonEncodedText.Encode(value).ToString() + "\"";
