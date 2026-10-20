using System.Text.Json;

namespace Diorthosis.Benchmarks;

public sealed record ReplayBinding(bool DatasetVerified, bool TraceBoundToRun, string ModelIdentityStatus);

public static class BenchmarkArtifactPolicy
{
    public static void RequireNewOutputs(params string[] paths)
    {
        if (paths.Any(path => File.Exists(path) || Directory.Exists(path))
            || paths.Select(Path.GetFullPath).Distinct(OperatingSystem.IsWindows() || OperatingSystem.IsMacOS() ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal).Count() != paths.Length)
            throw new IOException("Benchmark outputs must be distinct new paths; historical artifacts cannot be overwritten.");
    }

    public static async Task WriteTextNewAsync(string path, string text)
    {
        await using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await using var writer = new StreamWriter(stream);
        await writer.WriteAsync(text);
    }

    public static async Task WriteLinesNewAsync(string path, IEnumerable<string> lines)
    {
        await using var stream = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None);
        await using var writer = new StreamWriter(stream);
        foreach (var line in lines) await writer.WriteLineAsync(line);
    }

    public static ReplayBinding ValidateReplay(JsonElement report, string actualDatasetSha256, int cases, string actualTraceSha256)
    {
        if (report.ValueKind != JsonValueKind.Object || !report.TryGetProperty("datasetSha256", out var dataset)
            || dataset.ValueKind != JsonValueKind.String || dataset.GetString() != actualDatasetSha256)
            throw new InvalidDataException("Original report does not match the replay dataset checksum.");
        if (!report.TryGetProperty("caseCount", out var count) || count.ValueKind != JsonValueKind.Number || !count.TryGetInt32(out var value) || value != cases)
            throw new InvalidDataException("Original report does not match the replay case count.");
        var bound = report.TryGetProperty("traceSha256", out var trace) && trace.ValueKind != JsonValueKind.Null;
        if (bound && (trace.ValueKind != JsonValueKind.String || trace.GetString() != actualTraceSha256))
            throw new InvalidDataException("Original report does not match the captured trace checksum.");
        return new(true, bound, bound ? "declared source-run identity; trace checksum matches" : "unverified legacy run identity; source report retained separately");
    }
}
