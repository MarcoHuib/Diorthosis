using System.Text.Json;
using System.Text.Json.Serialization;
using Diorthosis.Benchmarks;
using Diorthosis.Core.Corrections;

// No inference adapter implementation is compiled or called by this analysis driver.
var options = new JsonSerializerOptions(JsonSerializerDefaults.Web) { WriteIndented = true };
options.Converters.Add(new JsonStringEnumConverter());
T[] Read<T>(string path) => File.ReadLines(path).Where(x => !string.IsNullOrWhiteSpace(x))
    .Select(x => JsonSerializer.Deserialize<T>(x, options)!).ToArray();
MetricResult All(BenchmarkCase c, BenchmarkPrediction p) => BenchmarkEvaluator.Evaluate([c], [p])
    .Single(m => m.Language == c.Language.ToString() && m.Category == "all");
string Apply(string text, IReadOnlyList<TextEdit> edits)
{
    foreach (var edit in edits.OrderByDescending(e => e.Start))
        text = text.Remove(edit.Start, edit.Length).Insert(edit.Start, edit.ReplacementText);
    return text;
}
void Require(bool value, string reason) { if (!value) throw new InvalidDataException(reason); }

if (args.Length == 4 && args[0] == "score")
{
    var cases = Read<BenchmarkCase>(args[1]);
    var predictions = Read<BenchmarkPrediction>(args[2]);
    var byId = predictions.ToDictionary(p => p.CaseId);
    Require(cases.Length == predictions.Length && cases.All(c => byId.ContainsKey(c.Id)), "Incomplete predictions");
    var result = new { metrics = BenchmarkEvaluator.Evaluate(cases, predictions),
        cases = cases.Select(c => new { caseId = c.Id, output = Apply(c.InputText, byId[c.Id].Edits),
            metrics = All(c, byId[c.Id]) }).ToArray() };
    File.WriteAllText(args[3], JsonSerializer.Serialize(result, options) + "\n");
    return;
}
if (args.Length == 3 && args[0] == "replay")
{
    var rows = Read<JsonElement>(args[1]);
    foreach (var row in rows)
    {
        var a = ModelCorrectionEngine.AnalyzeModelResponse(row.GetProperty("rawModelOutput").GetString()!, row.GetProperty("input").GetString()!);
        var edits = row.GetProperty("validatedTextEdits").Deserialize<TextEdit[]>(options)!;
        var rawEdits = row.GetProperty("rawCandidateTextEdits").Deserialize<TextEdit[]>(options)!;
        Require(a.Edits.SequenceEqual(edits) && a.RawCandidateEdits.SequenceEqual(rawEdits), "Replay edit mismatch");
        Require(a.ExtractedCorrection == row.GetProperty("extractedCorrection").GetString(), "Extraction mismatch");
        Require(a.RejectionStage == row.GetProperty("rejectionStage").GetString(), "Rejection mismatch");
        Require(a.RejectionReason == row.GetProperty("rejectionReason").GetString(), "Rejection reason mismatch");
    }
    File.WriteAllText(args[2], JsonSerializer.Serialize(new { rows = rows.Length, identicalReplay = true, inference = false }, options) + "\n");
    return;
}
if (args.Length != 2 || args[0] != "probe") throw new ArgumentException("score <cases> <predictions> <output> | replay <trace> <output> | probe <output>");

var observations = new List<object>();
const string source = "She have recieved it.";
var item = new BenchmarkCase("cross-category", CorrectionLanguage.English, source, [
    new(4, 4, "have", ErrorCategory.Grammar, [new("has")]),
    new(9, 8, "recieved", ErrorCategory.Spelling, [new("received")])]);
var split = new BenchmarkPrediction(item.Id, [new(4, 4, "have", "has"), new(9, 8, "recieved", "received")]);
var combined = new BenchmarkPrediction(item.Id, [new(0, source.Length, source, "She has received it.")]);
var splitMetrics = BenchmarkEvaluator.Evaluate([item], [split]);
Require(All(item, split).CorrectlyResolvedErrors == 2, "Independent all-category repair");
Require(splitMetrics.Where(m => m.Language == "English" && m.Category is "Grammar" or "Spelling").All(m => m.CorrectlyResolvedErrors == 0), "Category filtering reproduction");
Require(All(item, combined).CorrectlyResolvedErrors == 0, "Cross-annotation merged edit reproduction");
observations.Add(new { id = "category-filter-and-cross-boundary", splitMetrics, combined = All(item, combined) });

var restructureSource = "Tomorrow I will the package send.";
var restructureTarget = "Tomorrow, I will send the package.";
var restructureCase = new BenchmarkCase("hunks", CorrectionLanguage.English, restructureSource,
    [new(0, restructureSource.Length, restructureSource, ErrorCategory.SentenceStructure, [new(restructureTarget)])]);
var diff = ModelCorrectionEngine.AnalyzeModelResponse(restructureTarget, restructureSource);
var multi = All(restructureCase, new("hunks", diff.Edits));
var single = All(restructureCase, new("hunks", [new(0, restructureSource.Length, restructureSource, restructureTarget)]));
Require(multi.Recall == 1 && multi.Precision == 1 && multi.SuggestedEdits > 1 && single.SuggestedEdits == 1, "Hunk-weight reproduction");
observations.Add(new { id = "same-repair-different-precision-weight", multi, single, edits = diff.Edits });

var typoCase = new BenchmarkCase("lexical-veto", CorrectionLanguage.English, "Please recieve the form.",
    [new(7, 7, "recieve", ErrorCategory.Spelling, [new("receive")])]);
var harmless = new BenchmarkPrediction(typoCase.Id, [new(0, 6, "Please", "Kindly"), new(7, 7, "recieve", "receive")]);
Require(All(typoCase, harmless).CorrectlyResolvedErrors == 0, "Lexical veto reproduction");
observations.Add(new { id = "repair-plus-style-rewrite", metrics = All(typoCase, harmless), prediction = harmless });

var punctuationCase = new BenchmarkCase("punctuation-meaning", CorrectionLanguage.English, "She will recieve it.",
    [new(9, 7, "recieve", ErrorCategory.Spelling, [new("receive")])]);
var question = new BenchmarkPrediction(punctuationCase.Id, [new(9, 7, "recieve", "receive"), new(19, 1, ".", "?")]);
Require(All(punctuationCase, question).CorrectlyResolvedErrors == 1, "Cosmetic meaning-blindness reproduction");
observations.Add(new { id = "punctuation-changes-speech-act", metrics = All(punctuationCase, question), prediction = question });

var unicodeCase = new BenchmarkCase("canonical-unicode", CorrectionLanguage.Dutch, "geinstalleerd",
    [new(0, 13, "geinstalleerd", ErrorCategory.Spelling, [new("geïnstalleerd")])]);
var decomposed = new BenchmarkPrediction(unicodeCase.Id, [new(0, 13, "geinstalleerd", "gei\u0308nstalleerd")]);
Require(All(unicodeCase, decomposed).Recall == 0, "Canonical Unicode mismatch reproduction");
observations.Add(new { id = "canonical-unicode-exact-mismatch", metrics = All(unicodeCase, decomposed) });

foreach (var probe in new[] {
    ("identifier-hyphen", "The service starts on node-04.", "The service starts on node 04.", false, true),
    ("identifier-space-noop", "We use OAuth2 here.", "We use OAuth 2 here.", false, false),
    ("negation", "She must not send it.", "She must send it.", false, true),
    ("month-name", "Het start op 3 mei 2025.", "Het start op 3 juni 2025.", false, true),
    ("name-control", "Alice reviewed the file.", "Alicia reviewed the file.", true, false),
    ("number-control", "The task starts at 09:30.", "The task starts at 10:30.", true, false),
    ("url-control", "See https://example.test/a.", "See https://example.test/b.", true, false)
})
{
    var a = ModelCorrectionEngine.AnalyzeModelResponse(probe.Item3, probe.Item2);
    Require((a.RejectionStage != null) == probe.Item4 && (a.Edits.Count > 0) == probe.Item5, "Safety observation: " + probe.Item1);
    observations.Add(new { id = probe.Item1, input = probe.Item2, candidate = probe.Item3,
        accepted = Apply(probe.Item2, a.Edits), analysis = a });
}
File.WriteAllText(args[1], JsonSerializer.Serialize(new { status = "Current behavior observations, not fixes or future acceptance criteria", observations }, options) + "\n");
