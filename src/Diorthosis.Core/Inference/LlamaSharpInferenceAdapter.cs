using LLama;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;

namespace Diorthosis.Core.Inference;

/// <summary>In-process CPU inference over an explicitly selected local GGUF file.</summary>
public sealed class LlamaSharpInferenceAdapter : IModelInferenceAdapter, IDisposable
{
    public const string RuntimeVersion = "LLamaSharp 0.27.0 / llama.cpp backend 0.27.0";
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly LLamaWeights _weights;
    private readonly StatelessExecutor _executor;
    private bool _disposed;

    /// <summary>Number of tokens sampled, including a sampled end-of-generation token.</summary>
    public int LastSampledTokenCount { get; private set; }

    public enum GenerationTermination { Unknown, EndOfGeneration, MaximumTokens, Cancelled }

    // Kept as a compatibility diagnostic for existing callers; completion decisions use InferenceResult.
    public GenerationTermination LastTermination { get; private set; }

    /// <summary>Initializes llama.cpp's native backend without loading a model.</summary>
    public static void ValidateNativeBackend()
    {
        ConfigureNativeLibraries();
        _ = new ModelParams("native-backend-probe.gguf");
    }

    public LlamaSharpInferenceAdapter(string modelPath, uint contextSize = 4096, int? threads = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(modelPath);
        if (!string.Equals(Path.GetExtension(modelPath), ".gguf", StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException("Model path must point to a local .gguf file.", nameof(modelPath));
        if (!File.Exists(modelPath)) throw new FileNotFoundException("The selected local GGUF model file was not found.");
        Span<byte> header = stackalloc byte[4];
        using (var file = File.OpenRead(modelPath))
            if (file.Read(header) != header.Length || !header.SequenceEqual("GGUF"u8))
                throw new InvalidDataException("The selected model file does not have a valid GGUF header.");
        if (contextSize < 512) throw new ArgumentOutOfRangeException(nameof(contextSize), "Context size must be at least 512 tokens.");
        if (threads is <= 0) throw new ArgumentOutOfRangeException(nameof(threads), "Thread count must be positive.");
        LLamaWeights? weights = null;
        try
        {
            ConfigureNativeLibraries();
            var parameters = new ModelParams(modelPath)
            {
                ContextSize = contextSize,
                GpuLayerCount = 0,
                Threads = threads ?? Math.Max(1, Environment.ProcessorCount - 1)
            };
            weights = LLamaWeights.LoadFromFile(parameters);
            _weights = weights;
            _executor = new StatelessExecutor(weights, parameters) { ApplyTemplate = true };
        }
        catch (Exception exception)
        {
            weights?.Dispose();
            if (exception is OutOfMemoryException) throw;
            throw new InvalidDataException("The model could not be loaded. Check that it is a supported text GGUF file and that the native CPU backend matches this platform.", exception);
        }
    }

    private static void ConfigureNativeLibraries()
    {
        var nativeDirectory = Path.Combine(AppContext.BaseDirectory, "runtimes", System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier, "native");
        if (Directory.Exists(nativeDirectory))
            NativeLibraryConfig.All.WithCuda(false).WithVulkan(false).WithSearchDirectory(nativeDirectory);
    }

    public async ValueTask<InferenceResult> GenerateAsync(Corrections.InferenceRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ObjectDisposedException.ThrowIf(_disposed, this);
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            LastTermination = GenerationTermination.Unknown;
            LastSampledTokenCount = 0;
            using var samplingPipeline = new TerminationTrackingGreedySamplingPipeline(_weights);
            var settings = new InferenceParams
            {
                MaxTokens = request.MaximumOutputTokens,
                SamplingPipeline = samplingPipeline
            };
            var output = new System.Text.StringBuilder();
            try
            {
                await foreach (var token in _executor.InferAsync(request.Prompt, settings, cancellationToken).ConfigureAwait(false))
                    output.Append(token);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                LastSampledTokenCount = samplingPipeline.SampledTokenCount;
                LastTermination = GenerationTermination.Cancelled;
                throw;
            }
            LastSampledTokenCount = samplingPipeline.SampledTokenCount;
            if (cancellationToken.IsCancellationRequested)
            {
                LastTermination = GenerationTermination.Cancelled;
                cancellationToken.ThrowIfCancellationRequested();
            }
            var termination = samplingPipeline.EndOfGenerationSampled
                ? Diorthosis.Core.Inference.GenerationTermination.EndOfGeneration
                : Diorthosis.Core.Inference.GenerationTermination.MaximumTokens;
            LastTermination = termination switch
            {
                Diorthosis.Core.Inference.GenerationTermination.EndOfGeneration => GenerationTermination.EndOfGeneration,
                Diorthosis.Core.Inference.GenerationTermination.MaximumTokens => GenerationTermination.MaximumTokens,
                _ => GenerationTermination.Unknown
            };
            return new InferenceResult(output.ToString(), termination, samplingPipeline.SampledTokenCount);
        }
        finally
        {
            _gate.Release();
        }
    }

    public void Dispose()
    {
        _gate.Wait();
        try
        {
            if (_disposed) return;
            _weights.Dispose();
            _disposed = true;
        }
        finally
        {
            _gate.Release();
        }
    }

    private sealed class TerminationTrackingGreedySamplingPipeline(LLamaWeights weights) : GreedySamplingPipeline
    {
        public int SampledTokenCount { get; private set; }
        public bool EndOfGenerationSampled { get; private set; }

        public override LLamaToken Sample(LLama.Native.SafeLLamaContextHandle context, int index)
        {
            var token = base.Sample(context, index);
            SampledTokenCount++;
            EndOfGenerationSampled = token.IsEndOfGeneration(weights.Vocab);
            return token;
        }
    }
}
