# Architecture: Phase 1 foundation

**Status:** Phase 1 foundation implemented; inference and document processing remain unimplemented.

## Phase 1 boundaries

The solution targets .NET 10 and a CLI. The Core project owns correction request/result types and the correction and inference adapter contracts. The benchmark project evaluates offline JSONL cases against structured edits. No model runner, model weight, document parser, grammar engine, or GPU dependency is included.

The baseline target is CPU inference with an in-process native adapter in a later phase. Model weights are a separate local file and are never downloaded by the application. No service or listening port is part of the design.

## Preferred small model candidates

1. **Primary:** [`mistralai/Ministral-3-3B-Instruct-2512-GGUF`](https://huggingface.co/mistralai/Ministral-3-3B-Instruct-2512-GGUF). The official model page lists Apache 2.0, English and Dutch, an official llama.cpp path, and Q4_K_M at 2.15 GB. It includes a vision encoder, but Phase 1 evaluates text correction only. Model quality, CPU memory/latency and required runtime treatment of the vision components remain to be tested.
2. **Secondary:** [`utter-project/EuroLLM-1.7B-Instruct`](https://huggingface.co/utter-project/EuroLLM-1.7B-Instruct). Its model card lists 1.7B parameters, Dutch and English, Apache 2.0, and European Union funding with a European research consortium. GGUF conversion and llama.cpp compatibility require validation before use.

EuroLLM-9B and Mistral Small 3.1 24B are optional quality-reference candidates, not the lightweight baseline. The older Ministral 8B 2410 research-licensed checkpoint is distinct from the Apache-licensed Ministral 3 family and is not a preferred candidate.

These license/origin statements record upstream model card disclosures, not an independent supply-chain or training-data audit. Do not download, bundle, or claim a model is approved until exact revisions, weights, tokenizer, notices, and redistribution obligations have been reviewed. No weights are stored in this repository.

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
- **Inference:** `IModelInferenceAdapter` contract only in Phase 1. Evaluate direct in-process llama.cpp integration for CPU first; do not introduce an external process, local server, GPU package, or runtime model download in the baseline.
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

Implemented contracts are deliberately narrower: `ICorrectionEngine` and `IModelInferenceAdapter` in Core. Document contracts and the remaining presentation/validation abstractions are deferred until document work is in scope.

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

Phase 1 CI publishes and launches the CLI on a matching OS/architecture runner for all six target RIDs. The launch overrides `DOTNET_ROOT` to a nonexistent path; Linux x64 also traces network syscalls during `--version` startup. This verifies only the current CLI startup path. It does not prove model inference is offline or verify behavior of an installed release on every supported OS. The local Native AOT publish and launch has been checked on macOS ARM64; the other five targets remain pending CI results.

Prepare independent artifacts for Windows, macOS, and Linux; target x64/ARM64 wherever validated. A Windows `.exe` is not executable natively on macOS/Linux. macOS signing/notarization and Linux libc compatibility are release considerations, not reasons to add an installer prerequisite.

Use reproducible, pinned builds and publish an SBOM/license manifest when dependencies are actually included. Model weights may be distributed separately from binaries if it simplifies updates, provided the resulting end-user package remains offline and self-contained.
