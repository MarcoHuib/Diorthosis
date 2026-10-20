# Local model validation record

The macOS ARM64 native-runtime failure diagnosis and CPU-only generation reproduction are in [ADR 0006](decisions/0006-cpu-only-macos-inference-and-next-model-manifest.md). The replacement uses the same pinned llama.cpp commit as LLamaSharp 0.27.0 with Metal disabled. It generated an English spelling correction from the existing local Ministral 3B GGUF using the published Native AOT CLI; model-quality claims are out of scope for this one runtime check.

**Validation date:** 2026-10-20 (macOS local run)

**Status:** Ministral 3B, 8B and 14B, plus EuroLLM 1.7B, have completed local real-weight runs. The corrected Ministral family comparison is in [`results/v4/`](../benchmarks/model-validation/results/v4/); earlier v2/v3 family metrics remain available as historical records and are superseded because of the EOG termination defect and backend comparability. All 44-case smoke references remain provisional, and the 600-case candidate is not independently reviewed.

## Candidates and rights disclosures

| Candidate | Immutable upstream revision | Source / tested artifact | Upstream disclosure |
| --- | --- | --- | --- |
| `mistralai/Ministral-3-3B-Instruct-2512-GGUF` | `be4e90342e4468f37d4bea40ec143997f9256475` | Official `Ministral-3-3B-Instruct-2512-Q4_K_M.gguf`; SHA-256 `9ed150d4367e68df0ac8e1540f6ddc65b42d0ee26378329d1ecbca60f93fc5f8`; upstream file size 2,147,023,008 bytes | Mistral AI is headquartered in France; the official model repository declares Apache-2.0. Upstream metadata identifies English and Dutch. |
| `utter-project/EuroLLM-1.7B-Instruct` | `a25c7fa65fc2a644e6270b8940dbe295b51da681` | Official Transformers source checkpoint converted locally to `EuroLLM-1.7B-Instruct-Q4_K_M.gguf`; GGUF SHA-256 `a6cdb94bd98e6b39eb9abb8d79eb9f8af72deade34cbf4010c62e8e4da62c1b5`; GGUF size 1,045,157,024 bytes | The model card declares Apache-2.0, lists Dutch and English, names European universities/research organizations and Unbabel among developers, and says EU-funded. |

Official sources: [Ministral GGUF repository](https://huggingface.co/mistralai/Ministral-3-3B-Instruct-2512-GGUF/tree/be4e90342e4468f37d4bea40ec143997f9256475), [Ministral model file](https://huggingface.co/mistralai/Ministral-3-3B-Instruct-2512-GGUF/blob/be4e90342e4468f37d4bea40ec143997f9256475/Ministral-3-3B-Instruct-2512-Q4_K_M.gguf), and [EuroLLM model card](https://huggingface.co/utter-project/EuroLLM-1.7B-Instruct/tree/a25c7fa65fc2a644e6270b8940dbe295b51da681).

These are upstream statements, not an independent provenance audit. In particular, Diorthosis has not audited training-corpus provenance or every contributor's rights. Apache-2.0 permits commercial use and redistribution subject to its conditions, including retaining the license and notices and marking changes. Before redistribution, include the exact applicable license text, upstream notices/attribution and model-file provenance. The EuroLLM source snapshot exposes the Apache-2.0 license identifier in model-card metadata; preserve the canonical Apache-2.0 text and upstream card/revision with any redistribution review. No weights, tokenizers or generated models are stored in Git.

## EuroLLM conversion record

- Source files were fetched from the pinned official revision. The source checkpoint file `model.safetensors` has upstream SHA-256 `853ac1edd51009c46ce28e457a389ef69206b27e3b9734a43ae0644e766553ac` (upstream size 3,313,726,248 bytes). The source tokenizer/configuration files were fetched from the same revision and are recorded outside Git.
- Conversion used the official `ggml-org/llama.cpp` converter at commit `3f7c29d318e317b63f54c558bc69803963d7d88`, matching the llama.cpp revision recorded for LLamaSharp 0.27.0. The F16 GGUF intermediate was quantized by the matching CPU `llama-quantize` build to `Q4_K_M`.
- The GGUF metadata identifies the Llama architecture, includes tokenizer assets, and carries the source chat template. The existing LLamaSharp transformer consumed the embedded template. A generation request produced model text, confirming conversion/runtime compatibility for this tested path.
- Conversion ran with temporary developer-only Python 3.12.15 tools. The complete resolved Python package set (including CMake 3.31.6 and Ninja 1.11.1.4) is pinned in [`converter-requirements.lock`](../benchmarks/model-validation/converter-requirements.lock); the converter source itself is pinned to the llama.cpp commit above. This tooling is not an end-user requirement. Temporary files and source checkpoints should be removed after validation; retain only the final GGUF outside Git if further runs are planned.
- The converter emitted an informational warning about the source default RoPE type. It loaded and generated under llama.cpp; this does not validate every possible context length or downstream task.

## Runtime and machine

- Host: Apple Mac mini, Apple M4 (10 CPU cores), 32 GB unified memory, macOS 26.6.2, ARM64.
- App runtime: LLamaSharp 0.27.0 / `LLamaSharp.Backend.Cpu` 0.27.0; pinned llama.cpp revision above. Context 4096, `GpuLayerCount = 0`, temperature 0. The correction prompt is `diorthosis-correction-v3`; generation maximum is fixed per report.
- Model logs showed all transformer layers and KV cache on CPU, a 416 MiB CPU KV cache and a 268 MiB CPU compute buffer for Ministral. The package initializes an available Metal device, but the tested configuration reported a 0 MiB Metal compute buffer and no GPU layers. Thus model computation was assigned to CPU; the native package still contains/initializes Metal support. No vision projector or multimodal API was used.
- Inference was launched under macOS `sandbox-exec` with an explicit deny-network policy. The model and application ran from local files. This is evidence for the tested request's offline behavior, not a substitute for the repository-wide offline CI test.
- LLamaSharp model/context objects are disposed by `using` in the CLI and benchmark runner. Each AOT model-correction command exited successfully through that disposal path. A separate repeated in-process unload/reload cycle was not measured.

## Real inference runs

The 44-case file has 22 cases per language, 16 provisional error annotations and 7 provisional clean inputs per language. It is a smoke test only: it is too small and its annotations are not independently reviewed. It cannot demonstrate either 90% target.

| Model | Cases completed | NL provisional recall | EN provisional recall | NL precision / false positives | EN precision / false positives | Clean cases changed (NL / EN) | Model load | Mean latency NL / EN | Peak working set NL / EN | Rejected outputs |
| --- | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: | ---: |
| Ministral Q4_K_M | 44 (22 each language) | 7/16 (43.8%) | 7/16 (43.8%) | 7/9 (77.8%) / 2 | 7/11 (63.6%) / 4 | 2/7 / 0/7 | 1,393 ms | 4,130 / 4,200 ms | 4,927 / 4,943 MB | 7 |
| EuroLLM Q4_K_M | 44 (22 each language) | 1/16 (6.3%) | 6/16 (37.5%) | 1/2 (50.0%) / 1 | 6/8 (75.0%) / 2 | 0/7 / 0/7 | 1,127 ms | 1,943 / 1,993 ms | 2,291 / 2,303 MB | 25 |

For Ministral, provisional clean-text preservation was 5/7 (71.4%) Dutch and 7/7 (100%) English; the case-level false-positive rate was 2/7 Dutch and 0/7 English. EuroLLM left all 14 provisional clean cases unchanged. Peak working set was sampled every 100 ms; the per-language values include the loaded model and are process memory, not system-wide memory. The process-level model-load measurements followed earlier local model runs, so the OS file cache was not controlled and these are not cold-boot figures. The model file sizes are 2,147,023,008 bytes for Ministral and 1,045,157,024 bytes for EuroLLM. These are preliminary smoke outcomes against unreviewed annotations, not validated quality measurements; observed rates are below the targets.

Observed examples from the model output:

- Ministral Dutch grammar: `Hij vindt dat de afspraak morgen plaatsvind.` → `Hij vindt dat de afspraak morgen plaatsvindt.`
- Ministral English spelling: `Please recieve the signed form.` → `Please receive the signed form.`
- Ministral English grammar case: `She have send the documents yesterday.` → `She has sent the documents yesterday.` This retains an error with “yesterday”; the expected correction is `She sent the documents yesterday.`
- EuroLLM English spelling (also confirmed through Native AOT): `Please recieve the signed form.` → `Please receive the signed form.`
- EuroLLM Dutch (also confirmed through Native AOT): `Ik heb het verslag gespreekt.` → `Ik heb het verslag gesproken.` The annotation is provisional and this output needs semantic review in context.

The machine-readable smoke predictions, per-model reports, and comparison report are committed under `benchmarks/model-validation/results/`. They contain only the synthetic smoke cases, not private documents. The comparison command accepted both reports as matching the same dataset hash and decoding/inference settings.

## Pipeline-corrected v2 smoke run

The original v1 reports above have been preserved. The benchmark pipeline investigation manually inspected all 44 annotations, corrected two provisional references in a new dataset revision, and captured/reprocessed raw model generations with the corrected parser and evaluator. Details, per-case adjudication notes, exact commands, and metric interpretation are in [`benchmark-pipeline-investigation.md`](benchmark-pipeline-investigation.md). Raw traces are under [`results/v2/`](../benchmarks/model-validation/results/v2/).

| Model | Language | Raw candidate recall / edit precision | Accepted recall / edit precision | False-positive edits | Clean-text preservation / cases changed | Rejected outputs |
| --- | --- | --- | --- | ---: | --- | ---: |
| Ministral Q4_K_M | Dutch | 11/16 (68.8%) / 11/13 (84.6%) | 11/16 (68.8%) / 11/13 (84.6%) | 2 | 5/7 (71.4%) / 2/7 | 1/44 overall |
| Ministral Q4_K_M | English | 11/16 (68.8%) / 12/20 (60.0%) | 11/16 (68.8%) / 12/20 (60.0%) | 8 | 7/7 (100%) / 0/7 | 1/44 overall |
| EuroLLM Q4_K_M | Dutch | 1/16 (6.3%) / 1/54 (1.9%) | 1/16 (6.3%) / 1/1 (100%; one suggestion) | 0 | 7/7 (100%) / 0/7 | 16/44 overall |
| EuroLLM Q4_K_M | English | 7/16 (43.8%) / 8/11 (72.7%) | 7/16 (43.8%) / 8/11 (72.7%) | 3 | 7/7 (100%) / 0/7 | 16/44 overall |

The 54 EuroLLM Dutch raw edit hunks largely come from English translations and malformed output; the per-case trace shows the full raw generations. They are not 54 independent language corrections. The final validator rejected 1 Ministral output and 16 EuroLLM outputs; none of those rejected raw candidates fully matched an annotated accepted correction after whole-passage and unannotated-rewrite checks. The v2 rates remain small-sample comparisons against provisional labels, not validated quality. The recall target remains unmet.

V2 performance on the same M4 ARM64 CPU host: Ministral process model-load 1,789 ms, mean per-case latency 4,693 ms Dutch / 4,509 ms English, sampled peak working set 4.929 / 4.942 GB. EuroLLM: 1,118 ms load, 2,091 / 2,092 ms mean latency, 2.294 / 2.307 GB sampled peak working set. OS file cache was uncontrolled; load figures are not cold-boot measurements. Reparse retained these measurements from the corresponding actual offline inference runs.

Semantic preservation requires human review and remains pending. The provisional cases include names, identifiers, dates, URLs and technical terms, but automated protected-token behavior is not equivalent to human semantic review. Record exact datasets, predictions and run reports by SHA-256 when available. Do not run the 600-case candidate for quality scoring before independent review.

## Ministral family v3 diagnostic screen (superseded)

**Run date:** 2026-10-19. This was a single bounded screen, not a held-out evaluation. It used the same 44-case file as v2 (`smoke-44-reviewed-v2.jsonl`, SHA-256 `2190c109f5f8e8ce1979637a1beea9fcce615959033aa76576b32ba537465fa2`): 22 cases per language, 16 provisional annotated errors and 7 provisional clean cases per language. The annotations remain provisional; no quality target can be claimed from these results.

The frozen v3 prompt/parser/validator/evaluator were used without model-specific changes. The 8B and 14B runs used LLamaSharp 0.27.0 with llama.cpp `3f7c29d318e317b63f54c558bc69803963d7d88c`, context 4096, temperature 0, max 128 output tokens, nine CPU threads and zero GPU layers. They ran under a macOS sandbox denying inbound and outbound network access, using local GGUF files only. Hardware was an Apple M4 Mac mini, 32 GB unified memory, macOS 26.6.2, ARM64, .NET 10.0.12. The v3 comparison carries forward the 3B aggregate from v2; it is not a fresh CPU-only-backend run. A Native AOT publish of the CLI loaded and generated with each model; the benchmark runner captured all 44 8B/14B raw generations and accepted edits.

The local downloads matched the manifest SHA-256 values. The GGUF v3 headers report `mistral3`; both include the `gpt2`/`tekken` tokenizer metadata, 131,072 vocabulary entries, and an embedded Mistral chat template (about 7.75 KB). The pinned runtime loaded and used the embedded template for real text generation. Only the text Q4_K_M files were used; no `mmproj` vision artifact was acquired. Official repositories declare Apache-2.0 and identify Mistral AI (France); commercial redistribution is permitted under the license conditions and notice requirements. This is an upstream disclosure, not an independent audit of training data, provenance, or rights.

| Model and local artifact | Raw candidate NL: recall / precision / false positives | Accepted NL: recall / precision / false positives | Raw candidate EN: recall / precision / false positives | Accepted EN: recall / precision / false positives | Clean-text preservation NL / EN | Rejected generations | Model load | Mean / max case latency | Peak process working set |
| --- | --- | --- | --- | --- | --- | ---: | ---: | ---: | ---: |
| Ministral 3B Q4_K_M; revision `be4e90342e4468f37d4bea40ec143997f9256475`; SHA-256 `9ed150d4367e68df0ac8e1540f6ddc65b42d0ee26378329d1ecbca60f93fc5f8`; 2.147 GB | 11/16 (68.8%) / 11/13 (84.6%) / 2 | 11/16 (68.8%) / 11/13 (84.6%) / 2 | 11/16 (68.8%) / 12/20 (60.0%) / 8 | 11/16 (68.8%) / 12/20 (60.0%) / 8 | 5/7 (71.4%) / 7/7 (100%) | 1/44 | 1.789 s | 4.601 / 6.115 s | 4.942 GB |
| Ministral 3 8B Q4_K_M; revision `0102285ad796bd99af90f58de616092e5630e970`; SHA-256 `33e7a72cf5e6e2cfc2f2847075acc013d68bba023e35310cef86b5cf8fdca761`; 5.199 GB | 7/16 (43.8%) / 8/17 (47.1%) / 9 | 7/16 (43.8%) / 8/13 (61.5%) / 5 | 9/16 (56.3%) / 10/22 (45.5%) / 12 | 9/16 (56.3%) / 10/19 (52.6%) / 9 | 7/7 (100%) / 7/7 (100%) | 15/44 | 0.333 s | 9.812 / 11.022 s | 6.287 GB |
| Ministral 3 14B Q4_K_M; revision `fb49df4a3cde2c774da8def12437118a66c4f5cf`; SHA-256 `824e0f3373e69b84f2cae46fdcb9bd1ebc6ab3bfc7acc125d818b7b8178cc613`; 8.240 GB | 12/16 (75.0%) / 13/21 (61.9%) / 8 | 12/16 (75.0%) / 13/18 (72.2%) / 5 | 5/16 (31.3%) / 6/11 (54.5%) / 5 | 5/16 (31.3%) / 6/11 (54.5%) / 5 | 6/7 (85.7%) / 7/7 (100%) | 13/44 | 1.918 s | 15.548 / 19.131 s | 9.537 GB |

Recall fractions use 16 provisional errors per language. Precision fractions are correct edit hunks / all proposed edit hunks; false positives are edit-hunk counts. Correct-text preservation uses 7 provisional clean cases per language. The 3B values come from the existing v2 baseline report; its v2 trace predates per-case latency capture, so its report has aggregate mean and maximum only. The 8B and 14B traces each record all 44 per-case latencies (median/min/max: 8B 9.707/9.505/11.022 s; 14B 15.345/14.734/19.131 s). The 100 ms process working-set sampler reported peak memory for the benchmark process, not system-wide unified memory.

The 8B validator rejected 15 generations: 9 added line breaks, 3 did not yield a verifiable passage, 2 changed symbols, and 1 changed protected content. The 14B validator rejected 13: 12 added line breaks and 1 changed a symbol. The 3B baseline rejected 1 generation. Raw model text, extracted candidates, rejections, validated edits, per-case metrics and timings are retained in the `*-trace.jsonl` files; predictions and reports are alongside them in `results/v3/`. The consolidated machine-readable comparison is `results/v3/comparison.json`.

The AOT sanity check for 14B corrected `Ik heb het document gisteren verstuurt.` to `Ik heb het document gisteren verstuurd.`; its English example was unchanged. The 8B Dutch example was unchanged; its English example became `She had sent the documents yesterday.` This is grammatical but differs from the example's expected `She sent the documents yesterday.` The commands exited successfully and disposed model/context through the CLI's `using` path. These examples are diagnostic, not evidence of general quality.

The 8B/14B figures in this table were produced by an executor that continued after sampled EOS markers; the bounded audit below shows that this materially affected raw outputs. Treat these provisional scores as invalid for model-family selection until the general termination defect is corrected and the same diagnostic cases are rerun. The 3B v2 aggregate also predates the CPU-only backend rebuild and was not rerun for v3. Do not use this table to select a model or claim a quality target. Prompt optimization remains stopped; the next step is approval for the general EOG-stop correction, followed by the bounded regression run described in ADR 0007. No 90% or 95% target has been met or established.

Measurements were made after the model files had been downloaded and hashed, with the operating-system file cache uncontrolled. The reported load time is model-load time inside a running process, not a cold-boot measurement. Semantic preservation was not independently scored; reviewers must inspect the saved traces before any safety claim. The smoke set's seven clean inputs per language cannot establish a 99% preservation rate.

Official sources: [Ministral 3 8B GGUF repository at its pinned revision](https://huggingface.co/mistralai/Ministral-3-8B-Instruct-2512-GGUF/tree/0102285ad796bd99af90f58de616092e5630e970), [8B Q4_K_M file and upstream SHA-256](https://huggingface.co/mistralai/Ministral-3-8B-Instruct-2512-GGUF/blob/0102285ad796bd99af90f58de616092e5630e970/Ministral-3-8B-Instruct-2512-Q4_K_M.gguf), [Ministral 3 14B GGUF repository at its pinned revision](https://huggingface.co/mistralai/Ministral-3-14B-Instruct-2512-GGUF/tree/fb49df4a3cde2c774da8def12437118a66c4f5cf), and [14B Q4_K_M file and upstream SHA-256](https://huggingface.co/mistralai/Ministral-3-14B-Instruct-2512-GGUF/blob/fb49df4a3cde2c774da8def12437118a66c4f5cf/Ministral-3-14B-Instruct-2512-Q4_K_M.gguf).

## LLamaSharp generation-termination audit

**Audit date:** 2026-10-19. This was a bounded runtime investigation, not a new quality benchmark. No prompt, correction parsing, validation, evaluation or runtime code was changed. The four diagnostic inputs below are synthetic and belong to the existing provisional 44-case set.

### Findings

The official 8B and 14B GGUF metadata reports BOS token ID `1` (`<s>`), EOS token ID `2` (`</s>`), `tokenizer.ggml.add_bos_token=true`, and `tokenizer.ggml.add_eos_token=false`. The embedded Mistral template renders the user's text as an `[INST] ... [/INST]` message; it leaves BOS insertion to tokenization and uses `eos_token` as the assistant-turn terminator. LLamaSharp's `PromptTemplateTransformer` obtains the model's embedded template via `LLamaTemplate` and calls native `llama_chat_apply_template` with `addAssistant=true`; `InteractiveExecutor` tokenizes that formatted prompt with `addBos=true` and parses special tokens. A direct native llama.cpp rendering and tokenization produced byte-identical formatted prompt text and identical token IDs for `en-03` (113 tokens). The four native prompt lengths were 113, 112, 112 and 117 tokens, respectively. No separate end-of-turn token is present for these GGUFs; `</s>` is registered by the pinned runtime as an end-of-generation (EOG) token.

The continuation failure is an **LLamaSharp `InteractiveExecutor` integration defect**, not the model template omitting an end marker. In pinned LLamaSharp v0.27.0 source, `InteractiveExecutor.InferInternal` samples an EOG/EOS token, but when the token equals the model EOS it replaces it with the vocabulary newline token before putting it in `_embeds` (`LLama/LLamaInteractExecutor.cs`, `InferInternal`, lines 265–280). Its later `PostProcess` EOG check examines `_embeds.Last()` (`PostProcess`, lines 168–193), which can no longer be EOS after that replacement. With no antiprompt configured, generation therefore continues until `MaxTokens=128`. `MaxTokens` is reached as a fallback, but it does not cause the initial continuation; the sampled EOS is being converted into ordinary text and fed back into the model.

This was observed directly in a weight-backed 8B LLamaSharp run of `en-03`: it yielded 128 token pieces. The sampled sequence contained EOS ID `2` (`IsEndOfGeneration=true`) at generation positions 9, 45, 70, 89, 105 and 125, with generation continuing after each. The first eight generated pieces form `The information is included in the report.`; the following pieces are explanations and repeated variants. The old v3 trace's `en-03`, `en-06`, `en-07` and `nl-09` examples show the same symptom: correct initial repairs followed by Markdown, explanations, repeated blocks or alternatives. The corresponding 8B and 14B raw traces were inspected; the application's frozen parser/validator was left untouched.

### Native comparison

A temporary C++ harness called the pinned llama.cpp C API from the CPU-only libraries built at commit `3f7c29d318e317b63f54c558bc69803963d7d88c`. It used the embedded template, the same prompt strings, context 4096, nine CPU threads, zero GPU layers, greedy temperature-zero selection, and a 128-token cap. It used `kv_unified=true`, matching the LLamaSharp context reported in its native log. Prompt rendering and token IDs were checked byte-for-byte for `en-03`. For each model, llama.cpp stopped naturally on EOG after the following generated text:

| Case | Ministral 8B | Ministral 14B | Generated output tokens before EOG |
| --- | --- | --- | ---: |
| `en-03` | `The information is included in the report.` | `The information is included in the report.` | 8 |
| `en-06` | `The user successfully logged in.` | `The user successfully logged in.` | 6 |
| `en-07` | `They were waiting outside the office.` | `They were waiting outside the office.` | 7 |
| `nl-09` | `Als hij morgen komt, beginnen we om negen uur.` | `Als hij morgen komt, beginnen we om negen uur.` | 12 |

In all eight model/case runs, `llama_vocab_is_eog` ended generation naturally. These are four diagnostic examples per model, not quality evidence. They establish that the same weights, prompts, template, CPU backend and greedy decoding can stop after the intended correction in native llama.cpp; the LLamaSharp interactive executor's EOS-to-newline behavior explains why the application traces continued to the cap. Other model limitations and failures outside these four traces remain unassessed by this bounded audit.

### Baseline comparability caveat (resolved in v4)

The historical 3B v2 run predates commit `ca7a3c2`, which added the Metal-disabled CPU-only native backend. The v3 comparison reused that aggregate, so those old rows were not comparable. This caveat is resolved for the new v4 run, which freshly ran all three models against the same rebuilt backend, cases, pipeline and settings.

The approved general correction uses `StatelessExecutor`, which stops on any token LLamaSharp identifies as EOG before decoding that token as text. It preserves `MaxTokens` as an independent upper bound, and retains strict isolation using a new context per request. See [ADR 0007](decisions/0007-llamasharp-eog-termination-audit.md) for the implementation and test evidence.

## Ministral family v4 diagnostic comparison

**Run date:** 2026-10-20. Models were run sequentially on the same Apple M4 Mac mini, 32 GB unified memory, macOS 26.6.2 ARM64, .NET 10.0.12. All used the Metal-disabled pinned llama.cpp backend `3f7c29d318e317b63f54c558bc69803963d7d88c`, LLamaSharp 0.27.0, CPU-only, nine threads, context 4096, greedy decoding, maximum 128 generated tokens and the frozen `diorthosis-correction-v3` pipeline. Dataset `smoke-44-reviewed-v2.jsonl` hash: `2190c109f5f8e8ce1979637a1beea9fcce615959033aa76576b32ba537465fa2`. Exact model revisions and checksums are listed above. No weights were downloaded during this run.

| Model | NL accepted recall / precision / false-positive edits | EN accepted recall / precision / false-positive edits | False-positive rate NL / EN | Correct-text preservation NL / EN | Rejected generations | Mean / max case latency | Init time | Peak process working set | Termination |
| --- | --- | --- | --- | --- | ---: | ---: | ---: | ---: | --- |
| Ministral 3B Q4_K_M | 11/16 (68.8%) / 11/13 (84.6%) / 2 | 11/16 (68.8%) / 12/20 (60.0%) / 8 | 2/7 (28.6%) / 0/7 (0%) | 5/7 (71.4%) / 7/7 (100%) | 2/44 | 1.373 / 2.043 s | 1.180 s | 2.933 GB | EOG 44/44 |
| Ministral 3 8B Q4_K_M | 12/16 (75.0%) / 13/21 (61.9%) / 8 | 11/16 (68.8%) / 12/22 (54.5%) / 10 | 2/7 (28.6%) / 1/7 (14.3%) | 5/7 (71.4%) / 6/7 (85.7%) | 4/44 | 3.148 / 4.720 s | 1.466 s | 6.269 GB | EOG 44/44 |
| Ministral 3 14B Q4_K_M | 14/16 (87.5%) / 15/20 (75.0%) / 5 | 12/16 (75.0%) / 13/22 (59.1%) / 9 | 1/7 (14.3%) / 0/7 (0%) | 6/7 (85.7%) / 7/7 (100%) | 1/44 | 4.975 / 6.936 s | 2.021 s | 9.525 GB | EOG 44/44 |

All 132 outputs naturally ended after a sampled model EOG token; none used all 128 tokens. Traces under [`results/v4/`](../benchmarks/model-validation/results/v4/) include raw generations, sampled token counts, termination reasons, extracted corrections, accepted edits, latency and per-case evaluation. The four separately inspected termination cases were `en-03`, `en-06`, `en-07`, and `nl-09`; all three models passed the real-model termination test, the output cap test, repeated-request isolation, correct-input preservation, cancellation and recovery.

The 14B model has the strongest smoke recall, but precision is 75.0% Dutch and 59.1% English, and Dutch clean-text preservation is 85.7%. None of the models meets strict precision or preservation expectations. This 44-case set has only 16 provisional errors and 7 provisional clean cases per language, so it remains diagnostic and cannot establish final quality. Recommendation: evaluate EuroLLM 9B next on an independently reviewed dataset; retain 14B as a corrected Ministral baseline for comparison if useful. Do not claim that any accuracy target has been met.

## Reproduction and asset handling

Keep model and temporary conversion files outside the repository (the local validation path used `/private/tmp/diorthosis-model-assets`). For Ministral, download only the named Q4_K_M file from the exact repository revision linked above; do not fetch `mmproj`. Verify it with `shasum -a 256`. For EuroLLM, acquire `model.safetensors`, `config.json`, `generation_config.json`, `special_tokens_map.json`, `tokenizer.json`, `tokenizer.model`, and `tokenizer_config.json` from the pinned source revision and verify the checkpoint SHA-256 above. Using the temporary pinned Python environment and llama.cpp checkout from this validation record, conversion commands are:

```sh
python convert_hf_to_gguf.py /path/to/eurollm-source \
  --outfile /tmp/EuroLLM-1.7B-Instruct-F16.gguf --outtype f16 --use-temp-file
llama-quantize /tmp/EuroLLM-1.7B-Instruct-F16.gguf \
  /path/outside/repository/EuroLLM-1.7B-Instruct-Q4_K_M.gguf Q4_K_M
shasum -a 256 /path/outside/repository/EuroLLM-1.7B-Instruct-Q4_K_M.gguf
```

Compute SHA-256 after acquisition/conversion and compare it with the table above. Use the commands in [`docs/benchmarks.md`](benchmarks.md) and keep temperature, context, prompt, token limit and case-file hash identical between models. The `compare` command rejects mismatched data/settings. The benchmark runner's process peak working set is the reported memory metric; it is not a measurement of system-wide unified-memory pressure.

## 2026-10-20 quality engineering addendum

The [new report](quality-engineering-report.md) separates deterministic replay of four historical model runs from fresh 3B prompt/format diagnostics. The evaluated EuroLLM 9B artifact is documented in [its existing evidence record](../benchmarks/model-validation/results/eurollm-9b-v1/README.md). No official model is selected, no new weights were downloaded, and original historical results remain unchanged.
