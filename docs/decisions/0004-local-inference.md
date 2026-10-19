# ADR 0004: In-process llama.cpp binding for local correction

- **Status:** Accepted; macOS ARM64 model/AOT path validated, quality and other-platform validation pending
- **Date:** 2026-10-19

## Context

The correction milestone requires an in-process CPU GGUF backend, Native AOT, six target RIDs, and no model download or external service. Direct P/Invoke offers a small managed surface but needs an owned native build/package pipeline and tight C ABI version control. LLamaSharp is an existing C# binding over llama.cpp with a separately packaged CPU backend.

## Decision

- Pin `LLamaSharp` and `LLamaSharp.Backend.Cpu` at 0.27.0. The NuGet package metadata identifies MIT licensing. Select CPU explicitly with `GpuLayerCount = 0`; load a caller-supplied file path.
- Use the binding rather than hand-maintained P/Invoke. No GPU backend or runner process is included.
- Use a versioned JSON edit prompt and independently validate model-provided spans against the UTF-16 source. Reject malformed, overlapping, out-of-range, changed-source, and numeric-content-changing edits.
- Model weights remain separate and are never fetched by the application.

## Validation and limitations

- Local deterministic tests pass. Native AOT `osx-arm64` publish and actual model correction succeeded for both models with network access denied and `DOTNET_ROOT` pointing at a nonexistent directory. All model layers and KV cache were logged on CPU; the bundled Metal backend had a zero-sized compute buffer. LLamaSharp's default resolver triggers IL3000 because `Assembly.Location` is empty under AOT; Diorthosis configures the native search directory from `AppContext.BaseDirectory` and suppresses only IL3000. CI retains six RID native build/start checks. Other RIDs remain unvalidated with real weights.
- Ministral Q4_K_M and a reproducible EuroLLM Q4_K_M conversion load and generate. Their 44-case results are recorded in `docs/model-validation.md`, but the cases have unreviewed annotations and are not a quality benchmark. The 600-case synthetic candidate needs independent review before held-out measurement.
- LLamaSharp's CPU backend NuGet package contains native libraries for multiple RIDs, including an unused `libmtmd` library. The upstream llama.cpp commit mapping is recorded in `THIRD_PARTY_NOTICES.md`; complete redistribution notices and unnecessary asset removal must be settled before shipping release artifacts.
- Benchmark process working-set peak is sampled at 100 ms intervals and can miss short-lived peaks. The observed process-level model-load time used a warmable OS file cache; true cold-boot startup is not measured. Native dependency redistribution notices remain a release task.

## Rejected alternative

Direct P/Invoke to llama.cpp was not chosen because it adds native packaging and ABI maintenance with no advantage established by a working Native AOT prototype. Revisit if binding/AOT or model compatibility fails in the full CI/model validation.

## Primary sources

- [LLamaSharp repository and MIT license](https://github.com/SciSharp/LLamaSharp)
- [LLamaSharp 0.27.0](https://www.nuget.org/packages/LLamaSharp/0.27.0)
- [LLamaSharp CPU backend 0.27.0](https://www.nuget.org/packages/LLamaSharp.Backend.Cpu/0.27.0)
- [llama.cpp upstream](https://github.com/ggml-org/llama.cpp)
- [Ministral official GGUF page](https://huggingface.co/mistralai/Ministral-3-3B-Instruct-2512-GGUF)
- [EuroLLM upstream model card](https://huggingface.co/utter-project/EuroLLM-1.7B-Instruct)
