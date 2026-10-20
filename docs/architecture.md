# Architecture: Phase 1 foundation

**Status:** The existing Ministral 3B GGUF generated and corrected text on macOS ARM64 through the published Native AOT CLI when using llama.cpp rebuilt at the pinned revision with Metal disabled. The stock LLamaSharp 0.27.0 macOS package still fails here during Metal backend initialization; see [ADR 0006](decisions/0006-cpu-only-macos-inference-and-next-model-manifest.md) for the CPU-only build remedy and validation. The 44-case results are preliminary only; the 600-case candidate still needs independent human review and held-out model evaluation. Other platforms have not run real weights.

## Implemented scope

The solution targets .NET 10 and a CLI. Core owns correction types, a versioned correction prompt, edit validation, and an `IModelInferenceAdapter`. `LlamaSharpInferenceAdapter` uses pinned LLamaSharp 0.27.0 to load a user-selected local GGUF in-process with GPU layer offload disabled. On macOS ARM64, the stock native package also contains and initializes Metal; publish the custom Metal-disabled CPU backend from ADR 0006 to avoid that startup failure. The application makes no network request and does not download weights. The benchmark project can run cases and pass structured predictions to the existing evaluator.

The baseline uses in-process CPU inference. Model weights are a separate local file and are never downloaded by the application. No service or listening port is part of the design.

## Preferred small model candidates

1. **Primary:** [`mistralai/Ministral-3-3B-Instruct-2512-GGUF`](https://huggingface.co/mistralai/Ministral-3-3B-Instruct-2512-GGUF). The official Q4_K_M text GGUF (2,147,023,008 bytes) loaded and ran on CPU. No separate vision projector was downloaded or used. See [`model-validation.md`](model-validation.md) for revision, checksum, smoke outcomes and limitations.
2. **Secondary:** [`utter-project/EuroLLM-1.7B-Instruct`](https://huggingface.co/utter-project/EuroLLM-1.7B-Instruct). The official checkpoint was converted at the pinned llama.cpp revision to Q4_K_M (1,045,157,024 bytes); it loaded and ran on CPU. See [`model-validation.md`](model-validation.md) for conversion, chat-template, checksum, smoke outcomes and limitations.

The macOS ARM64 LLamaSharp 0.27.0 CPU package initializes its compiled Metal backend even with zero GPU layers. The current remedy and bounded next-candidate manifest are recorded in [ADR 0006](decisions/0006-cpu-only-macos-inference-and-next-model-manifest.md). Do not treat candidate metadata or smoke diagnostics as model-quality approval.

EuroLLM-9B and Mistral Small 3.1 24B are optional quality-reference candidates, not the lightweight baseline. The older Ministral 8B 2410 research-licensed checkpoint is distinct from the Apache-licensed Ministral 3 family and is not a preferred candidate.

These license/origin statements record upstream disclosures, not an independent supply-chain or training-data audit. Exact immutable revisions, file checksums and Apache-2.0 declarations are recorded in [`model-validation.md`](model-validation.md). `PromptTemplateTransformer` uses the GGUF-embedded chat template; real generation succeeded for both models. The app does not download weights; local assets are not stored in Git.

## Intended processing flow

```text
Local document (.txt, .docx, .doc)
  -> Format-specific reader
  -> Structured text segments and source positions
  -> Offline language selection / detection
  -> Local model inference (replaceable engine)
  -> Structured correction proposals
  -> Deterministic safety and integrity checks
  -> Review report
  -> Explicitly approved export to a NEW file
```

## Candidate technology

- **Main application:** C# / .NET 10, CLI and Native AOT publishing, with platform-specific self-contained artifacts. End users should not need the .NET runtime.
- **Inference:** LLamaSharp 0.27.0 with `LLamaSharp.Backend.Cpu` 0.27.0 (MIT) is pinned. CPU selection is explicit (`GpuLayerCount = 0`). Native AOT publication and real in-process inference with both models succeeded on macOS ARM64, with all model layers/KV cache on CPU and zero Metal compute buffer. LLamaSharp initializes its bundled Metal backend, but no model layers are assigned to it. LLamaSharp's IL3000 warning about its fallback `Assembly.Location` resolver is suppressed narrowly; Diorthosis sets a runtime-specific native search directory using `AppContext.BaseDirectory`. CI has six RID build/start checks; other platforms have not been tested with real weights. Native dependency redistribution notices remain to be completed before releases.
- **Models:** prioritize the small Ministral 3 3B GGUF and EuroLLM 1.7B candidates above; benchmark evidence still determines suitability.
- **Office documents:** prefer cross-platform, no-Java, commercially permissive components. `.docx` is a ZIP/OpenXML format; `.doc` is a separate legacy binary format and requires dedicated parsing or conversion. Investigate extraction, tracked changes, embedded objects, section layout and round-trip fidelity independently.
- **Spelling libraries:** optional. Start AI-only, then use benchmark evidence to decide whether supplementary detection improves quality enough to justify its footprint and license obligations.

All choices must undergo an explicit license and supply-chain audit before release.

## Interfaces to aim for

- `IDocumentReader`: load supported documents into segments with formatting/source mapping metadata.
- `ILanguageSelector`: return selected or detected language with an explicit override.
- `ICorrectionEngine`: submit local text and receive suggested corrections with source spans or carefully validated replacement text.
- `ICorrectionValidator`: reject unsafe, unverifiable, or out-of-range modifications.
- `IReviewReportWriter`: generate a local human-readable diff with accept/reject choices (the UI format is open).
- `IDocumentWriter`: write to a new destination, preserving validated formatting where supported.

These are conceptual contracts, not required method or project names.

Implemented contracts are `ICorrectionEngine` and `IModelInferenceAdapter` in Core, with a model-backed correction engine and validated UTF-16 edits. Document contracts and presentation/export abstractions are deferred until document work is in scope.

## Security and privacy design

- Strictly local processing. No telemetry, analytics, network requests, automatic model fetching, or document content logging.
- No externally reachable inference port. Prefer in-process model invocation.
- Avoid leaking text in error messages, temporary files, swap-sensitive debug dumps, or CI artifacts. Define secure cleanup and reasonable failure behavior.
- Treat input files and model files as untrusted. Apply parsing limits; validate native library dependencies and model checksums.
- Verify offline behavior using a network-isolated test. A description or configuration toggle is not evidence.
- Enterprise use still requires the organization's approval; offline processing alone does not guarantee compliance with internal security rules.

## Document fidelity policy

1. Never modify or overwrite the original.
2. Review-only reports are always an acceptable fallback when safe round-trip editing is not supported.
3. For `.docx`, preserve structure, styles, tables, headers/footers and non-text content where possible; measure round-trip fidelity with representative documents.
4. For `.doc`, do not silently convert to `.docx` or promise identical layout without explicit consent and evidence.
5. A text-only source must not be presented as a fully format-preserving Word export.

## Cross-platform packaging

The existing six-RID CI matrix now also loads the native CPU backend on matching OS/architecture runners: Windows x64/ARM64, macOS x64/ARM64, and Linux x64/ARM64. The launch overrides `DOTNET_ROOT` to a nonexistent path. Linux x64 traces network syscalls during CLI startup and native backend initialization. These checks do not execute a model or prove the real inference path is offline. The updated matrix has not run yet.

Prepare independent artifacts for Windows, macOS, and Linux; target x64/ARM64 wherever validated. A Windows `.exe` is not executable natively on macOS/Linux. macOS signing/notarization and Linux libc compatibility are release considerations, not reasons to add an installer prerequisite.

Use reproducible, pinned builds and publish an SBOM/license manifest when dependencies are actually included. Model weights may be distributed separately from binaries if it simplifies updates, provided the resulting end-user package remains offline and self-contained.

## Correction quality checkpoint

See [ADR 0011](decisions/0011-observable-conservative-correction-results.md) for observable result states, source/grapheme checks and conservative scope review; [ADR 0012](decisions/0012-opt-in-prompt-and-edit-experiments.md) for isolated prompt/edit/grammar research. Default prompt/runtime remain pinned. [Context investigation](document-context-investigation.md) adds target-offset validation only, without document parsing or export. Baseline CI at `968f4dc` passed all six publish/native-start jobs; the earlier statement that the matrix had not run is superseded for that commit only.
