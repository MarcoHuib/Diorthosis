# Correction quality benchmark

**Status:** proposed specification. No test set has been assembled and no model has been evaluated yet.

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

## Core metrics (report per language and pooled)

- **Error repair recall:** number of annotated errors correctly fixed / total annotated errors. **Proposed gate: >=90% for Dutch and >=90% for English.** A 90% figure is not a model capability claim until measured.
- **Correction precision:** number of suggested changes judged correct / all suggested changes. **Initial aspirational gate: >=98%.**
- **Correct-input preservation:** proportion of already-correct inputs left free of unjustified changes. **Initial aspirational gate: >=99%.**
- **Semantic/factual preservation:** independently inspect alterations of meaning, names, dates, numbers, quotations, links, and technical identifiers. **Release-blocking: zero known critical unwanted changes on the test suite.** No finite sample proves zero risk.
- **Presentation/fidelity:** measure `.docx` style/content differences, supported `.doc` behavior, and whether review exports preserve original input.
- **Operational measures:** tokens/sec where appropriate, wall-clock correction latency, memory use, cold-start time, package size, and fully offline operation.

Provide confidence intervals and exact sample sizes alongside percentages where feasible. Record false positives separately from missed errors.

## Evaluation procedure

1. Pin the dataset revision, evaluation script, inference engine revision, model ID/checksum, quantization, prompt template, settings and hardware/OS.
2. Run at temperature 0 or another fixed decoding configuration, with a fixed seed where the engine supports it. Note that some backends are not perfectly deterministic.
3. Record each input, gold annotations, suggested corrections, and whether meaning or formatting was changed.
4. Use structured annotation matching rather than exact final-string equality alone; human reviewers resolve ambiguous alternatives, ideally without knowing the model identity.
5. Run a held-out comparison of candidate models; do not tune against the final evaluation set.
6. Regression-test newly discovered failures without retroactively altering the original benchmark scores.
7. Publish machine-readable results and a human-readable summary without exposing private documents.

## Candidate comparison worksheet

| Candidate | Provenance verified | License verified | NL recall | EN recall | Precision | Correct-text preservation | Peak RAM | Notes |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| European model A | No | No | Not tested | Not tested | Not tested | Not tested | Not tested | Candidate only |
| European model B | No | No | Not tested | Not tested | Not tested | Not tested | Not tested | Candidate only |

Do not treat model architecture, parameter count, quantization, or language support listings as proof of correction quality.
