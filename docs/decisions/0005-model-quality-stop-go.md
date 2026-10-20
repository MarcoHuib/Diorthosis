# Model quality stop/go decision

**Status:** Stop prompt and correction-engine optimization for Ministral 3B and EuroLLM 1.7B; evaluate stronger European model candidates after explicit weight-download approval. The validated correction pipeline and `diorthosis-correction-v3` remain frozen as the baseline.

**Date:** 2026-10-19

## Evidence

The current evidence is the 44-case `smoke-44-reviewed-v2.jsonl` diagnostic set, which has only provisional labels and is not held-out or statistically representative. The baseline uses the pinned Ministral 3B Q4_K_M checkpoint (revision `be4e90342e4468f37d4bea40ec143997f9256475`, SHA-256 `9ed150d4367e68df0ac8e1540f6ddc65b42d0ee26378329d1ecbca60f93fc5f8`) and EuroLLM 1.7B Q4_K_M conversion documented in [model-validation.md](../model-validation.md). Both are configured at temperature 0, context 4096, 9 CPU threads, 0 GPU layers, 128 maximum output tokens, and the same prompt/data.

| Model/language | Corrected errors / annotated | Recall | Correct / proposed edits | Precision | False-positive edits | Unchanged clean inputs |
| --- | ---: | ---: | ---: | ---: | ---: | ---: |
| Ministral NL | 11 / 16 | 68.8% | 11 / 13 | 84.6% | 2 | 5 / 7 (71.4%) |
| Ministral EN | 11 / 16 | 68.8% | 12 / 20 | 60.0% | 8 | 7 / 7 (100%) |
| EuroLLM NL | 1 / 16 | 6.3% | 1 / 1 accepted | 100% accepted only | 0 accepted | 7 / 7 (100%) |
| EuroLLM EN | 7 / 16 | 43.8% | 8 / 11 | 72.7% | 3 | 7 / 7 (100%) |

These are diagnostic observations only. Sample sizes are 16 annotated errors and 7 clean cases per language, labels remain provisional, and no semantic-preservation score was independently assigned. The 100% accepted precision for Dutch EuroLLM is one accepted edit and must not be read as evidence of high quality. The historical raw/rejected candidate details, latency and sampled process memory are in [benchmark-pipeline-investigation.md](../benchmark-pipeline-investigation.md) and the versioned v2 reports.

The main pattern is a model-quality issue rather than an identified correction pipeline defect: Ministral misses about one third of annotated repairs on this smoke set and makes unjustified changes to two of seven correct Dutch inputs and eight English edit hunks. EuroLLM 1.7B makes very few useful Dutch corrections; its low accepted false-positive count is largely due to validator rejection, not strong raw correction performance. Pipeline code remains frozen unless a reproducible defect is demonstrated.

## Final Ministral prompt hypothesis and execution status

One targeted prompt hypothesis was selected: explicitly state that proofreading is not translation, require the output to remain in the requested language, and ask for the fewest changes while forbidding paraphrase and added information. The hypothesis was that this would reduce full-passage rewrites/translation without suppressing valid corrections. It introduced no language-specific regex or application workaround.

The v4 trial could not produce model generations in this session. The local LLamaSharp backend loaded the GGUF metadata but failed creating a Metal command queue (`ggml_metal_init: failed to create command queue`) and exited before any output. Attempting to remove the Metal plugin from the temporary app output did not load the model. This is an environment/backend launch failure, not a result for the prompt hypothesis. The v4 prompt was therefore reverted; no trial output, score, or prompt improvement is claimed. The baseline prompt, correction code, and evaluator are unchanged. A valid final prompt experiment still requires rerunning the same smoke set with the same exact model/settings on a host where the pinned native backend successfully generates.

## Stop/go recommendation

**STOP OPTIMIZATION AND EVALUATE ALTERNATIVE MODELS.** Do not spend additional work tuning the current two models or adding application heuristics on the basis of this diagnostic set. This recommendation is a resource-allocation decision, not a conclusion that Ministral has failed a reviewed quality gate: smoke data is insufficient for a formal rejection. Ministral may remain a reproducible comparison baseline. The single final prompt hypothesis is documented above but has not been tested successfully.

The product quality goals are at least 90% repair recall per language as an acceptance floor and **95% per language as the aspirational final target**, plus at least 98% correction precision, at least 99% preservation of correct input, no known critical semantic alterations in reviewed cases, and human verification of semantic safety. None is established by these smoke results. Only an independently reviewed, held-out benchmark can support a go decision.

## European-origin alternatives to evaluate

These candidates are research priorities, not selected models. Upstream model cards state Apache 2.0 and identify EU research institutions/funding; those statements are disclosures, not an independent training-data audit or legal opinion. Verify the exact immutable checkpoint, license files/notices, tokenizer/template and any quantizer's provenance before downloading. No weights were downloaded for this decision.

| Candidate | Why investigate | Source/runtime evidence | Practical considerations and unresolved checks |
| --- | --- | --- | --- |
| [EuroLLM-9B-Instruct-2512](https://huggingface.co/utter-project/EuroLLM-9B-Instruct-2512) | Newer, larger EU-funded multilingual instruction model. Card lists Dutch and English, an EU academic/industry consortium and Apache-2.0. | Official checkpoint is Transformers/Safetensors, with tokenizer and chat template. A community [Q4_K_M GGUF conversion](https://huggingface.co/mradermacher/EuroLLM-9B-Instruct-2512-GGUF) is listed for llama.cpp. | Strong next candidate: roughly 5–6 GB Q4 weights by parameter-size estimate, plus KV cache/runtime; estimate is not a measured file/memory size. Verify architecture conversion and the exact GGUF template/quantization before approval. 32 GB Apple Silicon should be plausible, subject to measurement. |
| [EuroLLM-22B-Instruct-2512](https://huggingface.co/utter-project/EuroLLM-22B-Instruct-2512) | Larger version from the same disclosed European consortium; card lists Dutch and English and Apache-2.0. | A community [GGUF repository](https://huggingface.co/mradermacher/EuroLLM-22B-Instruct-2512-GGUF) advertises llama.cpp Q4_K_M files. | Roughly 12–14 GB Q4 weights by parameter-size estimate, with more memory for context/runtime. Likely feasible to evaluate on the 32 GB M4 with a bounded context, but measure peak RAM, speed and thermal behavior. Not yet validated by this app. |
| [EuroMoE-2.6B-A0.6B-Instruct-2512](https://huggingface.co/utter-project/EuroMoE-2.6B-A0.6B-Instruct-2512) | Newer EU consortium model with Dutch/English and Apache-2.0; 2.6B total / 0.6B active. | Upstream describes a Mixtral-style MoE and points to quantizations for llama.cpp-compatible apps. | Small active compute is attractive, but total expert weights still need memory. Treat as lower-priority candidate; confirm that this MoE architecture and routing are supported by the pinned llama.cpp 0.27 backend and verify an exact Q4 GGUF before approval. |
| [Mistral-Small-3.1-24B-Instruct-2503](https://huggingface.co/mistralai/Mistral-Small-3.1-24B-Instruct-2503) | European developer, Apache-2.0 card and substantially larger capability class. | GGUF model conversion is available from community providers; upstream distribution is a multimodal Transformers checkpoint with Mistral's own template. | Card does not list Dutch among its supported 24 languages and model includes vision capability Diorthosis does not need. Lower priority than EuroLLM-9B/22B. 24B Q4 implies roughly 13–15 GB weights, plus context/runtime; likely possible on 32 GB but not yet tested. |

There are third-party GGUF artifacts for EuroLLM 9B/22B and Mistral Small. A repository's license label usually follows the upstream weights; it does not by itself verify the converter, exact quantization, file integrity, tokenizer/template correctness, commercial rights for all artifacts, or llama.cpp 0.27 compatibility. Prefer reproducibly converting the official EuroLLM checkpoint at an immutable revision with pinned temporary tooling when the model architecture is supported; otherwise review the specific conversion output and provenance before using it.

## Next decision gate

1. Obtain independent approval of exact alternative checkpoint/quantization assets and their download sizes before downloading.
2. Verify upstream immutable revisions, license texts and notices, provenance disclosures, GGUF architecture support, chat template and text-only inference path.
3. Run a brief generation/CPU-load check on macOS ARM64 without changing the frozen correction pipeline.
4. If the model loads and generates correctly, compare it against Ministral v3 and EuroLLM 1.7B on the same diagnostic cases/settings. Keep smoke results diagnostic only.
5. Select a candidate for a separately reviewed held-out evaluation; do not claim acceptance from smoke results.

The final prompt experiment remains pending because this host could not start model inference. Revisit it only as that one predeclared hypothesis; do not turn it into iterative tuning against smoke or the provisional 600-case set.
