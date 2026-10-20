# Stored-trace analysis (development diagnostic)

Reference matches, validator outcomes, and historical AI labels are separate. No model ranking or population generalization. Rejected unchanged delivery is not an assessable successful correction.

| Model | Parsed | Delivered assessable | Rejected | Needs review |
|---|---:|---:|---:|---:|
| ministral-3b | 44/44 | 42/44 | 2/44 | 0/44 |
| ministral-3-8b | 44/44 | 39/44 | 5/44 | 0/44 |
| ministral-3-14b | 44/44 | 42/44 | 2/44 | 0/44 |
| eurollm-9b | 44/44 | 44/44 | 0/44 | 0/44 |

All 176 outputs are linked to exact trace/report hashes and model revisions in failure-analysis.jsonl. Original traces and timings were not altered. Independent linguistic quality remains unknown. See analysis.json for reference symptoms, all taxonomy categories, paired case IDs and shared misses.

Inventory: 387 cases; 685 blinded, content-bound review subjects. All need human sign-off. No source-family independence or document count is invented.
