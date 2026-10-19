# Benchmark pipeline investigation

**Date:** 2026-10-19. **Status:** Pipeline defects corrected; results are still preliminary smoke measurements, not a held-out quality evaluation.

## Findings and changes

The first 44-case run exposed defects in both prediction handling and scoring:

1. The parser compared the source and candidate after deleting all whitespace. That treated a Dutch compounding correction such as `vergader ruimte` → `vergaderruimte` as unchanged. It now retains true no-op whitespace normalization, but accepts a whitespace-only change that joins word tokens.
2. A single longest-common-prefix/suffix edit could include unchanged words between distant changes and obscure independent corrections. The replacement diff uses Unicode text elements, deterministic LCS for small inputs and a bounded Myers diff otherwise. It emits multiple non-overlapping UTF-16 edits, merges fragments within a word, and verifies that applying the edits recreates the extracted candidate exactly. A work bound makes pathological diffs fail closed.
3. The evaluator required exact equality between the prediction edit span and annotation span. It now applies relevant edits and compares the resulting complete text with accepted corrections, so harmless boundary differences are recognized while partial or meaning-changing replacements receive no credit.
4. A per-annotation comparison could give credit to `zijn` → `is` inside a full Dutch-to-English translation. The evaluator now withholds recall credit when lexical edits outside the annotations change the passage; cosmetic case, whitespace, and punctuation extras remain countable false positives.
5. Some generations contain one complete answer followed by repetition or explanations. The parser now extracts a complete quoted answer or a repeated first answer where the source is one line, and strips recognized output labels. It rejects unsolicited line breaks for a one-line source, malformed/incomplete responses, protected content changes, and unverifiable rewrites. Raw candidate edits and accepted edits are recorded separately.
6. Capitalized sentence-initial function words were being treated like proper names. The name guard now distinguishes a maintained list of common Dutch and English sentence-initial function words while retaining exact protection for other capitalized terms, mixed-case identifiers, numbers, URLs, and symbols. This is a conservative rule, not general named-entity recognition.

All safety-rejected candidates leave the source unchanged. The benchmark trace records raw generation, extracted candidate, raw candidate edits, accepted edits, rejection stage/reason, and per-case raw/accepted evaluator output. It contains synthetic smoke text only.

## Manual annotation review

All 44 smoke annotations were inspected independently of the evaluated models. Two provisional references were revised in `smoke-44-reviewed-v2.jsonl`; the original `smoke-44.jsonl`, predictions, and reports remain unchanged.

| Case | Original annotation | Revision | Reason |
| --- | --- | --- | --- |
| `nl-01` | `Ik heb het verslag gespreekt.` → `Ik heb het verslag gesproken.` | Input is now `Ik heb met haar gespreekt.`; expected `gesproken` is retained. | The original object made the intended meaning and correction ambiguous. |
| `nl-15` | `en laat` → `en laten` | Annotated span is `en laat me`; accepted replacement is `en me laten`. | The previous replacement produced ungrammatical Dutch word order. |

The remaining 42 references were retained after case-by-case review. This was not an independent bilingual adjudication or blinded expert review. The full file remains provisional; repeated constructions (including two near-duplicate `lets` punctuation cases) make it unsuitable as a representative held-out set.

The reviewed file SHA-256 is `2190c109f5f8e8ce1979637a1beea9fcce615959033aa76576b32ba537465fa2`. The original smoke dataset SHA-256 remains `1cb37b489824f2d8abb1d8802042d1771cea289334f59ebc3a0fbb94c3855769`.

## Re-run results

Both pinned Q4_K_M models were executed again on the same revised 44-case inputs, with the same prompt (`diorthosis-correction-v3`), temperature 0, context 4096, CPU threads 9, GPU layers 0, and maximum output 128 tokens. They ran on the Apple M4 ARM64 / macOS 26.6.2 under a deny-network sandbox. Captured raw generations were then reparsed with the final deterministic parser; `reparse` preserves measured inference timing and working-set data and replaces only parsing/evaluation fields. Use the per-case trace to distinguish raw candidates from accepted edits.

### Model candidate text before safety validation

| Model | Language | Annotated errors | Raw candidate recall | Raw edit precision | Raw clean-text preservation |
| --- | --- | ---: | ---: | ---: | ---: |
| Ministral Q4_K_M | Dutch | 16 | 11/16 (68.8%) | 11/13 (84.6%) | 5/7 (71.4%) |
| Ministral Q4_K_M | English | 16 | 11/16 (68.8%) | 12/20 (60.0%) | 7/7 (100%) |
| EuroLLM Q4_K_M | Dutch | 16 | 1/16 (6.3%) | 1/54 (1.9%) | 5/7 (71.4%) |
| EuroLLM Q4_K_M | English | 16 | 7/16 (43.8%) | 8/11 (72.7%) | 7/7 (100%) |

EuroLLM's Dutch raw candidate counts include edit hunks from several English translations and malformed continuations. The high hunk count is evidence of those rewrites, not 54 independent correction attempts. Raw rates are exact comparisons against still-provisional annotations and must not be interpreted as validated quality.

### Corrections after validation

| Model | Language | Recall | Edit precision | False-positive edits | Clean-text preservation | Clean cases changed |
| --- | --- | ---: | ---: | ---: | ---: | ---: |
| Ministral Q4_K_M | Dutch | 11/16 (68.8%) | 11/13 (84.6%) | 2 | 5/7 (71.4%) | 2/7 |
| Ministral Q4_K_M | English | 11/16 (68.8%) | 12/20 (60.0%) | 8 | 7/7 (100%) | 0/7 |
| EuroLLM Q4_K_M | Dutch | 1/16 (6.3%) | 1/1 (100%; one suggestion) | 0 | 7/7 (100%) | 0/7 |
| EuroLLM Q4_K_M | English | 7/16 (43.8%) | 8/11 (72.7%) | 3 | 7/7 (100%) | 0/7 |

False-positive rates on the seven clean cases per language were 28.6% Dutch / 0% English for Ministral and 0% / 0% for EuroLLM. The small per-language denominators and provisional references make these descriptive smoke rates only. No quality target is established.

### Rejected outputs and causes

The final parser rejected 1/44 Ministral outputs (one-line input followed by unsolicited alternative lines) and 16/44 EuroLLM outputs:

| EuroLLM rejection reason | Count | Review of raw candidate |
| --- | ---: | --- |
| Excessive source wording change | 8 | Predominantly English translations of Dutch text; rejected as unverified rewrites. |
| Capitalized name/term mismatch | 2 | Month/date wording or terms changed in English translations; no accepted complete Dutch repair was lost. |
| No verifiable correction passage | 3 | Instruction echoes or malformed response; no usable candidate. |
| New line breaks absent from input | 3 | Repeated/commentary blocks on single-line input; rejected fail-closed. |

The raw-candidate evaluator found no fully accepted annotated correction among the safety-rejected candidates after requiring the full passage to match an accepted correction and rejecting unannotated lexical rewrites. In particular, a local `zijn` → `is` match inside EuroLLM's English translation is not scored as a Dutch repair. No known correct annotated repair was blocked by the final validator in this smoke set. This conclusion is limited to these 44 cases and provisional labels.

The strongest observed model defect is EuroLLM's language drift on Dutch inputs and both models' inconsistent adherence to the requested output format. The evaluator had independently been overstating corrections embedded in rewrites; that issue is fixed. The smoke data still cannot establish semantic preservation, and the model outputs have not had blinded human semantic review.

## Performance

| Model | Model load (process start) | Mean latency NL / EN | Peak sampled working set NL / EN |
| --- | ---: | ---: | ---: |
| Ministral Q4_K_M | 1,789 ms | 4,693 / 4,509 ms | 4.929 / 4.942 GB |
| EuroLLM Q4_K_M | 1,118 ms | 2,091 / 2,092 ms | 2.294 / 2.307 GB |

Memory is process working set sampled every 100 ms, including the loaded model; it is not total system/unified memory. Model-load values are from process launches with uncontrolled OS file cache, not cold boot. Times reflect one 44-case run and are not throughput guarantees.

## Reproduction and artifacts

The original results remain under `benchmarks/model-validation/results/`. Revised reports and per-case traces are under `benchmarks/model-validation/results/v2/`. Both reports share the reviewed dataset hash, model runtime, hardware, prompt and decoding settings; `comparison.json` records raw and accepted metrics, rejection reasons, timing, and memory.

Reparse a captured run without rerunning generation:

```sh
dotnet run --project src/Diorthosis.Benchmarks -- reparse \
  benchmarks/model-validation/smoke-44-reviewed-v2.jsonl \
  benchmarks/model-validation/results/v2/ministral-trace.jsonl \
  benchmarks/model-validation/results/v2/ministral-report.json \
  /tmp/ministral-predictions.jsonl /tmp/ministral-trace-v2.jsonl /tmp/ministral-report-v2.json
```

The trace must contain exactly one raw generation per case, and each captured input is checked against the dataset before reanalysis. For a fresh model run, pass `--trace <path>` to `diorthosis-benchmarks run` and `--max-output-tokens 128` to use the settings above. Keep model weights outside Git.

The smoke set has been used during prompt/parser development. The 600-case candidate was not tuned against or run for quality. Independent bilingual review and a separate held-out benchmark remain necessary.
