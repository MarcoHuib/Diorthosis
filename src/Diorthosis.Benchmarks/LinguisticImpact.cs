using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;
using Diorthosis.Experiments;

namespace Diorthosis.Benchmarks;

internal static class LinguisticImpact
{
    public static async Task<int> RunAsync(string[] args,JsonSerializerOptions options)
    {
        if(args.Length!=4)throw new ArgumentException("linguistic-impact <panel.jsonl> <frozen-run-directory> <NEW-output-directory>");
        if(Directory.Exists(args[3])||File.Exists(args[3]))throw new IOException("Impact output directory must be new.");
        var cases=DatasetCommands.ReadCases(args[1],options);
        var inputRows=File.ReadLines(args[1]).Select(line=>JsonSerializer.Deserialize<JsonElement>(line,options)).ToDictionary(r=>r.GetProperty("id").GetString()!);
        var tracePath=Path.Combine(args[2],"trace.jsonl");
        var rows=File.ReadLines(tracePath).Select(line=>JsonSerializer.Deserialize<JsonElement>(line,options)).ToArray();
        using var manifest=JsonDocument.Parse(File.ReadAllText(Path.Combine(args[2],"manifest.json")));
        if(manifest.RootElement.GetProperty("state").GetString()!="complete"||manifest.RootElement.GetProperty("datasetSha256").GetString()!=Hash(args[1]))
            throw new InvalidDataException("Expected complete dataset-bound run.");
        var arms=new[]{"baseline","hints","sample-101","sample-202","sample-303","voting-3"};
        var map=rows.ToDictionary(r=>(Id:r.GetProperty("id").GetString()!,Arm:r.GetProperty("arm").GetString()!));
        if(map.Count!=cases.Length*arms.Length||map.Keys.Any(k=>!cases.Any(c=>c.Id==k.Id)||!arms.Contains(k.Arm)))throw new InvalidDataException("Impact replay coverage mismatch.");
        Directory.CreateDirectory(args[3]);
        var output=new List<object>();
        var before=arms.ToDictionary(a=>a,_=>new List<BenchmarkPrediction>());
        var after=arms.ToDictionary(a=>a,_=>new List<BenchmarkPrediction>());
        var beforeStates=arms.ToDictionary(a=>a,_=>new Dictionary<string,CorrectionDisposition>());
        var afterStates=arms.ToDictionary(a=>a,_=>new Dictionary<string,CorrectionDisposition>());
        using var process=Process.GetCurrentProcess();
        var watch=Stopwatch.StartNew();
        foreach(var item in cases)
        {
            var spans=inputRows[item.Id].TryGetProperty("protectedSpans",out var p)?JsonSerializer.Deserialize<ProtectedSpan[]>(p,options)??[]:[];
            var request=new CorrectionRequest(item.Language,item.InputText,spans);
            var samples=new List<VotingCandidate>();
            foreach(var arm in arms)
            {
                var row=map[(item.Id,arm)];
                if(row.GetProperty("input").GetString()!=request.Text)throw new InvalidDataException("Trace/source mismatch.");
                var original=JsonSerializer.Deserialize<ModelResponseAnalysis>(row.GetProperty("analysis"),options)!;
                ModelResponseAnalysis updated;
                if(arm=="voting-3")updated=EditVoting.Aggregate(request,samples).Analysis;
                else
                {
                    var inference=JsonSerializer.Deserialize<InferenceResult>(row.GetProperty("inference"),options)!;
                    updated=ModelCorrectionEngine.AnalyzeInference(inference,request);
                    if(arm.StartsWith("sample-",StringComparison.Ordinal))samples.Add(new(arm,inference));
                }
                var oldText=SourceEdits.Apply(request.Text,original.Edits);
                var newText=SourceEdits.Apply(request.Text,updated.Edits);
                before[arm].Add(new(item.Id,original.Edits));after[arm].Add(new(item.Id,updated.Edits));
                beforeStates[arm][item.Id]=original.Disposition;afterStates[arm][item.Id]=updated.Disposition;
                output.Add(new{caseId=item.Id,item.Language,arm,input=request.Text,before=original,after=updated,
                    beforeDelivered=oldText,afterDelivered=newText,deliveredChanged=oldText!=newText,dispositionChanged=original.Disposition!=updated.Disposition,
                    inference=row.TryGetProperty("inference",out var inf)?(JsonElement?)inf:null});
            }
        }
        watch.Stop();process.Refresh();
        await File.WriteAllLinesAsync(Path.Combine(args[3],"paired.jsonl"),output.Select(r=>JsonSerializer.Serialize(r,options)));
        foreach(var arm in arms)
        {
            await File.WriteAllTextAsync(Path.Combine(args[3],arm+"-before-metrics.json"),JsonSerializer.Serialize(ReferenceEvaluator.Evaluate(cases,before[arm],beforeStates[arm]),options));
            await File.WriteAllTextAsync(Path.Combine(args[3],arm+"-after-metrics.json"),JsonSerializer.Serialize(ReferenceEvaluator.Evaluate(cases,after[arm],afterStates[arm]),options));
        }
        await File.WriteAllTextAsync(Path.Combine(args[3],"manifest.json"),JsonSerializer.Serialize(new{kind="deterministic paired replay; zero new linguistic inference",
            datasetSha256=Hash(args[1]),sourceTraceSha256=Hash(tracePath),coreAssemblySha256=Hash(typeof(ModelCorrectionEngine).Assembly.Location),
            experimentalAssemblySha256=Hash(typeof(EditVoting).Assembly.Location),evaluator=ReferenceEvaluator.Version,voting=EditVoting.Version,
            cases=cases.Length,generatedCandidates=cases.Length*5,votedPassages=cases.Length,
            analysisElapsedMilliseconds=watch.Elapsed.TotalMilliseconds,processWorkingSetBytes=process.WorkingSet64,
            performanceLimitations="single process snapshot/replay, includes JIT; not model-inference latency or a paired performance benchmark"},options));
        return 0;
    }
    private static string Hash(string path){using var file=File.OpenRead(path);return Convert.ToHexString(SHA256.HashData(file)).ToLowerInvariant();}
}
