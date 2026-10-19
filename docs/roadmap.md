# Roadmap

**Status:** proposal for review; milestone order may change when evidence warrants it.

## Phase 0: repository and legal foundation

- [x] Establish conventions, contribution checks, automated tests and CI foundation.
- [ ] Define permissive dependency/license/provenance requirements and reporting.
- [ ] Investigate European-origin candidates and exact license terms for models, weights, tokenizers and data.
- [ ] Establish reproducible builds for target operating systems.

**Exit:** a reviewed candidate shortlist and build strategy, with no unverified licensing assertions.

## Phase 1: AI quality benchmark (before major document work)

- [x] Define the Dutch/English test schema, smoke fixtures and metric evaluator.
- [x] Build the repeatable evaluation harness and local inference abstraction contract.
- [x] Execute local smoke inference with both European model candidates on macOS ARM64 (preliminary results in `docs/model-validation.md`).
- [ ] Independently review and approve the 600-case candidate dataset.
- [ ] Evaluate multiple legally suitable European models on the same approved held-out cases.
- [ ] Compare accuracy, semantic fidelity, false positives, memory and performance.

**Local inference and harness implemented; quality exit remains open:** LLamaSharp CPU backend, correction prompt/parser, CLI, and benchmark run path exist. Both candidate models have run locally, including through the macOS ARM64 Native AOT executable. Provisional smoke tests are not acceptance evidence. Model/engine selection requires independent dataset review and a reproducible held-out benchmark; if none meets the target, document a different approach rather than claiming success.

## Phase 2: document ingestion

- [ ] Read `.txt` and `.docx` with structural metadata.
- [ ] Validate `.doc` extraction using a commercially reusable, Java-free parser or converter.
- [ ] Test malformed files, large documents, tables, headers, footnotes and other structures.

**Exit:** honest support matrix for each format and its limitations.

## Phase 3: review and export

- [ ] Produce structured correction proposals and local reports.
- [ ] Add deterministic safeguards for names, numbers, meaning and source positions.
- [ ] Export a new corrected document only when formatting is sufficiently reliable.
- [ ] Preserve original inputs; add explicit approval for any lossy conversions.

**Exit:** verified accuracy and original-preserving round-trip tests on fixtures.

## Phase 4: portable distribution

- [ ] Produce self-contained Windows/macOS/Linux artifacts (architectures as validated).
- [ ] Bundle pinned offline inference dependencies and model assets with checksums.
- [ ] Validate on clean machines without Java, Python, .NET, Docker, or other prerequisite installations.
- [ ] Test network-isolated execution, code signing requirements and license notices.

**Exit:** reproducible portable release artifacts and a published support matrix.
