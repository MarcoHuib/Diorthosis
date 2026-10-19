using System.Text.Json;
using System.Text.Json.Serialization;
using Diorthosis.Benchmarks;

if (args.Length != 3 || args[0] != "evaluate")
{
    Console.Error.WriteLine("Usage: diorthosis-benchmarks evaluate <cases.jsonl> <predictions.jsonl>");
    return 2;
}

try
{
var options = new JsonSerializerOptions(JsonSerializerDefaults.Web);
options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
    var cases = File.ReadLines(args[1]).Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => JsonSerializer.Deserialize<BenchmarkCase>(line, options)
            ?? throw new InvalidDataException("Benchmark case line is null.")).ToArray();
    var predictions = File.ReadLines(args[2]).Where(line => !string.IsNullOrWhiteSpace(line))
        .Select(line => JsonSerializer.Deserialize<BenchmarkPrediction>(line, options)
            ?? throw new InvalidDataException("Prediction line is null.")).ToArray();
    Console.WriteLine(JsonSerializer.Serialize(BenchmarkEvaluator.Evaluate(cases, predictions), options));
    return 0;
}
catch (Exception exception) when (exception is IOException or JsonException or ArgumentException or InvalidDataException)
{
    Console.Error.WriteLine(exception.Message);
    return 1;
}
