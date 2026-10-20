# EuroLLM 9B v1 benchmark

This is a bounded, offline replay of the frozen `smoke-44-reviewed-v2.jsonl` dataset. It used the existing Diorthosis LLamaSharp runtime and correction pipeline once for each of the 44 cases. No Ministral inference was rerun; Ministral values below come from the frozen v4 and `v4-post-safety` results.

## Model and provenance

- Original: [`utter-project/EuroLLM-9B-Instruct`](https://huggingface.co/utter-project/EuroLLM-9B-Instruct), repository revision `f7ae2bc3bcbb538c0b93fa6cfbf388d9898b1ace`.
- GGUF conversion: [`bartowski/EuroLLM-9B-Instruct-GGUF`](https://huggingface.co/bartowski/EuroLLM-9B-Instruct-GGUF), revision `55e5590cc457e42ed197c34c5b10403472b05d54`; the Q4_K_M file was added at conversion commit [`f111d6373575ce0852edc1451ff0a2204dfc27f6`](https://huggingface.co/bartowski/EuroLLM-9B-Instruct-GGUF/commit/f111d6373575ce0852edc1451ff0a2204dfc27f6).
- File: `EuroLLM-9B-Instruct-Q4_K_M.gguf`, Q4_K_M, 5,582,838,496 bytes, SHA-256 `785a3b2883532381704ef74f866f822f179a931801d1ed1cf12e6deeb838806b`.
- The upstream card identifies Apache 2.0 licensing; the GGUF metadata reports `apache-2.0`. The original model is European-origin work from the UTTER project.
- Weights were kept at `/private/tmp/diorthosis-models/` and are not in Git.

## Compatibility

The six-case smoke test passed. LLamaSharp loaded the embedded chat template; all six generations terminated with `EndOfGeneration`; Dutch and English corrections, an unchanged technical identifier, clean text, and sentence restructuring reached the correction pipeline.

Full settings matched the Ministral v4 run: context 4096, 9 CPU threads, 0 GPU layers, greedy temperature 0, maximum output 128, prompt `diorthosis-correction-v3`.

## Results

The `v2-error-level` metrics are current evaluator metrics. `historicalHunkPrecision` is retained as a historical edit-hunk measure and is not a semantic-validity probability.

| Language | Model | Error recall | Historical hunk precision | Clean-text preservation | Strict passage acceptability | Dispositions |
|---|---|---:|---:|---:|---:|---|
| Dutch | EuroLLM 9B | 56.25% | 100.00% | 100.00% | 68.18% | 8 accepted, 14 unchanged |
| Dutch | Ministral 14B post-safety | 87.50% | 75.00% | 85.71% | 86.36% | 15 accepted, 2 rejected, 5 unchanged |
| English | EuroLLM 9B | 87.50% | 80.95% | 100.00% | 90.91% | 15 accepted, 7 unchanged |
| English | Ministral 14B post-safety | 75.00% | 59.09% | 100.00% | 81.82% | 15 accepted, 0 rejected, 7 unchanged |

Historical v4 metrics remain unchanged in their frozen files: Ministral Dutch recall/hunk precision/clean preservation were 87.50%/75.00%/85.71%, and English were 75.00%/59.09%/100.00%. The post-safety report adds the versioned strict and disposition metrics without rewriting those historical values.

Runtime measurements on the same Arm64 macOS host and settings:

| Metric | EuroLLM 9B | Ministral 14B frozen v4 |
|---|---:|---:|
| Cold start | 3,413 ms | 2,021 ms |
| Mean latency, Dutch | 3,489 ms | 5,235 ms |
| Mean latency, English | 3,407 ms | 4,716 ms |
| Mean latency, all cases | 3,448 ms | 4,975 ms |
| Peak working set | 11.32 GB | 9.52 GB |

EuroLLM was faster in this run but used about 1.8 GB more peak working set. The memory measurement is process working set sampled across load and inference.

## Case differences and safety observations

- EuroLLM newly received correct error-level credit where Ministral did not on `en-05` and `en-09`.
- Ministral repaired cases that EuroLLM left unchanged on `nl-04`, `nl-05`, `nl-07`, `nl-11`, and `nl-13`.
- `en-04` was not a valid repair for either model under the current annotation; both changed the sentence but retained incorrect content.
- EuroLLM accepted changes on `en-01` and `en-04` with false-positive edit attribution. These require human review; they are not evidence of semantic safety.
- EuroLLM left the technical/content-sensitive `nl-20` and `nl-21` inputs unchanged. The post-safety Ministral replay rejected its corresponding unsafe raw outputs on those cases (URL/symbol and protected identifier changes). Those are baseline safety blocks, not new EuroLLM rejections.
- EuroLLM had no rejected, suppressed, or `NeedsReview` outputs in this 44-case run. This is an outcome count, not proof that the model made no semantically questionable accepted change.

## Accounting and artifacts

`predictions.jsonl` and `trace.jsonl` each contain 44 cases. The dataset SHA-256 is `2190c109f5f8e8ce1979637a1beea9fcce615959033aa76576b32ba537465fa2`; it matches the frozen dataset. Historical `v4`, `v4-post-safety`, audit artifacts, and gold annotations were not modified.

## Recommendation

**C. Keep both for further evaluation.** EuroLLM has materially better English results, perfect clean-text preservation in this run, and lower latency, but it is substantially worse on Dutch recall and has higher peak memory. The accepted English false-positive cases also need independent human adjudication before either model is selected as the default offline correction model. This bounded run is not an independent human validation or release-quality claim.
