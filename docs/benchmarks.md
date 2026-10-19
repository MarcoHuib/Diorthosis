# Correction quality benchmark

**Status:** Schema/evaluator plus local inference runner implemented. No quality benchmark dataset has been assembled and no real model has been evaluated.

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

Run local inference with `dotnet run --project src/Diorthosis.Benchmarks -- run cases.jsonl /path/to/model.gguf predictions.jsonl report.json --model-revision <commit> --quantization Q4_K_M`. The model must already exist locally. The runner writes one prediction per input case and a report with model and dataset SHA-256, supplied model revision/quantization, prompt/runtime versions, decoding settings, OS, process architecture, cold load time, per-case latency summary, peak working set, and post-run managed heap reading. CPU model identification is not collected, so the report is not a complete reproducibility record. Semantic changes require human review and are not inferred by this runner. Evaluate saved predictions with the existing `evaluate` command. Generate a comparison using `dotnet run --project src/Diorthosis.Benchmarks -- compare comparison.json run-a.json run-b.json`; it rejects reports with mismatched dataset hashes, prompt versions, runtime versions, or decoding settings. Keep prompt development cases separate from held-out cases.

No weights are included. Downloading multi-gigabyte candidates is a deliberate developer action; verify immutable upstream revision, license, tokenizer, GGUF provenance, and redistribution obligations before use. Ministral's upstream GGUF page lists Apache 2.0 and Q4_K_M, but no exact revision is pinned. EuroLLM declares Apache 2.0 and European consortium/EU funding; a rights-cleared reproducible GGUF conversion remains unresolved. Neither candidate has been executed. No platform has real-model validation.

### Explicit local model setup

The application and CI never fetch weights. To prepare a developer-only evaluation, use the upstream model pages in a browser and download one chosen GGUF to a local directory. For Ministral, use `Ministral-3-3B-Instruct-2512-Q4_K_M.gguf`; do not download the separate vision projector for text correction. Record the immutable repository commit shown by the model file page and compute a local SHA-256 before benchmarking. Candidate terms and supply-chain metadata still require review before redistribution.

EuroLLM's source page is a Transformers checkpoint. A conversion investigation can use llama.cpp's `convert_hf_to_gguf.py` and its conversion documentation, but that developer-only toolchain uses Python and is not part of Diorthosis or an end-user prerequisite. Pin llama.cpp source and conversion dependencies, convert the exact reviewed source revision, verify tokenizer/template metadata and CPU load, then quantify the output checksum and quantization. If the converter does not support EuroLLM's architecture, stop and document the gap rather than adopting an unverified community conversion. No such conversion has been run.

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
