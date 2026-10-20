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
return 0;
