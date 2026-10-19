# Correction quality benchmark

**Status:** Phase 1 schema and deterministic evaluator implemented. No quality benchmark dataset has been assembled and no model has been evaluated.

## Objective

Determine whether an offline, European-origin model can independently provide acceptable Dutch and English spelling and grammar correction, without Java-based grammar engines or extra spelling libraries.

## Initial benchmark design

Start with **600 rights-cleared test cases** (300 Dutch, 300 English), including challenging real-world errors. Per language:

| Category | Cases |
| --- | ---: |
| Spelling, including realistic typos | 75 |
| Grammar, including Dutch verb forms where applicable | 75 |
| Sentence structure and punctuation | 75 |
| Already-correct text and specialist terminology | 75 |

Use hand-annotated errors and acceptable corrections. Keep a separate development set for prompt tuning and a **held-out evaluation set** not used for prompt selection. Mix single sentences with paragraphs and representative document excerpts, and record complexity and ambiguity. Do not include confidential or copyrighted material without rights to use it.

Each JSONL case contains a stable `id`, `language`, `inputText`, and `errors`. Each error has a UTF-16 source `start` and `length`, `originalText`, `category`, and one or more `acceptedCorrections` containing valid alternative replacement texts. The parallel predictions JSONL maps `caseId` to proposed edits (`start`, `length`, `originalText`, `replacementText`). The evaluator checks bounds and exact source-span text before scoring. A tiny synthetic CLI smoke fixture is stored at `benchmarks/smoke/`; it validates the tool path only and is not a quality benchmark.

Run the evaluator with `dotnet run --project src/Diorthosis.Benchmarks -- evaluate cases.jsonl predictions.jsonl`. It emits stable JSON metric rows by language and category. Missing predictions are counted as no edits. Duplicate IDs, unknown prediction IDs, and invalid source spans are rejected.

## Core metrics (report per language and pooled)

- **Error repair recall:** number of annotated errors correctly fixed / total annotated errors. **Proposed gate: >=90% for Dutch and >=90% for English.** A 90% figure is not a model capability claim until measured.
- **Correction precision:** number of suggested changes judged correct / all suggested changes. **Initial aspirational gate: >=98%.**
- **False-positive suggestions:** suggestions not matching an annotated error and one accepted alternative, reported separately as a count and through precision.
- **False-positive rate on clean text:** clean cases with one or more unjustified edits / clean cases. This case-level rate is distinct from proposal precision.
- **Correct-input preservation:** proportion of already-correct inputs left free of unjustified changes. **Initial aspirational gate: >=99%.**
- **Semantic/factual preservation:** independently inspect alterations of meaning, names, dates, numbers, quotations, links, and technical identifiers. **Release-blocking: zero known critical unwanted changes on the test suite.** No finite sample proves zero risk.
- **Presentation/fidelity:** measure `.docx` style/content differences, supported `.doc` behavior, and whether review exports preserve original input.
- **Operational measures:** tokens/sec where appropriate, wall-clock correction latency, memory use, cold-start time, package size, and fully offline operation.

Provide confidence intervals and exact sample sizes alongside percentages where feasible. Record false positives separately from missed errors.

## Evaluation procedure

1. Pin the dataset revision, evaluation script, inference engine revision, model ID/checksum, quantization, prompt template, settings and hardware/OS.
2. Run at temperature 0 or another fixed decoding configuration, with a fixed seed where the engine supports it. Note that some backends are not perfectly deterministic.
3. Record each input, gold annotations, suggested corrections, and whether meaning or formatting was changed. Meaning preservation remains a human-reviewed field; it is not inferred from string similarity.
4. Use structured annotation matching rather than exact final-string equality alone; human reviewers resolve ambiguous alternatives, ideally without knowing the model identity.
5. Run a held-out comparison of candidate models; do not tune against the final evaluation set.
6. Regression-test newly discovered failures without retroactively altering the original benchmark scores.
7. Publish machine-readable results and a human-readable summary without exposing private documents.

## Candidate comparison worksheet

| Candidate | Provenance verified | License verified | NL recall | EN recall | Precision | Correct-text preservation | Peak RAM | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `mistralai/Ministral-3-3B-Instruct-2512-GGUF` | Upstream disclosure; audit pending | Apache 2.0 listed upstream | Not tested | Not tested | Not tested | Not tested | Not measured | Primary small candidate; official Q4_K_M file listed at 2.15 GB |
| `utter-project/EuroLLM-1.7B-Instruct` | EU-funded consortium disclosed; audit pending | Apache 2.0 listed upstream | Not tested | Not tested | Not tested | Not tested | Not measured | Secondary small candidate; GGUF conversion unverified |

The Q4_K_M file size is not a peak RAM estimate. Measure process memory, cold start, latency and package size on target hardware and record the exact model revision and context length.

Do not treat model architecture, parameter count, quantization, or language support listings as proof of correction quality.
