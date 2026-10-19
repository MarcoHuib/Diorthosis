# ADR 0007: LLamaSharp EOG termination

- **Status:** Approved fix implemented and validated on macOS ARM64
- **Decision updated:** 2026-10-20
- **Scope:** Ministral 3B, 8B and 14B using LLamaSharp 0.27.0 and its pinned llama.cpp revision

## Context and diagnosis

The 8B and 14B v3 diagnostic traces began with correct repairs and continued with explanations, alternatives or repeated Markdown up to the 128-token maximum. Inspection found that `InteractiveExecutor.InferInternal` replaced a sampled EOS token with the vocabulary newline token before queueing it; its later EOG check inspected that replacement. The embedded template and BOS/EOS metadata were correct. The frozen correction prompt, parser, validator and evaluator are unchanged.

## Decision

Use LLamaSharp's supported `StatelessExecutor` with `ApplyTemplate = true`. It applies the model-embedded template through `LLamaTemplate`, checks model-defined EOG on the sampled token before decoding or yielding it, and creates/disposes an isolated context per request while reusing model weights. This avoids custom stop strings or model-specific workarounds and preserves prompt content. The 128-token maximum remains a separate upper bound.

The stateless executor recreates a context for each request. The inference semaphore serializes access to weights and the executor. Context cleanup is owned by its async inference iterator on normal completion, cancellation and exceptions. A per-request greedy sampler records actual termination reason and sampled-token count (including the terminal EOG). Cancellation is rethrown after the iterator exits so a partial output is not returned.

## Verification

Machine: Apple M4 Mac mini, 32 GB unified memory; macOS 26.6.2 ARM64; .NET 10.0.12. Runtime: LLamaSharp 0.27.0 and llama.cpp `3f7c29d318e317b63f54c558bc69803963d7d88c`, Metal disabled, CPU-only, nine threads, context 4096, greedy decoding, maximum 128 tokens. Existing local Q4_K_M artifacts were used from outside Git; no model assets were downloaded.

The real-model regression passed on all three Ministral sizes. It checks natural EOG termination for `en-03`, `en-06`, `en-07` and `nl-09`, absence of emitted EOS text and multi-line explanations, the one-token cap, repeat-request determinism after a different-language request, correct-text preservation, cancellation and subsequent successful inference using the same adapter. It is opt-in with `DIORTHOSIS_TEST_MODEL`, so ordinary CI does not require weights.

The updated macOS ARM64 Native AOT CLI published and generated the same `en-03` correction using 3B, 8B and 14B. The benchmark runner then ran the existing 44-case dataset on all three models sequentially. All 132 generations terminated on a sampled EOG; none hit the token limit. Traces record raw output, termination reason and token count, accepted edits, latency and evaluator results in [`results/v4/`](../../benchmarks/model-validation/results/v4/).

| Model | NL recall / precision / false-positive edits | EN recall / precision / false-positive edits | Clean-text preservation NL / EN | Rejected outputs | Mean / max latency | Peak working set |
| --- | --- | --- | --- | ---: | ---: | ---: |
| Ministral 3B Q4_K_M | 68.8% / 84.6% / 2 | 68.8% / 60.0% / 8 | 71.4% / 100% | 2/44 | 1.373 / 2.043 s | 2.933 GB |
| Ministral 3 8B Q4_K_M | 75.0% / 61.9% / 8 | 68.8% / 54.5% / 10 | 71.4% / 85.7% | 4/44 | 3.148 / 4.720 s | 6.269 GB |
| Ministral 3 14B Q4_K_M | 87.5% / 75.0% / 5 | 75.0% / 59.1% / 9 | 85.7% / 100% | 1/44 | 4.975 / 6.936 s | 9.525 GB |

Model initialization times were 1.180 s, 1.466 s and 2.021 s, respectively. Latency includes correction processing; the host was already running and file-cache state was uncontrolled. Process working set was sampled every 100 ms. All runs share dataset SHA-256 `2190c109f5f8e8ce1979637a1beea9fcce615959033aa76576b32ba537465fa2`, prompt `diorthosis-correction-v3`, context, thread policy and decoding settings.

These 44 cases have only 16 provisional annotated errors and 7 provisional clean cases per language; they are diagnostic, not held-out evidence. No target is claimed. Although 14B has the strongest recall, its English recall is 75%, precision is 59.1%, and Dutch clean-text preservation is 85.7%. None meets strict precision or preservation expectations. Recommend evaluating EuroLLM 9B next on independently reviewed data, with corrected Ministral 14B as a comparison baseline if appropriate.

The v2/v3 model-selection results remain preserved as historical artifacts and are superseded: v3 8B/14B generations were affected by the executor defect, and its 3B aggregate was carried forward from before the rebuilt CPU backend.
