# Ministral EOG-fixed comparison (v4)

The v2/v3 artifacts are preserved unchanged. V3's 8B/14B outputs were affected by LLamaSharp's EOS-to-newline termination defect, and its 3B aggregate was from a non-comparable earlier backend. These v4 results supersede v2/v3 for comparing the three Ministral sizes.

## Reproduction

Host: Apple M4 Mac mini, 32 GB unified memory; macOS 26.6.2 ARM64; .NET 10.0.12. Runtime: LLamaSharp 0.27.0 with llama.cpp `3f7c29d318e317b63f54c558bc69803963d7d88c`, Metal disabled, nine CPU threads, context 4096, greedy sampling, and 128 maximum output tokens. The source file is `benchmarks/model-validation/smoke-44-reviewed-v2.jsonl`, SHA-256 `2190c109f5f8e8ce1979637a1beea9fcce615959033aa76576b32ba537465fa2`. The v3 prompt, parser, validator and evaluator were not changed.

First run the four-case gate. Then run the full dataset for each model sequentially:

```sh
dotnet run --project src/Diorthosis.Benchmarks -c Release -- run \
  benchmarks/model-validation/smoke-44-reviewed-v2.jsonl /outside/git/Ministral-3-3B-Instruct-2512-Q4_K_M.gguf \
  /tmp/3b-predictions.jsonl /tmp/3b-report.json \
  --model-revision be4e90342e4468f37d4bea40ec143997f9256475 \
  --quantization Q4_K_M --max-output-tokens 128 --trace /tmp/3b-trace.jsonl
```

Repeat with the 8B and 14B GGUFs and revisions recorded in [`docs/model-validation.md`](../../../../docs/model-validation.md). The checked-in `termination-four/cases.jsonl` is selected directly from the full 44-case dataset; its raw response and prediction files are kept for all three sizes.

## Results

| Model | NL recall / precision | EN recall / precision | Clean preservation NL / EN | Rejections | Mean / max latency | Peak working set | EOG stops |
| --- | --- | --- | --- | ---: | ---: | ---: | ---: |
| Ministral 3B Q4_K_M | 68.8% / 84.6% | 68.8% / 60.0% | 71.4% / 100% | 2 | 1.373 / 2.043 s | 2.933 GB | 44/44 |
| Ministral 3 8B Q4_K_M | 75.0% / 61.9% | 68.8% / 54.5% | 71.4% / 85.7% | 4 | 3.148 / 4.720 s | 6.269 GB | 44/44 |
| Ministral 3 14B Q4_K_M | 87.5% / 75.0% | 75.0% / 59.1% | 85.7% / 100% | 1 | 4.975 / 6.936 s | 9.525 GB | 44/44 |

Rejection reasons, false-positive edits, per-language measures and exact timing details are in each report and [`comparison.json`](comparison.json). Each trace retains raw model response, termination reason and sampled token count, extracted correction, accepted edits and case metrics. The four-case gate outputs are in [`termination-four/`](termination-four/).

These 44 diagnostic cases contain 16 provisional errors and 7 provisional clean inputs per language. Annotations are not independently validated and these results do not establish language-correction quality or the 90%/95% recall targets. The 14B model has the best recall, but its precision and Dutch clean-input preservation remain poor; evaluate EuroLLM 9B next on an independently reviewed dataset.
