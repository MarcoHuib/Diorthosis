using System.Security.Cryptography;
using System.Text.Json;

namespace Diorthosis.Benchmarks;

internal static class DatasetCommands
{
    public static async Task<int> ExecuteAsync(string[] args, JsonSerializerOptions options)
    {
        if (args[0] == "audit-dataset")
        {
            if (args.Length is < 3 or > 4) throw new ArgumentException("audit-dataset <cases.jsonl> <NEW-output-directory> [similarity-threshold]");
            var threshold = args.Length == 4 ? double.Parse(args[3], System.Globalization.CultureInfo.InvariantCulture) : 0.85;
            var cases = ReadCases(args[1], options);
            var report = DatasetAudit.Analyze(cases, threshold);
            if (Directory.Exists(args[2]) || File.Exists(args[2])) throw new IOException("Audit destination already exists; choose a new versioned directory.");
            Directory.CreateDirectory(args[2]);
            await Write("audit.json", report);
            await Write("duplicates.json", report.Groups.Where(group => group.Occurrences > 1).ToArray());
            await Write("near-duplicates.json", report.NearDuplicates);
            await Write("annotation-consistency.json", new { issues = report.AnnotationIssues, disagreements = report.Groups.Where(group => !group.AnnotationsEquivalent).ToArray() });
            var derived = report.Groups.Select(group => new { id = group.Language.ToLowerInvariant() + "-" + group.InputSha256,
                language = group.Language, inputText = group.InputText, sourceIds = group.CaseIds, annotationVariants = group.AnnotationVariants,
                reviewStatus = "unreviewed", partition = "development-exposed", sourceFamily = (string?)null }).ToArray();
            await File.WriteAllLinesAsync(Path.Combine(args[2], "candidate-groups-1.0.0.jsonl"), derived.Select(item => JsonSerializer.Serialize(item, options)));
            await Write("id-map.json", report.Groups.SelectMany(group => group.CaseIds.Select(id => new { historicalId = id, derivedId = group.Language.ToLowerInvariant() + "-" + group.InputSha256 })).OrderBy(item => item.historicalId, StringComparer.Ordinal).ToArray());
            var hashes = Directory.GetFiles(args[2]).Order(StringComparer.Ordinal).ToDictionary(path => Path.GetFileName(path)!, path => Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))).ToLowerInvariant());
            await Write("manifest.json", new { contract = DatasetAudit.Version, datasetVersion = "candidate-groups-1.0.0", sourceFile = Path.GetFileName(args[1]),
                sourceSha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(args[1]))).ToLowerInvariant(),
                similarityThreshold = threshold, independentReview = false, sourceFamiliesKnown = false, files = hashes });
            Console.WriteLine($"{report.Records} records; {report.UniqueInputs} exact unique texts; {report.DuplicateRecords} duplicate records; {report.NearDuplicates.Count} diagnostic near pairs; {report.AnnotationIssues.Count} annotation issues.");
            return 0;
            Task Write(string name, object value) => File.WriteAllTextAsync(Path.Combine(args[2], name), JsonSerializer.Serialize(value, options) + "\n");
        }
        if (args.Length is < 2 or > 3) throw new ArgumentException("check-splits <split-manifest.json> [similarity-threshold]");
        var manifest = JsonSerializer.Deserialize<SplitFile[]>(await File.ReadAllTextAsync(args[1]), options) ?? throw new InvalidDataException("Split manifest is null.");
        var entries = manifest.SelectMany(part => ReadCases(Path.Combine(Path.GetDirectoryName(Path.GetFullPath(args[1]))!, part.Path), options)
            .Select(item => new SplitEntry(part.Partition, item, part.SourceFamilies?.GetValueOrDefault(item.Id)))).ToArray();
        var overlaps = DatasetAudit.CheckSplits(entries, args.Length == 3 ? double.Parse(args[2], System.Globalization.CultureInfo.InvariantCulture) : 0.85);
        Console.WriteLine(JsonSerializer.Serialize(new { version = DatasetAudit.Version, overlaps, sourceFamiliesComplete = entries.All(entry => !string.IsNullOrWhiteSpace(entry.SourceFamily)),
            limitation = "Absence of detected overlap does not establish unseen template-family independence." }, options));
        return overlaps.Count == 0 ? 0 : 1;
    }
    internal static BenchmarkCase[] ReadCases(string path, JsonSerializerOptions options) => File.ReadLines(path).Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => JsonSerializer.Deserialize<BenchmarkCase>(line, options) ?? throw new InvalidDataException("Null dataset record.")).ToArray();
    private sealed record SplitFile(string Partition, string Path, IReadOnlyDictionary<string, string>? SourceFamilies);
}
