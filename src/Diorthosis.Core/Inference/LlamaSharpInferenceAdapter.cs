using LLama;
using LLama.Common;
using LLama.Native;
using LLama.Sampling;
using LLama.Transformers;

namespace Diorthosis.Core.Inference;

/// <summary>In-process CPU inference over an explicitly selected local GGUF file.</summary>
public sealed class LlamaSharpInferenceAdapter : IModelInferenceAdapter, IDisposable
{
    public const string RuntimeVersion = "LLamaSharp 0.27.0 / llama.cpp backend 0.27.0";
    private readonly SemaphoreSlim _gate = new(1, 1);
    private readonly LLamaWeights _weights;
    private readonly LLamaContext _context;
    private readonly InteractiveExecutor _executor;
    private bool _disposed;

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
        LLamaContext? context = null;
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
            context = weights.CreateContext(parameters);
            _weights = weights;
            _context = context;
            _executor = new InteractiveExecutor(_context);
        }
        catch (Exception exception) when (exception is not OutOfMemoryException)
        {
            context?.Dispose();
            weights?.Dispose();
            throw new InvalidDataException("The model could not be loaded. Check that it is a supported text GGUF file and that the native CPU backend matches this platform.", exception);
        }
    }

    private static void ConfigureNativeLibraries()
    {
        var nativeDirectory = Path.Combine(AppContext.BaseDirectory, "runtimes", System.Runtime.InteropServices.RuntimeInformation.RuntimeIdentifier, "native");
        if (Directory.Exists(nativeDirectory))
            NativeLibraryConfig.All.WithCuda(false).WithVulkan(false).WithSearchDirectory(nativeDirectory);
    }

    public async ValueTask<string> GenerateAsync(Corrections.InferenceRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        ObjectDisposedException.ThrowIf(_disposed, this);
        await _gate.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            ObjectDisposedException.ThrowIf(_disposed, this);
            var session = new ChatSession(_executor);
            session.WithHistoryTransform(new PromptTemplateTransformer(_weights, withAssistant: true));
            var settings = new InferenceParams
            {
                MaxTokens = request.MaximumOutputTokens,
                SamplingPipeline = new DefaultSamplingPipeline { Temperature = 0.0f }
            };
            var output = new System.Text.StringBuilder();
            await foreach (var token in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, request.Prompt), settings, cancellationToken).ConfigureAwait(false))
                output.Append(token);
            return output.ToString().Trim();
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
            _context.Dispose();
            _weights.Dispose();
            _disposed = true;
        }
        finally
        {
            _gate.Release();
        }
    }
}
