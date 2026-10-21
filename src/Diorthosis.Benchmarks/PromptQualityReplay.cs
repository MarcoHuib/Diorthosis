using System.Security.Cryptography;
using System.Text.Json;
using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;
using Diorthosis.Experiments;

namespace Diorthosis.Benchmarks;

internal static class PromptQualityReplay
{
    public static async Task<int> RunAsync(string[] args, JsonSerializerOptions options)
    {
        if (args.Length != 4) throw new ArgumentException("prompt-quality-replay <panel.jsonl> <complete-run-directory> <NEW-report.json>");
        BenchmarkArtifactPolicy.RequireNewOutputs(args[3]);
        using var manifest = JsonDocument.Parse(File.ReadAllText(Path.Combine(args[2], "manifest.json")));
        var metadata = manifest.RootElement;
        if (metadata.GetProperty("state").GetString() != "complete" || metadata.GetProperty("datasetSha256").GetString() != Hash(args[1]))
            throw new InvalidDataException("Replay requires complete, dataset-bound evidence.");
        var cases = DatasetCommands.ReadCases(args[1], options).ToDictionary(c => c.Id, StringComparer.Ordinal);
        var arms = metadata.GetProperty("arms").EnumerateArray().Select(a => a.GetProperty("promptVersion").GetString()!).ToHashSet(StringComparer.Ordinal);
        var seen = new HashSet<(string, string)>(); var mismatches = new List<string>();
        var tracePath = Path.Combine(args[2], "trace.jsonl");
        foreach (var line in File.ReadLines(tracePath))
        {
            using var parsed = JsonDocument.Parse(line); var row = parsed.RootElement;
            var id = row.GetProperty("id").GetString()!; var version = row.GetProperty("promptVersion").GetString()!;
            if (!cases.TryGetValue(id, out var item) || !arms.Contains(version) || !seen.Add((id, version)) || row.GetProperty("input").GetString() != item.InputText)
                throw new InvalidDataException("Duplicate, unknown or source-mismatched attempt.");
            var request = new CorrectionRequest(item.Language, item.InputText);
            var representation = row.GetProperty("representation").Deserialize<InputRepresentation>(options);
            var instruction = row.GetProperty("instruction").Deserialize<ProofreadingInstruction>(options);
            var prompt = PromptQuality.Build(request, representation, instruction);
            if (PromptQuality.Version(representation, instruction) != version || prompt != row.GetProperty("prompt").GetString()
                || DatasetAudit.Hash(prompt) != row.GetProperty("promptSha256").GetString()) mismatches.Add(id + "/" + version + "/prompt");
            ModelResponseAnalysis analysis;
            if (row.GetProperty("inference").ValueKind == JsonValueKind.Null)
            {
                var failure = row.GetProperty("failure").GetString();
                if (string.IsNullOrWhiteSpace(failure)) throw new InvalidDataException("Unrecorded failed attempt.");
                analysis = ModelResponseAnalysis.Rejected("", null, [], "inference-failure", failure);
            }
            else
            {
                var inference = row.GetProperty("inference").Deserialize<InferenceResult>(options)!;
                analysis = ModelCorrectionEngine.AnalyzeInference(inference, request);
                if (row.GetProperty("generationTermination").GetString() != inference.Termination.ToString()
                    || row.GetProperty("sampledTokens").GetInt32() != inference.SampledTokens) mismatches.Add(id + "/" + version + "/termination");
            }
            if (JsonSerializer.Serialize(analysis, options) != row.GetProperty("analysis").GetRawText()) mismatches.Add(id + "/" + version + "/analysis");
            if (SourceEdits.Apply(item.InputText, analysis.Edits) != row.GetProperty("delivered").GetString()) mismatches.Add(id + "/" + version + "/delivery");
        }
        if (seen.Count != cases.Count * arms.Count || seen.Count != metadata.GetProperty("attempts").GetInt32()) throw new InvalidDataException("Incomplete attempt coverage.");
        await BenchmarkArtifactPolicy.WriteTextNewAsync(args[3], JsonSerializer.Serialize(new { version = "phase4-replay-1.0.0", cases = cases.Count, attempts = seen.Count,
            datasetSha256 = Hash(args[1]), traceSha256 = Hash(tracePath), sourceManifestSha256 = Hash(Path.Combine(args[2], "manifest.json")),
            replayAssemblySha256 = Hash(typeof(PromptQualityReplay).Assembly.Location), coreAssemblySha256 = Hash(typeof(ModelCorrectionEngine).Assembly.Location),
            experimentalAssemblySha256 = Hash(typeof(PromptQuality).Assembly.Location), mismatches,
            interpretation = "Deterministic replay of retained literal prompts, terminations, extraction, validation and delivery. Not fresh inference, model performance measurement or independent linguistic assessment." }, options));
        return mismatches.Count == 0 ? 0 : 1;
    }
    private static string Hash(string path) { using var file = File.OpenRead(path); return Convert.ToHexString(SHA256.HashData(file)).ToLowerInvariant(); }
}
