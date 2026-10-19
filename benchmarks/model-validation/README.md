# Model validation datasets

This directory separates six prompt-development cases, a 44-case smoke set used during integration, and a 600-case provisional evaluation candidate.

## Status

`development.jsonl` contains prompt-tuning examples. `smoke-44.jsonl` contains 22 Dutch and 22 English cases intended for an initial, manually reviewable model smoke run. This smoke set has been used to troubleshoot and tune prompt behavior; its annotations have not been independently reviewed, so it is not held-out evaluation data. It is therefore a smoke set, not a validated held-out evaluation set.

`provisional-600.jsonl` contains 600 rights-cleared synthetic cases: 300 Dutch and 300 English. Per language it contains 75 spelling, 75 grammar, 37 sentence-structure, 38 punctuation, and 75 correct-text cases. Inputs include short examples and synthetic context paragraphs, plus technical terms, identifiers, dates, names, and URLs. The cases are constructed from deterministic templates and injected errors; none came from confidential documents or the evaluated models. The references and annotations are **provisional and unreviewed**. Some templates use repeated constructions by design. A fluent independent Dutch/English reviewer must verify every input, error span, category, and accepted correction before this file is used to make model-quality claims. It is a reviewable candidate, not an accepted benchmark or statistically representative language sample.

UTF-16 offsets and JSONL schema are validated by the existing benchmark evaluator. Prompt-development examples must remain separate from the eventual held-out evaluation set. Reviewers should record accepted/rejected cases and produce a new versioned dataset; do not edit a dataset after looking at model predictions and then reuse its old score.

## Local smoke procedure

Model runs use the same input file, `diorthosis-correction-v3`, temperature 0, context 4096, CPU layer count 0, and maximum output of 128 tokens. Do not run the 600-case file until an independent review approves it. Keep model weights and temporary assets outside Git; the synthetic smoke predictions and run reports are committed under `results/` for review.

```sh
dotnet run --project src/Diorthosis.Benchmarks -c Release -- run \
  benchmarks/model-validation/smoke-44.jsonl /local/path/model.gguf \
  /tmp/predictions.jsonl /tmp/report.json \
  --model-revision <immutable-revision> --quantization Q4_K_M --max-output-tokens 128
```

The 44-case smoke run is only a pipeline/inference check. Its provisional labels do not support claims about the 90% target. Semantic preservation and disputed corrections need blinded human review. The benchmark runner records timing and process peak working set, not an independent measurement of total physical memory or semantic safety.
