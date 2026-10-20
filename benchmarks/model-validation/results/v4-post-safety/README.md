# v4 post-safety replay

This is a non-destructive replay of the 132 stored v4 raw generations through the current deterministic correction pipeline. No GGUF was loaded and no inference ran. Historical `results/v4/` files and gold annotations remain unchanged.

Dataset: `smoke-44-reviewed-v2.jsonl`, SHA-256 `2190c109f5f8e8ce1979637a1beea9fcce615959033aa76576b32ba537465fa2`.

## Historical versus post-safety accepted metrics

Percentages are recall / hunk precision / clean-input preservation. Historical values are the frozen v4 reports; post-safety values are recalculated from the replayed validated edits with the versioned historical evaluator.

| Model | Dutch historical | Dutch post-safety | English historical | English post-safety |
| --- | --- | --- | --- | --- |
| Ministral 3B | 68.8% / 84.6% / 71.4% | 68.8% / 84.6% / 71.4% | 68.8% / 60.0% / 100% | 68.8% / 60.0% / 100% |
| Ministral 3 8B | 75.0% / 61.9% / 71.4% | 75.0% / 61.9% / 71.4% | 68.8% / 54.5% / 85.7% | 68.8% / 57.1% / 100% |
| Ministral 3 14B | 87.5% / 75.0% / 85.7% | 87.5% / 75.0% / 85.7% | 75.0% / 59.1% / 100% | 75.0% / 59.1% / 100% |

The 8B English change reflects replay/parser/evaluator behavior and is not a new model result.

## Current v2 quality metrics

These are current replay metrics, separate from historical hunk rows: error-level recall / strict complete-passage acceptability / historical hunk precision / clean preservation.

| Model | Dutch | English |
| --- | --- | --- |
| Ministral 3B | 68.8% / 68.2% / 84.6% / 71.4% | 68.8% / 77.3% / 60.0% / 100% |
| Ministral 3 8B | 75.0% / 72.7% / 61.9% / 71.4% | 68.8% / 77.3% / 57.1% / 100% |
| Ministral 3 14B | 87.5% / 86.4% / 75.0% / 85.7% | 75.0% / 81.8% / 59.1% / 100% |

Disposition counts across 44 cases:

- 3B: 26 accepted, 16 unchanged, 2 rejected, 0 suppressed, 0 needs-review.
- 8B: 30 accepted, 9 unchanged, 5 rejected, 0 suppressed, 0 needs-review.
- 14B: 30 accepted, 12 unchanged, 2 rejected, 0 suppressed, 0 needs-review.

## Replay findings

- Previously accepted unsafe modification blocked: 8B English `en-17` (`node-04` → `node 04`), rejected by protected-content validation.
- Previously accepted valid correction blocked: none under the deterministic evaluator; this is not independent human adjudication.
- Valid corrections newly accepted: none.
- Rejected outputs without historical accepted edits: 3B `nl-20`, `en-20`; 8B `nl-20`, `nl-21`, `en-20`, `en-21`; 14B `nl-20`, `nl-21`.
- Accepted changes requiring human review: 3B `nl-17`, `nl-22`; 8B `nl-02`, `nl-03`, `nl-12`, `nl-19`, `nl-22`; 14B `nl-12`, `nl-19`.
- Suppressed outputs: none in these stored generations. Unchanged outputs are recorded per case in each report.

The per-case records are in each `*-trace.jsonl` and the `replayComparisons` section of each report. “Valid” and “unsafe” above describe deterministic classification or historical evaluator credit, not independent human semantic adjudication.

## Integrity and recommendation

Each post-safety trace and prediction file contains 44 cases; the three replays account for all 132 stored generations. The source dataset hash remains unchanged, and no frozen v4, audit, or gold file was overwritten.

The pipeline is suitable for an independently reviewed benchmark pilot, but it is not itself evidence of production quality or semantic equivalence. Human review remains required for accepted changes, ambiguous restructuring, and any safety-sensitive rejection before using a release-quality benchmark.
