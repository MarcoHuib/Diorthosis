# Correction quality benchmark

**Status:** Schema/evaluator and local inference runner are implemented. A 44-case preliminary smoke set has provisional annotations and a 600-case synthetic review candidate is available; neither is an independently approved benchmark. See [model-validation datasets](../benchmarks/model-validation/README.md) and [model validation record](model-validation.md). No quality gate has been established.

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

The final approved set must have independently reviewed error spans and acceptable corrections. Keep separate development data for prompt tuning and a **held-out evaluation set** not used for prompt selection. Mix single sentences with synthetic paragraphs and representative non-confidential scenarios, and record complexity and ambiguity. Do not include confidential or copyrighted material without rights to use it.

The six-case `development.jsonl` and 44-case `smoke-44.jsonl` have been used while adjusting prompt/parser behavior and are not held-out data. `provisional-600.jsonl` has exactly 300 cases per language with 75 spelling, 75 grammar, 37 sentence-structure, 38 punctuation and 75 clean cases per language. It is synthetic and schema-valid, but its annotations are not independently reviewed; do not score it or describe the quality target as met until review and a versioned held-out protocol are complete.

Each JSONL case contains a stable `id`, `language`, `inputText`, and `errors`. Each error has a UTF-16 source `start` and `length`, `originalText`, `category`, and one or more `acceptedCorrections` containing valid alternative replacement texts. The parallel predictions JSONL maps `caseId` to proposed edits (`start`, `length`, `originalText`, `replacementText`). The evaluator checks bounds and exact source-span text before scoring. A tiny synthetic CLI smoke fixture is stored at `benchmarks/smoke/`; it validates the tool path only and is not a quality benchmark.

The evaluator compares the complete output produced by relevant edits against accepted full-text repairs, so source edit-boundary differences do not cause false negatives. Partial or meaning-changing results do not receive repair credit. Precision and false positives count `TextEdit` hunks; one corrected annotation can require multiple hunks. Unannotated lexical rewrites suppress repair recall for that case, while cosmetic false positives remain separately countable. See the pipeline investigation for this rule and per-case trace semantics.

Run the evaluator with `dotnet run --project src/Diorthosis.Benchmarks -- evaluate cases.jsonl predictions.jsonl`. It emits stable JSON metric rows by language and category. Missing predictions are counted as no edits. Duplicate IDs, unknown prediction IDs, and invalid source spans are rejected.

Run local inference with `dotnet run --project src/Diorthosis.Benchmarks -- run cases.jsonl /path/to/model.gguf predictions.jsonl report.json --model-revision <commit> --quantization Q4_K_M --max-output-tokens 128`. The model must already exist locally. The runner writes one prediction per input case and a report with model and dataset SHA-256, supplied model revision/quantization, prompt/runtime versions, decoding settings, OS, process architecture, cold load time, per-case latency by language, rejected model output count, sampled peak process working set overall and by language, and post-run managed heap reading. Context size is 4096; GPU layer count is zero. Memory is sampled every 100 ms, so short transient peaks can be missed. CPU model identification is not collected, so the report is not a complete reproducibility record. Semantic changes require human review and are not inferred by this runner. Evaluate saved predictions with the existing `evaluate` command. Generate a comparison using `dotnet run --project src/Diorthosis.Benchmarks -- compare comparison.json run-a.json run-b.json`; it rejects reports with mismatched dataset hashes, prompt versions, runtime versions, or decoding settings. Keep prompt development cases separate from held-out cases.

No weights are included. Downloading multi-gigabyte candidates is a deliberate developer action. The exact candidate revisions and converted assets tested during this validation are documented in [model-validation.md](model-validation.md). Upstream license and provenance disclosures are recorded there; they are not an independent training-data audit or a legal opinion. No model weights or conversion outputs are committed.

### Explicit local model setup

The application and CI never fetch weights. Download model files only after the developer explicitly reviews the pinned source revision, required disk space, license statement and notices. For Ministral text correction, use its official Q4_K_M GGUF and omit the vision projector. EuroLLM's source checkpoint can be converted with the pinned developer-only llama.cpp toolchain described in [model-validation.md](model-validation.md). Python and conversion tools are never end-user runtime dependencies. Keep model weights, source checkpoints and temporary tools outside Git. The synthetic smoke predictions and reports are committed under `benchmarks/model-validation/results/` for reproducibility.

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

## Initial v1 comparison (preserved)

This historical table uses the original smoke labels and evaluator. Its results are preserved for audit and superseded by the pipeline investigation linked below.

| Candidate | Provenance verified | License verified | NL recall | EN recall | Precision | Correct-text preservation | Peak RAM | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `mistralai/Ministral-3-3B-Instruct-2512-GGUF` | Upstream disclosure; audit pending | Apache 2.0 declared upstream | 43.8%* | 43.8%* | 77.8% NL / 63.6% EN* | 71.4% NL / 100% EN* | 4.94 GB sampled process working set | Preliminary 44-case smoke only; see `docs/model-validation.md` |
| `utter-project/EuroLLM-1.7B-Instruct` | EU-funded European consortium disclosed; audit pending | Apache 2.0 declared upstream | 6.3%* | 37.5%* | 50.0% NL / 75.0% EN* | 100% NL / 100% EN* | 2.30 GB sampled process working set | Official checkpoint converted to Q4_K_M; preliminary 44-case smoke only |

\* These are provisional evaluator rates over 16 unreviewed error annotations and 7 unreviewed clean cases per language, not validated quality results. They do not show that the 90% recall, 98% precision or 99% preservation target is met. Human semantic review remains pending.

The Q4_K_M file size is not a peak RAM estimate. Measure process memory, cold start, latency and package size on target hardware and record the exact model revision and context length.

Do not treat model architecture, parameter count, quantization, or language support listings as proof of correction quality.

## Pipeline-corrected v2 smoke run

The benchmark pipeline was corrected after reviewing all 44 cases. Version 2 revises two provisional references, applies the final parser to captured raw model generations, evaluates complete resulting passages, and records raw versus accepted metrics. The v2 results remain preliminary because the smoke data is small and not independently bilingual-adjudicated. See [the pipeline investigation](benchmark-pipeline-investigation.md) and [the v2 raw-output traces and reports](../benchmarks/model-validation/results/v2/).
