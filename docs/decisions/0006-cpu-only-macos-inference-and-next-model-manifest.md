# ADR 0006: CPU-only macOS ARM64 backend and next model candidates

Date: 2026-10-19

## Decision

On macOS ARM64, build the pinned llama.cpp native backend with `GGML_METAL=OFF` and Apple Accelerate enabled, then replace the macOS ARM64 LLamaSharp CPU package libraries in the published app. `GpuLayerCount = 0` alone does not stop this package's Metal backend from initializing. The build is isolated in [`build-osx-arm64-cpu-backend.sh`](../../scripts/build-osx-arm64-cpu-backend.sh), which pins llama.cpp to `3f7c29d318e317b63f54c558bc69803963d7d88c`, matching LLamaSharp 0.27.0.

This is a build-time native dependency only. Application inference remains in-process, CPU-only, local, and offline. The correction prompt, parser, validator, scoring, and language rules are unchanged.

## Root cause and reproduction

The macOS libraries in `LLamaSharp.Backend.Cpu` 0.27.0 were built with Metal support. `libllama.dylib` has a direct `@rpath/libggml-metal.0.dylib` dependency. The default llama.cpp backend registry initializes compiled backends during model startup. Therefore the `GpuLayerCount = 0` setting avoids layer offload but does not prevent Metal backend initialization. On this host the Metal command queue could not be created; model load failed before generation and the CLI exited with native status 139.

Reproducible comparison on an Apple M4 Mac mini with 32 GB unified memory and macOS 26.6, using the existing local file `Ministral-3-3B-Instruct-2512-Q4_K_M.gguf` (SHA-256 `9ed150d4367e68df0ac8e1540f6ddc65b42d0ee26378329d1ecbca60f93fc5f8`):

| Native runtime | Result |
| --- | --- |
| Published LLamaSharp 0.27.0 CPU package | GGUF metadata/tensors loaded, then `ggml_metal_init: failed to create command queue`; generation did not start; process exited 139. |
| llama.cpp `3f7c29d318e317b63f54c558bc69803963d7d88c`, built with `GGML_NATIVE=OFF`, `GGML_CPU_ARM_ARCH=armv8.2-a`, `GGML_METAL=OFF`, `GGML_BLAS=ON`, `GGML_BLAS_VENDOR=Apple`, `GGML_OPENMP=OFF` | A direct adapter probe returned model-generated `READY` tokens. The replacement dylibs were then installed into the published Native AOT CLI; native startup passed and model inference proposed/applied `Please recieve the signed form.` → `Please receive the signed form.`. The AOT process unloaded and exited normally. This proves inference works with the existing 3B GGUF and the portable CPU-only backend, not correction quality. |

The final run used the published Native AOT CLI. It therefore validates both real token generation and this backend's Native AOT interoperability on this macOS ARM64 host.

## Build and regression coverage

The script verifies macOS ARM64, pinned source, CMake, Ninja and Xcode tools. It builds only llama.cpp shared libraries for a portable ARMv8.2 baseline (rather than specializing for the build Mac), installs them into an already-published `osx-arm64` app, removes unneeded `libmtmd` and Metal libraries, and rejects output if `libllama.dylib` still links Metal. The existing CI macOS ARM64 publish job runs the script before `--check-native-inference`; this turns CI into a repeatable startup regression for the problematic package configuration. A real local GGUF generation run, as above, remains the definitive model-load/generation check.

The script uses temporary source/build directories under `$TMPDIR`; CMake, Ninja and Xcode Command Line Tools are developer/build dependencies, never end-user runtime dependencies. The script requires network access to fetch the exact llama.cpp commit at build time. Application inference itself makes no network calls.

## Immutable candidate manifest

The following files are evaluation candidates only. No weights are included, downloaded, selected, or quality-approved by this decision. Full git commit IDs below were queried from the model repositories on the date above. SHA-256 values and sizes are upstream file metadata, not local download verification.

| Candidate | Pinned source and file | Size, upstream SHA-256 | License, origin and GGUF notes |
| --- | --- | --- | --- |
| Ministral 3 14B Instruct 2512, Q4_K_M | [`mistralai/Ministral-3-14B-Instruct-2512-GGUF`](https://huggingface.co/mistralai/Ministral-3-14B-Instruct-2512-GGUF), revision `fb49df4a3cde2c774da8def12437118a66c4f5cf`; [`Ministral-3-14B-Instruct-2512-Q4_K_M.gguf`](https://huggingface.co/mistralai/Ministral-3-14B-Instruct-2512-GGUF/resolve/fb49df4a3cde2c774da8def12437118a66c4f5cf/Ministral-3-14B-Instruct-2512-Q4_K_M.gguf) | 8.24 GB; `824e0f3373e69b84f2cae46fdcb9bd1ebc6ab3bfc7acc125d818b7b8178cc613` | Official Mistral AI artifact; repository marks Apache-2.0 and recommends llama.cpp. Mistral AI is French. Use the text GGUF only; do not fetch the separate 879 MB `BF16-mmproj.gguf`. GGUF embeds tokenizer metadata and the Mistral 3 chat template. The 3B sibling has already generated with the pinned architecture support; 14B compatibility still needs this app's load/generation gate. |
| EuroLLM 9B Instruct 2512, Q4_K_M | Official source [`utter-project/EuroLLM-9B-Instruct-2512`](https://huggingface.co/utter-project/EuroLLM-9B-Instruct-2512/tree/def82454026b0353f12e48a0de081a9ed380d91e), revision `def82454026b0353f12e48a0de081a9ed380d91e`. Reproducible conversion with llama.cpp `3f7c29d318e317b63f54c558bc69803963d7d88c` is the preferred route. A community candidate is [`mradermacher/EuroLLM-9B-Instruct-2512-GGUF`](https://huggingface.co/mradermacher/EuroLLM-9B-Instruct-2512-GGUF/tree/5138603fccb9dc1f35c25cfd1a4981a78617e9ff), revision `5138603fccb9dc1f35c25cfd1a4981a78617e9ff`, file [`EuroLLM-9B-Instruct-2512.Q4_K_M.gguf`](https://huggingface.co/mradermacher/EuroLLM-9B-Instruct-2512-GGUF/resolve/5138603fccb9dc1f35c25cfd1a4981a78617e9ff/EuroLLM-9B-Instruct-2512.Q4_K_M.gguf). | Community file: 5.58 GB; `fe9b50d4ba67eb0131f1ceeaab2e56ea8fb6b2bbc37391d3219c791c3b140a28`. Official checkpoint: 18.3 GB total across four shards plus tokenizer/config/template. | Official model card states Apache-2.0, Dutch and English, EU funding and an academic/industry consortium including Lisbon, Edinburgh, Paris-Saclay, Amsterdam and Sorbonne. It is text-only. The upstream checkpoint is tagged `llama`; it supplies tokenizer assets and a ChatML-style Jinja chat template. The 1.7B predecessor converted and ran on this same llama.cpp revision; 9B converter and template compatibility still need an actual conversion/generation. The community repo advertises GGUF and llama.cpp compatibility and carries an upstream Apache-2.0 label, but the actual artifact has not been locally downloaded or inspected; prefer reproducible conversion from official weights and verify resulting GGUF/template/checksum. |
| Ministral 3 8B Instruct 2512, Q4_K_M | [`mistralai/Ministral-3-8B-Instruct-2512-GGUF`](https://huggingface.co/mistralai/Ministral-3-8B-Instruct-2512-GGUF), revision `0102285ad796bd99af90f58de616092e5630e970`; [`Ministral-3-8B-Instruct-2512-Q4_K_M.gguf`](https://huggingface.co/mistralai/Ministral-3-8B-Instruct-2512-GGUF/resolve/0102285ad796bd99af90f58de616092e5630e970/Ministral-3-8B-Instruct-2512-Q4_K_M.gguf) | 5.2 GB; `33e7a72cf5e6e2cfc2f2847075acc013d68bba023e35310cef86b5cf8fdca761` | Official Mistral AI artifact; repository marks Apache-2.0 and recommends llama.cpp. French-origin provider. Use text GGUF only; omit separate BF16 vision projector (858 MB). GGUF embeds tokenizer and Mistral 3 chat metadata. Runtime architecture compatibility is likely shared with tested Ministral 3B, but must be confirmed by loading and generation. |

Apache-2.0 permits commercial use and redistribution subject to its conditions, including retaining the license and notices and marking modified files. Before redistributing a model, preserve the upstream license, model card and attribution notices alongside the file and verify the model's actual repository contents at the pinned revision. This is a repository-level license review, not a legal opinion or an independent audit of training-data rights.

### Memory and disk planning (estimates, not measurements)

At an identical 4,096-token context and CPU-only inference, rough planning ranges for Apple M4 with 32 GB unified memory are: Ministral 8B **8–11 GB** process working set; EuroLLM 9B **9–13 GB**; Ministral 14B **12–17 GB**. These allowances include quantized weights, KV cache and runtime/compute buffers; actual resident/peak memory depends on llama.cpp build, context/batch settings, prompt length and OS pressure. The 14B and 9B candidates should fit in 32 GB at this bounded context in principle, but memory and speed are not validated until measured. Full advertised context lengths are out of scope for this device evaluation.

The three final GGUF files require about **19.0 GB** combined (8.24 + 5.58 + 5.2 GB). For EuroLLM's reproducible conversion, reserve at least **45 GB additional temporary space** for the 18.3 GB checkpoint, F16 GGUF intermediate, Q4 output, and conversion workspace. Staging all final candidates and converting EuroLLM in one session can peak around **65 GB** before unrelated caches; do not start until explicit download approval. Conversion tooling is pinned separately in `benchmarks/model-validation/converter-requirements.lock` and is temporary developer tooling only.

## Bounded evaluation protocol

Freeze correction pipeline v3, its prompt, validator and scoring code. Use identical context (4,096), temperature (0), output-token limit, CPU thread policy and exact same 44 diagnostic cases for all candidates.

1. After explicit weight approval, verify candidate file SHA-256, metadata, tokenizer and template; record model file size and repository revision.
2. Run a short deterministic technical generation request and one Dutch and English correction request. Fail this gate if loading, generation, output parsing or disposal fails. Capture cold model-load time, first-token/whole-response latency and peak resident memory.
3. Run the existing 44-case diagnostic smoke dataset exactly once per candidate using the frozen pipeline. Compare against the current Ministral 3B and EuroLLM 1.7B baseline with repair recall, precision, false positives, correct-input preservation, rejected generations and semantic-review flags. Record raw model output separately from accepted edits.
4. Treat these cases only as debugging/screening evidence. Do not optimize prompts or add model-specific application logic to improve results. Advance only promising candidates to a separately approved, independently reviewed held-out selection set.

No 44-case result, generated reference answer or unreviewed case establishes the 90% project target or the aspirational 95% final repair-recall target. Do not enlarge or retune against the held-out dataset in this bounded evaluation.

## Initial evaluation order

1. **Ministral 3 14B Instruct Q4_K_M**: highest-capacity candidate and primary check of whether moving up within the existing Mistral 3 architecture resolves the quality ceiling. About 8.24 GB storage; estimated 12–17 GB RAM at 4k context.
2. **EuroLLM 9B Instruct 2512 Q4_K_M**: distinct, EU-funded multilingual line with Dutch and English explicitly listed; acquire official checkpoint and convert reproducibly unless the community artifact's integrity/template is independently verified. About 18.3 GB source checkpoint and at least 45 GB temporary conversion space; final GGUF estimated at 5.58 GB from the pinned community artifact's upstream file metadata.
3. **Ministral 3 8B Instruct Q4_K_M**: lower-memory/speed alternative and same-family size point; about 5.2 GB storage, estimated 8–11 GB RAM.

If storage or run time requires a staged gate, test 14B first; test EuroLLM only if the reproducible conversion gate succeeds; test 8B if the 14B's quality/latency/memory tradeoff is not suitable. No candidate is approved until actual results are reviewed.
