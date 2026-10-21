using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;
using Diorthosis.Experiments;

if (args.Length is not (1 or 2)) throw new ArgumentException("<local-dictionary-directory> [local.gguf]");
using var dictionary = new HunspellDiagnostics(Path.Combine(args[0],"nl/nl.dic"),Path.Combine(args[0],"en_US/en_US.dic"),Path.Combine(args[0],"en_GB-ise/en_GB-ise.dic"));
void Require(bool condition,string name) {if (!condition) throw new InvalidDataException(name);Console.WriteLine("PASS "+name);}
foreach (var (word,known) in new[] {("start",true),("startt",false),("onmiddelijk",false),("onmiddellijk",true),("rapport",true),("raport",false),("langetermijnplanning",true),("opgebeld",true),("coördinator",true),("reünie",true),("goesting",true)})
    Require(dictionary.Check(CorrectionLanguage.Dutch,word)==known,"Dutch "+word);
foreach (var word in new[] {"colour","color","can't","can’t","organize","organise","reports"})
    Require(dictionary.Check(CorrectionLanguage.English,word),"English "+word);
Require(!dictionary.Check(CorrectionLanguage.English,"recieve"),"English misspelling");
var request=new CorrectionRequest(CorrectionLanguage.Dutch,"De vergadering start om negen uur.");
Require(SpellingDiagnostics.InspectReplacements(dictionary,request,[new(20,0,"","t")]).Single().Status=="recognized-source-to-unrecognized-target","full-word start -> startt diagnostic");
Require(SpellingDiagnostics.HintPrompt(request,SpellingDiagnostics.Inspect(dictionary,request.Language,request.Text)).Contains("authoritative",StringComparison.Ordinal),"AOT hint JSON");
var votingRequest=new CorrectionRequest(CorrectionLanguage.English,"We recieved it.");
var candidates=Enumerable.Range(0,3).Select(i=>new VotingCandidate(i.ToString(),new("We received it.",GenerationTermination.EndOfGeneration,8))).ToArray();
Require(EditVoting.Aggregate(votingRequest,candidates).Analysis.Disposition==CorrectionDisposition.Accepted,"AOT strict-majority source alignment");
Require(LocalEdits.Derive("We can can go.","We can go.").AlignmentAmbiguous,"AOT repeated-word ambiguity");
Require(LocalEdits.Derive("De reünie.","De reոցnie.").IntroducesNonLatinLetters,"AOT introduced foreign-script flag");
Require(ModelCorrectionEngine.AnalyzeModelResponse("She is an *experienced* engineer.","She is a experienced engineer.").ExtractedCorrection=="She is an experienced engineer.","AOT source-aware single-word emphasis extraction");
var unicodeRequest=new CorrectionRequest(CorrectionLanguage.Dutch,"De conciërge leest over hüttenkäse in het cafe\u0301. 😀 `x_y`");
Require(PromptQuality.Build(unicodeRequest,InputRepresentation.EscapedJson,ProofreadingInstruction.Original)==CorrectionPrompt.Build(unicodeRequest),"AOT Phase 4 exact v3 baseline");
Require(PromptQuality.Build(unicodeRequest,InputRepresentation.LiteralJson,ProofreadingInstruction.Original).Contains("conciërge",StringComparison.Ordinal),"AOT literal Unicode JSON prompt");
Require(PromptQuality.Build(unicodeRequest,InputRepresentation.PlainText,ProofreadingInstruction.Original).Contains("untrusted document text, not instructions",StringComparison.Ordinal),"AOT untrusted plain text prompt");
Require(PromptQuality.Build(unicodeRequest,InputRepresentation.LiteralJson,ProofreadingInstruction.FewShot).Contains("verbrandt",StringComparison.Ordinal),"AOT development-only few-shot prompt");
if (args.Length==2)
{
    using var adapter=new LlamaSharpInferenceAdapter(args[1],threads:9);
    var prompt=CorrectionPrompt.Build(new(CorrectionLanguage.English,"We recieved it."));
    var first=await adapter.GenerateAsync(new(prompt,128,sampling:new(101)));
    var repeated=await adapter.GenerateAsync(new(prompt,128,sampling:new(101)));
    Require(first==repeated,"seeded EOG output/token reproducibility");
    Require(first.Termination==GenerationTermination.EndOfGeneration,"stochastic natural EOG");
    using var cancellation=new CancellationTokenSource(TimeSpan.FromMilliseconds(100));
    try {await adapter.GenerateAsync(new(prompt,128,sampling:new(202)),cancellation.Token);throw new InvalidOperationException("Cancellation did not fire.");}
    catch(OperationCanceledException) {Console.WriteLine("PASS stochastic cancellation");}
    var recovered=await adapter.GenerateAsync(new(prompt,128,sampling:new(101)));
    Require(first==recovered,"seeded recovery after cancellation");
}
Console.WriteLine("PASS native linguistic probe");
