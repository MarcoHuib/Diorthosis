# ADR 0004: In-process llama.cpp binding for local correction

- **Status:** Accepted for implementation; platform/model validation pending
- **Date:** 2026-10-19

## Context

The correction milestone requires an in-process CPU GGUF backend, Native AOT, six target RIDs, and no model download or external service. Direct P/Invoke offers a small managed surface but needs an owned native build/package pipeline and tight C ABI version control. LLamaSharp is an existing C# binding over llama.cpp with a separately packaged CPU backend.

## Decision

- Pin `LLamaSharp` and `LLamaSharp.Backend.Cpu` at 0.27.0. The NuGet package metadata identifies MIT licensing. Select CPU explicitly with `GpuLayerCount = 0`; load a caller-supplied file path.
- Use the binding rather than hand-maintained P/Invoke. No GPU backend or runner process is included.
- Use a versioned JSON edit prompt and independently validate model-provided spans against the UTF-16 source. Reject malformed, overlapping, out-of-range, changed-source, and numeric-content-changing edits.
- Model weights remain separate and are never fetched by the application.

## Validation and limitations

- Local .NET 10 compile/tests, native CPU backend load, and Native AOT `osx-arm64` publish and launch passed. LLamaSharp's default resolver triggers IL3000 because `Assembly.Location` is empty under AOT; Diorthosis configures the native search directory from `AppContext.BaseDirectory` and suppresses only IL3000 with the reason documented in both project files. CI now executes a backend-load check for each RID. Other RIDs remain unvalidated until CI runs.
- No GGUF weights were locally available, so neither candidate's model load, chat template, text-only path, correction quality, latency, nor memory use was tested.
- LLamaSharp's CPU backend NuGet package contains native libraries for multiple RIDs, including an unused `libmtmd` library. The upstream llama.cpp commit mapping is recorded in `THIRD_PARTY_NOTICES.md`; complete redistribution notices and unnecessary asset removal must be settled before shipping release artifacts.
- Peak native memory and GGUF embedded revision/quantization metadata are not currently emitted by the benchmark report.

## Rejected alternative

Direct P/Invoke to llama.cpp was not chosen because it adds native packaging and ABI maintenance with no advantage established by a working Native AOT prototype. Revisit if binding/AOT or model compatibility fails in the full CI/model validation.

## Primary sources

- [LLamaSharp repository and MIT license](https://github.com/SciSharp/LLamaSharp)
- [LLamaSharp 0.27.0](https://www.nuget.org/packages/LLamaSharp/0.27.0)
- [LLamaSharp CPU backend 0.27.0](https://www.nuget.org/packages/LLamaSharp.Backend.Cpu/0.27.0)
- [llama.cpp upstream](https://github.com/ggml-org/llama.cpp)
- [Ministral official GGUF page](https://huggingface.co/mistralai/Ministral-3-3B-Instruct-2512-GGUF)
- [EuroLLM upstream model card](https://huggingface.co/utter-project/EuroLLM-1.7B-Instruct)
