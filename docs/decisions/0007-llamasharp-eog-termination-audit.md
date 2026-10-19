# ADR 0007: LLamaSharp EOG termination audit

- **Status:** Diagnosis confirmed; runtime correction pending human approval
- **Date:** 2026-10-19
- **Scope:** Ministral 8B and 14B diagnostic traces on macOS ARM64, using LLamaSharp 0.27.0 and its pinned llama.cpp revision

## Context

The 8B and 14B v3 diagnostic traces include outputs that begin with a correct repair and continue with explanations, alternatives or repeated Markdown until the 128-token maximum. The audit was limited to verifying prompt template application, BOS/EOS/EOG behavior, natural stopping and four representative cases (`en-03`, `en-06`, `en-07`, `nl-09`). It did not change or tune the prompt, parser, validator, evaluator or correction pipeline.

## Evidence

Both GGUFs identify BOS ID 1 (`<s>`), EOS ID 2 (`</s>`), automatic BOS insertion and no automatic EOS insertion. Their embedded Mistral template formats a user turn with `[INST]` and uses `eos_token` to mark the assistant turn. LLamaSharp `PromptTemplateTransformer` applies this embedded template through native `llama_chat_apply_template`; `InteractiveExecutor` then tokenizes with BOS enabled and special-token parsing enabled. A direct native render/tokenize of `en-03` matched the LLamaSharp formatted prompt byte-for-byte and matched all 113 prompt token IDs.

The direct C# reproduction yielded 128 token pieces for Ministral 8B `en-03`. A traced `GreedySamplingPipeline` recorded EOS token ID 2 with `IsEndOfGeneration=true` at generation positions 9, 45, 70, 89, 105 and 125, while generation continued. Pinned LLamaSharp v0.27.0 source explains this: `InteractiveExecutor.InferInternal` replaces sampled EOS with the vocabulary newline token before queuing it, while `PostProcess` checks the queued token for EOG. With the end marker replaced and no antiprompt configured, the executor continues to the output-token limit.

A temporary native C++ comparison used the pinned llama.cpp C API and the CPU-only libraries from commit `3f7c29d318e317b63f54c558bc69803963d7d88c`. It used the same prompt, embedded template, tokenization, context size (4096), CPU threads (9), GPU layers (0), greedy temperature-zero selection, 128-token maximum and `kv_unified=true`. For both 8B and 14B, native inference returned the initial correction and naturally stopped on EOG for all four selected cases. Generated output lengths were 8, 6, 7 and 12 tokens. This is direct evidence of an inference integration defect, not proof of general model quality.

## Decision

Do not add language- or model-specific workaround strings, accept only the first output line, or change the frozen correction pipeline. The minimal general runtime fix is to treat a sampled EOG as completion termination before replacing it with ordinary newline text or decoding another token. Preserve `MaxTokens` as an independent upper bound.

No runtime code has been changed. Approval is required before making this change. After approval, add a regression that demonstrates stopping on EOG and rerun the same four cases per model through the CLI; record whether the corrections validate, without expanding the benchmark or retuning the prompt.

## 3B comparison caveat

The existing 3B v2 benchmark predates `ca7a3c2`, which added the Metal-disabled CPU-only native backend. The v3 comparison has no 3B raw trace and carries the old 3B aggregate forward. Matching runtime labels and decoding settings do not prove that the old result used the rebuilt native binary. Treat the 3B metrics as historical and rerun 3B under the CPU-only backend before comparing all three family members.
