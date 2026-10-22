# Per-panel external AI comparisons

Every attempt is retained. `n + u?` means n known positive outcomes and u unknowns, not n+u successes. Local repairs do not establish passage success. Safe repair additionally requires meaning and no-new-error=true. Sound passage requires meaning/no-new-error plus all original defects repaired (or cleanPreserved on clean source). Strict full success additionally requires justified=true. Abstention never earns repair, passage success or eligible clean preservation. Source clean/error partitions remain frozen; source-review rationales may dispute labels.

## selection / dutch

| Condition | Attempts | Error/clean passages | Local repairs / errors | Safe repairs | Missed / unknown local | Clean preserved / clean | Sound passages | Strict full | Meaning failures | New errors | Critical | Unnecessary | Valid necessary withheld |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A production v3 | 70 | 42/28 | 28/43 | 26 + 1? | 15/0 | 15/28 | 40 + 1? | 35 + 4? | 4 + 1? | 5 | 4 + 1? | 7 | 0 |
| B literal original | 70 | 42/28 | 30/43 | 28 + 1? | 13/0 | 18/28 | 45 + 1? | 40 + 4? | 4 + 1? | 4 | 4 + 1? | 6 | 0 |
| C plain text | 70 | 42/28 | 29/43 | 27 + 1? | 14/0 | 14/28 | 40 + 1? | 36 + 3? | 4 + 1? | 6 + 1? | 3 + 1? | 9 | 0 |
| D minimal | 70 | 42/28 | 32/43 | 29 + 1? | 11/0 | 18/28 | 46 + 1? | 41 + 4? | 7 + 1? | 7 | 5 + 2? | 9 | 0 |
| E language checklist | 70 | 42/28 | 36/43 | 33 + 1? | 7/0 | 16/28 | 48 + 1? | 44 + 4? | 6 + 1? | 6 + 1? | 3 + 2? | 8 | 0 |
| F few-shot | 70 | 42/28 | 31/43 | 29 + 1? | 12/0 | 19/28 | 47 + 1? | 42 + 5? | 3 + 1? | 3 + 1? | 2 + 2? | 5 | 0 |
## selection / english

| Condition | Attempts | Error/clean passages | Local repairs / errors | Safe repairs | Missed / unknown local | Clean preserved / clean | Sound passages | Strict full | Meaning failures | New errors | Critical | Unnecessary | Valid necessary withheld |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A production v3 | 70 | 42/28 | 30/43 | 30 | 13/0 | 14/28 | 43 | 37 + 6? | 1 | 1 | 1 | 3 + 1? | 6 + 1? |
| B literal original | 70 | 42/28 | 33/43 | 32 + 1? | 10/0 | 19/28 | 50 + 1? | 42 + 9? | 0 | 0 + 1? | 0 | 2 + 1? | 4 |
| C plain text | 70 | 42/28 | 31/43 | 29 + 2? | 12/0 | 16/28 | 44 + 2? | 36 + 10? | 0 + 1? | 0 + 2? | 0 + 1? | 0 + 2? | 6 + 1? |
| D minimal | 70 | 42/28 | 33/43 | 32 + 1? | 10/0 | 16/28 | 47 + 1? | 40 + 8? | 1 | 1 + 1? | 1 | 3 + 1? | 4 |
| E language checklist | 70 | 42/28 | 33/43 | 31 + 2? | 10/0 | 16/28 | 46 + 2? | 39 + 9? | 1 + 1? | 1 + 2? | 1 + 1? | 3 + 2? | 4 |
| F few-shot | 70 | 42/28 | 32/43 | 31 + 1? | 11/0 | 18/28 | 48 + 1? | 39 + 10? | 0 | 0 + 1? | 0 | 1 + 1? | 4 + 1? |
## confirmation / dutch

| Condition | Attempts | Error/clean passages | Local repairs / errors | Safe repairs | Missed / unknown local | Clean preserved / clean | Sound passages | Strict full | Meaning failures | New errors | Critical | Unnecessary | Valid necessary withheld |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A production v3 | 20 | 12/8 | 7/12 | 7 | 5/0 | 4/8 | 11 | 9 + 2? | 2 | 2 | 1 + 1? | 2 | 0 |
| B literal original | 20 | 12/8 | 9/12 | 9 | 3/0 | 5/8 | 14 | 11 + 3? | 1 | 1 | 1 | 1 | 0 |
## confirmation / english

| Condition | Attempts | Error/clean passages | Local repairs / errors | Safe repairs | Missed / unknown local | Clean preserved / clean | Sound passages | Strict full | Meaning failures | New errors | Critical | Unnecessary | Valid necessary withheld |
|---|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|---:|
| A production v3 | 20 | 12/8 | 9/12 | 9 | 3/0 | 3 + 1?/8 | 12 + 1? | 9 + 4? | 0 | 0 | 0 | 0 + 1? | 1 |
| B literal original | 20 | 12/8 | 11/12 | 11 | 1/0 | 4 + 1?/8 | 15 + 1? | 12 + 4? | 0 | 0 | 0 | 0 + 1? | 1 |

## Dispositions and resources

| Panel/language/condition | Accepted | Unchanged | Rejected | Needs review | Suppressed | Mean prompt tokens | Median latency ms | Peak RSS bytes |
|---|---:|---:|---:|---:|---:|---:|---:|---:|
| confirmation/dutch/A production v3 | 9 | 7 | 3 | 0 | 1 | 123.1 | 1406 | 2954444800 |
| confirmation/dutch/B literal original | 10 | 8 | 0 | 0 | 2 | 118.7 | 1400 | 2948120576 |
| confirmation/english/A production v3 | 10 | 3 | 7 | 0 | 0 | 118.5 | 1319 | 2950856704 |
| confirmation/english/B literal original | 12 | 4 | 4 | 0 | 0 | 114.3 | 1302 | 2948300800 |
| selection/dutch/A production v3 | 33 | 22 | 13 | 1 | 1 | 122.1 | 1530 | 2964930560 |
| selection/dutch/F few-shot | 34 | 24 | 10 | 1 | 1 | 217.0 | 1838 | 2983297024 |
| selection/dutch/E language checklist | 41 | 18 | 9 | 2 | 0 | 176.0 | 1698 | 2976989184 |
| selection/dutch/D minimal | 38 | 22 | 8 | 2 | 0 | 188.0 | 1689 | 2978856960 |
| selection/dutch/B literal original | 34 | 24 | 10 | 1 | 1 | 119.0 | 1483 | 2958868480 |
| selection/dutch/C plain text | 36 | 19 | 14 | 1 | 0 | 188.3 | 1781 | 2970779648 |
| selection/english/A production v3 | 33 | 17 | 16 | 2 | 2 | 118.6 | 1342 | 2963668992 |
| selection/english/F few-shot | 34 | 21 | 11 | 2 | 2 | 190.1 | 1585 | 2975531008 |
| selection/english/E language checklist | 36 | 18 | 14 | 1 | 1 | 158.1 | 1497 | 2969894912 |
| selection/english/D minimal | 37 | 18 | 12 | 1 | 2 | 184.1 | 1580 | 2974728192 |
| selection/english/B literal original | 35 | 22 | 10 | 1 | 2 | 115.1 | 1340 | 2958737408 |
| selection/english/C plain text | 32 | 17 | 17 | 3 | 1 | 184.0 | 1587 | 2972254208 |

## Paired gains/losses

Gains/losses are desirable outcomes for repairs and sound passages. Unknown pairs are not ties. Exact IDs and descriptive intervals are retained in paired.json.

| Panel/language | Control → variant | Local repair gains/losses | Safe repair gains/losses/unknown | Sound passage gains/losses/unknown | Clean gains/losses/unknown |
|---|---|---:|---:|---:|---:|
| selection/dutch | A production v3 → B literal original | 2/0/0 | 2/0/1 | 5/0/1 | 3/0/0 |
| selection/dutch | B literal original → F few-shot | 2/1/0 | 2/1/1 | 3/1/1 | 1/0/0 |
| selection/dutch | B literal original → E language checklist | 6/0/0 | 5/0/1 | 5/2/1 | 0/2/0 |
| selection/dutch | B literal original → D minimal | 2/0/0 | 1/0/1 | 1/0/1 | 0/0/0 |
| selection/dutch | B literal original → C plain text | 1/2/0 | 0/1/1 | 1/6/1 | 1/5/0 |
| selection/english | A production v3 → B literal original | 3/0/0 | 3/0/1 | 8/0/1 | 5/0/0 |
| selection/english | B literal original → F few-shot | 0/1/0 | 0/1/1 | 1/3/1 | 1/2/0 |
| selection/english | B literal original → E language checklist | 1/1/0 | 0/1/2 | 0/4/2 | 0/3/0 |
| selection/english | B literal original → D minimal | 0/0/0 | 0/0/1 | 0/3/1 | 0/3/0 |
| selection/english | B literal original → C plain text | 1/3/0 | 0/3/2 | 1/7/2 | 1/4/0 |
| confirmation/dutch | A production v3 → B literal original | 2/0/0 | 2/0/0 | 4/1/0 | 2/1/0 |
| confirmation/english | A production v3 → B literal original | 2/0/0 | 2/0/0 | 3/0/1 | 1/0/1 |

New-error booleans include semantic, formatting and linguistic damage. No standardized introduced-spelling/grammar count can be recovered from heterogeneous reviewer categories; metrics.json exposes an explicit-tag lower bound and the full category evidence remains in occurrences.jsonl. Do not relabel source-error categories as newly introduced errors.

Alternative repairs absent from references are in local-repairs-vs-reference.jsonl; only records with wholePassageSound=true support a sound whole-passage alternative. The 19 local credits include deleted modality, uncertain aspect and singularized pumps, which are not 19 valid corrections.

All 478 unique bound candidate/error judgments are reused across 564 annotated-error generation occurrences and separate layers; repetition does not increase independent evidence. Zero human approvals.
