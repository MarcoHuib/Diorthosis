# Architecture: initial proposal

**Status:** proposed; no implementation or dependency has been approved by benchmark results yet.

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

- **Main application:** C# with self-contained .NET publishing; investigate Native AOT, compatibility, and package size on each target architecture. No external runtime installation for the end user.
- **Inference:** a pluggable adapter. `llama.cpp` is a candidate, not a precondition. Model loading must not start a public/local listening service. Cross-platform native libraries should be bundled where needed.
- **Models:** benchmark legally redistributable, European-origin candidates for Dutch and English. Verify exact checkpoint and license for the weights, tokenizer and related artifacts. Do not pick a default from parameter count or model marketing alone.
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

Prepare independent artifacts for Windows, macOS, and Linux; target x64/ARM64 wherever validated. A Windows `.exe` is not executable natively on macOS/Linux. macOS signing/notarization and Linux libc compatibility are release considerations, not reasons to add an installer prerequisite.

Use reproducible, pinned builds and publish an SBOM/license manifest when dependencies are actually included. Model weights may be distributed separately from binaries if it simplifies updates, provided the resulting end-user package remains offline and self-contained.
