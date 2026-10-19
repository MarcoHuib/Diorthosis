# Local model validation record

**Validation date:** 2026-10-19 (macOS local run)  
**Status:** Both real-weight candidates completed identical 44-case local smoke runs. Their provisional results are not acceptance evidence. The 600-case candidate is not independently reviewed.

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

Semantic preservation requires human review and remains pending. The provisional cases include names, identifiers, dates, URLs and technical terms, but automated protected-token behavior is not equivalent to human semantic review. Record exact datasets, predictions and run reports by SHA-256 when available. Do not run the 600-case candidate for quality scoring before independent review.

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
