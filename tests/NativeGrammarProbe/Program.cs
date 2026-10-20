using System.Text.Json;
using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;

if (args.Length != 1) { Console.Error.WriteLine("Pass one existing reviewed local GGUF path."); return 2; }
using var adapter = new LlamaSharpInferenceAdapter(args[0], threads: 9);
using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(90));
// Exercise the real experimental prompt builder and grammar path under Native AOT.
var engine = new ExperimentalCorrectionEngine(adapter, CorrectionExperimentVariant.StructuredOffsets, 128, constrainJson: true);
var result = await engine.CorrectAsync(new(CorrectionLanguage.English, "Correct text."), timeout.Token);
if (result.Termination != GenerationTermination.EndOfGeneration) throw new InvalidDataException("Grammar probe did not reach natural EOG.");
using var json = JsonDocument.Parse(result.Analysis!.RawOutput);
if (json.RootElement.ValueKind != JsonValueKind.Array) throw new InvalidDataException("Grammar probe did not emit an array.");
// Correction quality may fail: this is a contract/native-runtime probe, not a linguistic test.
Console.WriteLine($"Native AOT grammar: valid JSON array; natural EOG; {adapter.LastSampledTokenCount} sampled tokens; disposition {result.Disposition}.");
// Exercise source/target policy binding under AOT without making a quality claim.
foreach (var language in new[] { CorrectionLanguage.Dutch, CorrectionLanguage.English })
{
    const string document = "🌷 Context.\nPlease recieve it.\nKeep OAuth2 unchanged.";
    var target = new ContextSegment("t", "p", 12, "Please recieve it.", new Dictionary<string, string> { ["run"] = "bold" });
    var contextRequest = new ContextCorrectionRequest("r", document, language, target,
        [new("before", "p0", 0, "🌷 Context.", new Dictionary<string, string>())]);
    var contextResult = await new ContextCorrectionEngine(new FixedAdapter()).CorrectAsync(contextRequest);
    var mapped = ContextCorrectionEngine.MapToCurrentSnapshot(contextRequest, contextResult, "r", document);
    if (!SourceEdits.Apply(document, mapped).Contains("Please receive it.", StringComparison.Ordinal))
        throw new InvalidDataException("Native AOT context mapping failed.");
}
Console.WriteLine("Native AOT context: target-only UTF-16 mapping and metadata policy passed (fake adapter; no linguistic measurement).");
return 0;

sealed class FixedAdapter : IModelInferenceAdapter
{
    public ValueTask<InferenceResult> GenerateAsync(InferenceRequest request, CancellationToken cancellationToken = default)
        => ValueTask.FromResult(new InferenceResult("Please receive it.", GenerationTermination.EndOfGeneration, 1));
}
