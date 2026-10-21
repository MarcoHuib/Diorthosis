# Phase 4: external GPT-6 review preflight and quality checkpoint

2026-10-21. **Do not promote a prompt, change production v3, or merge PR #7.** This note records external AI preflight findings and the currently blocked official import. It is **not** a claim that the existing C# `adjudicate` or all 18 strict batch imports ran.

## Source, identity, coverage

All eighteen user-supplied `diorthosis-review-batch-NN.zip` archives passed ZIP-member and CRC inspection. Their original response files have been retained separately in the accompanying external audit bundle, with unchanged SHA-256 and provenance. Every supplied main decision is marked `reviewerKind=ai`; `independenceDeclared=false`; **zero human approvals**. Distinguish **180 original source cases**, **1,075 unique bound review subjects**, and **920 model generation occurrences**.

The **1,075/1,075 main decisions** passed initial schema, unique-subject and original frozen subject-binding checks, including all eighteen original batch `subjects.jsonl` SHA-256 values. This is **preflight-valid submitted coverage**, **not imported coverage**: official imports executed in the current review environment: **0/18**, imported subjects **0/1,075**, because the existing .NET CLI and repository checkout were not available together.

**Supplementary error-binding defect:** six of 484 `error-decisions.jsonl` entries, all in batch 07, refer to `kind=case` source-annotation subjects instead of allowed `passage` or `rejection` candidate subjects. The current importer should reject these six. **478/484 candidate-annotation records are properly bound.** Original judgments have not been rewritten; a separately named *import-only* derivative with 21 original byte-identical batch-07 records excludes the six and records their exact IDs. The original 27-record file remains unchanged.

| Batch | Main verified | Per-error valid/received |
|---|---:|---:|
| 01 | 68/68 | 25/25 |
| 02 | 52/52 | 22/22 |
| 03 | 67/67 | 21/21 |
| 04 | 56/56 | 29/29 |
| 05 | 56/56 | 31/31 |
| 06 | 51/51 | 12/12 |
| 07 | 51/51 | **21/27** |
| 08 | 55/55 | 19/19 |
| 09 | 52/52 | 46/46 |
| 10 | 71/71 | 37/37 |
| 11 | 70/70 | 22/22 |
| 12 | 55/55 | 26/26 |
| 13 | 62/62 | 36/36 |
| 14 | 59/59 | 16/16 |
| 15 | 60/60 | 37/37 |
| 16 | 70/70 | 23/23 |
| 17 | 62/62 | 30/30 |
| 18 | 58/58 | 25/25 |
| **Total** | **1,075/1,075** | **478/484** |

## Linguistic AI diagnosis (descriptive only)

For each arm, the selection comparisons use the **same 70 original passages and 43 annotated errors per language**. Scores are credited only to eligible delivered outputs. Rejected raw/parsed candidate repairs and source fallback are **not successful delivered repairs**. Unknown is neither success nor failure.

| Selection prompt | NL delivered errors repaired / 43 | EN repaired / 43 | NL acceptable delivered passages / 70 | EN acceptable / 70 |
|---|---:|---:|---:|---:|
| A production v3 | 28 | 30 | 40 | 43 |
| B original instructions + literal JSON | **30** | **33** | **45** | **50** |
| C delimited plain text | 29 | 31 | 40 | 44 |
| M literal JSON + minimal edit | 32 | 33 | 46 | 47 |
| L literal JSON + language checklist | **36** | 33 | 48 | 46 |
| F literal JSON + few-shot | 31 | 32 | 47 | 48 |

Confirmation is a separate locked panel (20 cases and 12 annotated source errors per language): NL A/B **7/9** delivered repairs and **11/14** acceptable passages; EN A/B **9/11** delivered repairs and **12/15** acceptable passages. Descriptive totals on the same 180 original cases A/B across both panels are **74/83** delivered repairs and **106/124** fully acceptable delivered case exposures. These are not independent population estimates or a gross paired-gain/loss decomposition.

Eligible **clean-text preservation** on selection, NL/EN: A 15/14 of 28; B 18/19; C 14/16; M 18/16; L 16/16; F 19/18. Confirmation NL/EN: A 4/3 of eight; B 5/4. Critical semantic failures **still reach delivery**: selection NL A/B/M/L/F 4/4/5/3/2, EN A/B/M/L/F 1/0/1/1/0; confirmation NL A/B one each.

Meaning safety blocks automatic promotion. Concrete accepted damages include NL `hüttenkäse` changed to `huitkase` by A and `hijttenkäse` by B; `hesje` changed to `hemdje`/`heksje`; `goesting in frieten` changed to `geest` or nausea; `waarom wil ik ... doen` losing volition; English `leave the original file where it is` becoming `as it is` under M/L; multilingual `bienvenue` becoming `welkom` alongside existing `welkom`. A/C remove the trema in `conciërge`; C transliterates `καλημέρα`. These are not merely canonical Unicode normalization.

Possible validator false rejections include an otherwise safe `tomorow → tomorrow` repair, whereas a locally repaired `request_id` with dropped backticks and an emoji-removing candidate are legitimate conservative rejects. Local repairs in rejected candidates must be distinguished from entirely safe withheld deliveries.

The original preparer AI covers **154** overlapping subjects; only **53** agree with GPT-6 on all seven nullable judgment flags, while **101** differ on ≥1, often null/inapplicable rubric handling or repeated layers, **not 101 independent linguistic errors**. No matched disagreement in `criticalSemanticFailure`; four meaning flag differences and six new-error flag differences require targeted inspection. Examples include altered `trace_id` markup and disputed Greek `γεια` / `γειά` spelling. No human adjudication is inferred.

## Decision, reproducibility, incomplete work

Literal JSON B remains a reasonable **research substrate**, but its accepted critical damage makes a production promotion unsafe. M and L have worse meaning/clean-text tradeoffs; F has inconsistent cross-language gains and adds prompt cost; combined M+F **was not run**. **Keep v3 unchanged.**

A local archive-integrity/evidence smoke suite of six Python standard-library tests passed. No .NET tests, strict imports, parent C# adjudication, frozen replay, or affected CI runs could be executed in this environment. No imported datasets, gross condition-paired gain/loss table or full disagreement sensitivity ranking should be claimed completed. An accompanying offline bundle provides the original archives, 18 per-batch validation reports, original+derivative supplementary evidence, a provenance-preserving combined AI decision file, descriptive summaries, defect examples, reviewer comparisons, and an importer runner for a real checked-out repository.

**Next at this checkpoint:** run the existing `scripts/import-prompt-quality-review.py` on each corresponding frozen batch with the batch-07 import-only error sidecar, then the existing parent `adjudicate` with empty human adjudications; verify current-head CI and immutable historical checksums before considering any further decision. **Do not merge.**

