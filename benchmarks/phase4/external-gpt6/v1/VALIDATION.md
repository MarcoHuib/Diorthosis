# External batch validation

Main decisions and candidate-error coverage are counted separately. All human approvals: **0**.

| Batch | Expected | Received | Valid | Missing | Scope errors | Candidate errors | Coverage | Import |
|---|---:|---:|---:|---:|---:|---:|---|---|
| batch-01 | 68 | 68 | 68 | 0 | 0 | 25 | complete | imported |
| batch-02 | 52 | 52 | 52 | 0 | 0 | 22 | complete | imported |
| batch-03 | 67 | 67 | 67 | 0 | 0 | 21 | complete | imported |
| batch-04 | 56 | 56 | 56 | 0 | 0 | 29 | complete | imported |
| batch-05 | 56 | 56 | 56 | 0 | 0 | 31 | complete | imported |
| batch-06 | 51 | 51 | 51 | 0 | 0 | 12 | complete | imported |
| batch-07 | 51 | 51 | 51 | 0 | 6 | 21 | complete | imported; source-only supplementary records quarantined |
| batch-08 | 55 | 55 | 55 | 0 | 0 | 19 | complete | imported |
| batch-09 | 52 | 52 | 52 | 0 | 0 | 46 | complete | imported |
| batch-10 | 71 | 71 | 71 | 0 | 0 | 37 | complete | imported |
| batch-11 | 70 | 70 | 70 | 0 | 0 | 22 | complete | imported |
| batch-12 | 55 | 55 | 55 | 0 | 0 | 26 | complete | imported |
| batch-13 | 62 | 62 | 62 | 0 | 0 | 36 | complete | imported |
| batch-14 | 59 | 59 | 59 | 0 | 0 | 16 | complete | imported |
| batch-15 | 60 | 60 | 60 | 0 | 0 | 37 | complete | imported |
| batch-16 | 70 | 70 | 70 | 0 | 0 | 23 | complete | imported |
| batch-17 | 62 | 62 | 62 | 0 | 0 | 30 | complete | imported |
| batch-18 | 58 | 58 | 58 | 0 | 0 | 25 | complete | imported |

Batch 07 includes six explicitly source-only per-error entries. Their exact records remain in the original file and validation report. A byte-preserving candidate-only subset is imported; source records are never counted as candidate repairs. No ID, hash, rationale or judgment is rewritten.

Supplementary-file hashes were declared in batches 01, 02 and 18 and match. Other batches did not declare one; computed hashes establish retained-byte identity, not a retrospective reviewer declaration. All required main/provenance hashes match.
