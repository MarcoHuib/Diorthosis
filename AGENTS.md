# Diorthosis: instructions for coding agents

This file is the single source of truth for durable project requirements and agent working rules. Read it before planning, coding, or reviewing a change. Read relevant files under `docs/` as well. If a task conflicts with a non-negotiable rule, stop and explain the conflict; do not silently weaken the rule.

## Purpose

Build **Diorthosis**, a small, portable, privacy-first writing assistant that corrects Dutch and English spelling, grammar, and sentence structure in local documents, while preserving their meaning.

## Non-negotiable product requirements

1. **Offline by design.** Input documents, prompts, results, and model inference stay on the device. No cloud inference, external APIs, telemetry, phone-home checks, automatic downloads, or listening network services. An offline test must verify this.
2. **Cross-platform.** Support Windows, macOS, and Linux. Target x64 and ARM64 where supported by the selected toolchain and verified by tests. Ship platform-specific artifacts built from a shared codebase.
3. **Zero prerequisite installation for end users.** Do not require installing Python, Java/JRE/JDK, .NET, Docker, Ollama, office suites, model servers, or package managers. Bundle necessary permissively licensed native dependencies. In particular, **do not introduce Java as a runtime dependency**, whether installed separately or bundled.
4. **Open source and commercially usable.** Application source and redistributed dependencies, model weights, tokenizers, dictionaries, and native binaries must have licenses permitting modification, redistribution, and commercial use. Review licenses, notices, provenance, and redistribution obligations before adding them. Do not assume "open weights" implies a fully open-source model.
5. **European-origin AI models only.** Select models originating from Europe; do not use models originating from Russia or China. Verify provenance, license, and supply-chain details rather than relying on marketing labels. European origin of an inference engine is not equivalent to model origin.
6. **Languages.** Dutch (nl) and English (en); do not assume auto-detection is reliable until tested. Users must be able to force a language.
7. **Documents.** Read `.txt`, `.docx`, and legacy binary `.doc` documents. Distinguish plain text extraction from reliable, format-preserving editing. Never claim `.doc` round-trip support without proving it on representative real-world files.
8. **Safety of edits.** Never overwrite the source document. Default to reviewable correction proposals. Do not deliberately change meaning, names, numbers, dates, URLs, legal wording, or technical identifiers. Use programmatic safeguards; a model prompt alone is insufficient.
9. **Resource-conscious distribution.** Prefer CPU-only baseline support and optional acceleration. No mandatory background service. Model sizes and memory requirements must be measured, not guessed.
10. **No automatic PR merges.** Changes require tests, review, and an explicit human merge decision.

## Quality gates

- **Primary candidate acceptance target:** at least 90% of annotated spelling and grammar errors correctly repaired, measured separately for Dutch and English on a held-out benchmark.
- Measure correction precision, false positives on already-correct text, meaning preservation, document fidelity, latency, memory usage, and offline behavior independently. Current *proposed* targets and metric definitions are in `docs/benchmarks.md`.
- Do not equate text similarity with correctness. Correct alternatives may differ from reference wording. Use structured annotations plus human review for disputed cases.
- Never state a model or release has met a target without a reproducible report showing the model revision, quantization, hardware, test-set revision, and results.

## Architecture direction (candidates, not settled dependencies)

- C# / .NET and self-contained or Native AOT publication for the app.
- A swappable inference adapter; evaluate `llama.cpp` with suitable European models. The model and runner are **independent** components. Neither is mandatory before benchmarks.
- Separate interfaces for reading documents, finding corrections, validating changes, presenting proposals, and exporting files.
- Start by comparing AI-only correction against the stated quality gates. Add a dedicated spelling engine only if measurements justify it. Do not introduce Java-based LanguageTool.
- Investigate `.doc` parsing independently and test realistic, complex samples before selecting a library. Confirm licensing for both parser and transitive dependencies.
- Model candidates must pass a documented provenance and license review before being downloaded, bundled, or adopted. No candidate is currently the official model.

See `docs/architecture.md` and `docs/decisions/` for design context.

## Agent workflow

1. Inspect existing repository state, open issues, and relevant docs before proposing changes.
2. For nontrivial work, produce a short plan with scope, alternatives, risks, and objective acceptance criteria.
3. Keep changes scoped and incremental; prefer small, testable components. Do not implement an entire roadmap in one unreviewable change.
4. Add and run targeted tests, then the relevant broader suite. Clearly report unrun checks and why.
5. Record consequential technical decisions in `docs/decisions/`, including rejected options and evidence.
6. Update documentation when behavior, constraints, licensing, or architecture changes.
7. Preserve licenses and copyright notices in distributed artifacts; update `THIRD_PARTY_NOTICES.md` as real dependencies are selected.
8. Keep benchmark fixtures non-confidential and rights-cleared. Never commit real employer documents, confidential excerpts, personal information, or private model data.
9. Never fabricate benchmarks, performance figures, license approvals, or source evidence. Mark unresolved questions explicitly.
10. Do not auto-merge, auto-release, or modify repository protection settings.

## Initial development priorities

1. Verify legal/provenance and technical feasibility of a short list of European models and cross-platform inference runtimes.
2. Build an isolated, reproducible Dutch/English benchmark harness; test a baseline AI-only pipeline.
3. Select runtime/model only after benchmark evidence.
4. Validate document parsing/export fidelity, especially for legacy `.doc` files.
5. Deliver portable CLI builds and offline/security tests for each supported platform.

See `docs/roadmap.md` for milestone details.
