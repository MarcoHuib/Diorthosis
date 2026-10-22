# Phase 4: complete external GPT-6 evidence reconciliation

2026-10-21. **Keep production v3 unchanged. No prompt is promoted, no PR is merged, and no next development phase begins.** All eighteen external review batches have now passed the existing strict importer and C# adjudicator. Literal-Unicode JSON with original instructions is the strongest balanced research substrate, especially for English; the language checklist has the highest Dutch repair and sound-passage counts, while few-shot best preserves Dutch clean text. None satisfies the meaning-safety gate. These are external **AI diagnostic assessments**, with declared non-independence and zero human approvals, not a release-quality claim.

This completes the work left open by the [earlier preflight](phase4-external-gpt6-review-preflight.md). That note remains a historical account of an environment that could not run .NET. Its “acceptable” counts correspond to the **sound-passage** criterion below, not the stricter conjunction that also requires an explicit justified=true judgment.

## Coverage, integrity and the six quarantined records

The [validation matrix](../benchmarks/phase4/external-gpt6/v1/VALIDATION.md) records every batch's expected, received, valid and missing subjects, member/packet hashes, error coverage and import status. **1,075/1,075 unique bound subjects are validly imported; zero are missing, unexpected or duplicated.** They cover 180 original passages and map to 920 already-saved generation occurrences. There are 180 source-annotation, 287 raw, 275 parsed, 247 delivered and 86 rejected subjects. These are distinct counts, not interchangeable sample sizes. No new model generation, replacement assessment or human adjudication was made.

All original ZIPs and all three extracted members are retained byte-for-byte under [batch-specific originals](../benchmarks/phase4/external-gpt6/v1/batches/). Extraction permits exactly the three unique regular root files, checks CRC, bounds sizes and rejects traversal, links, encryption, duplicate/unexpected members and nonregular files. Each original packet's count, subject hash, public member bytes and ZIP are checked. Main JSON uses the existing strict duplicate-property/non-finite rejection, exact schema, boolean/null/string/array checks, Unicode validity, version 1, actual reviewer/language identity and recomputed byte-framed subject bindings. Required provenance hashes match exact bytes. Actual sets establish coverage; declared completion is not sufficient.

There are **484 submitted supplementary records**, of which **478/478 expected candidate/error pairs** are valid. Batch 07 has six additional, correctly hashed **source-only** annotation records. Their subject IDs, annotation indices, rationales and repaired=false values describe originals with no candidate. The violated contract is `kind=case` where per-error candidate assessments require `passage` or `rejection`. The exact file/line/ID/contract/explanation is retained in [batch-07 validation](../benchmarks/phase4/external-gpt6/v1/batches/batch-07/validation.json). No rebinding or judgment rewrite is defensible. A separately named derivative simply retains the 21 original candidate lines, with their original bytes and order; the full 27-line submission and archive remain intact. The six extras do not enter repair metrics. All other batches import their original supplementary bytes directly.

Batches 01, 02 and 18 declare supplementary-file SHA-256 values, and all match. The other fifteen do not declare that optional field. Their computed hashes bind the retained evidence but are **not invented retrospective reviewer declarations**. All required main-decision, request and subject hashes match. Separate reviewer identities, exact-build unavailability, evaluation requests, prior exposure and limitations remain in each original provenance; no combined reviewer session or independent reviewer count is invented. Every decision remains `reviewerKind=ai`, `independenceDeclared=false`. Identity and exposure statements are declarations, not authentication.

All eighteen imports used `scripts/import-prompt-quality-review.py` with `--error-decisions`, against the matching frozen child directory. The [combined dataset](../benchmarks/phase4/external-gpt6/v1/combined/analysis-dataset.jsonl) preserves each subject, decision, batch, original line and provenance hash; its [provenance index](../benchmarks/phase4/external-gpt6/v1/combined/provenance-index.json) points to eighteen separate originals. Concatenation preserves original main-decision line bytes. The existing C# `adjudicate` ran on every child and on the parent with its original lineage and an empty human-adjudication file. [Combined C# report](../benchmarks/phase4/external-gpt6/v1/combined/import-report.json): **zero human-finalized metrics**. The custom AI projection below is explicitly separate from those human-only metrics.

## What the metrics mean

[Complete condition/language comparison](../benchmarks/phase4/external-gpt6/v1/analysis/COMPARISON.md), [machine metrics](../benchmarks/phase4/external-gpt6/v1/analysis/metrics.json), [920 occurrence records](../benchmarks/phase4/external-gpt6/v1/analysis/occurrences.jsonl), [564 annotated-error occurrence records](../benchmarks/phase4/external-gpt6/v1/analysis/error-occurrences.jsonl) and [paired analysis](../benchmarks/phase4/external-gpt6/v1/analysis/paired.json) provide all denominators, exact source IDs, subjects, judgments, rejections and resources. The 478 unique error judgments apply to deduplicated subjects across layers; they are reused across occurrences, not independent observations.

Selection has 70 passages per language: 42 annotated erroneous, 28 proposed clean and 43 annotated errors. Confirmation has 20 per language: 12 erroneous, eight clean and twelve errors. These are the **frozen partitions**, not freshly validated human labels. Both malformed Windows-path fixtures and all unsuccessful cases remain in their original denominators and texts. Their control characters preclude claiming valid-path preservation. Source rationales and errata remain available; no source is silently repaired or relabeled.

A local error is counted repaired only from a bound supplementary repaired=true assessment of an eligible delivered subject. Rejected/needs-review/suppressed source fallback cannot repair it. The missed total includes annotated errors not repaired through abstention, with abstained-error totals separately exposed. No unknown eligible judgment becomes true or false. All local delivered-error judgments happen to be known here; other criteria have substantial null coverage.

**Safe local repair** additionally requires meaningPreserved=true and noNewErrors=true for the delivered passage. It does not require every other original defect to have been repaired. **Sound passage** requires meaning/no-new-error plus allDefectsRepaired on erroneous input, or cleanPreserved on clean input. **Strict fully acceptable passage** also requires justified=true. On a clean source, allDefectsRepaired is often null because it is inapplicable; requiring it would manufacture missing repair evidence. A critical=true candidate cannot pass either passage criterion. False defeats unknown in a conjunction; otherwise unresolved necessary fields remain unknown. All attempted passages remain in the denominator, including abstentions. We also retain each criterion independently.

Clean preservation uses the explicit cleanPreserved judgment and delivery eligibility; literal preservation is reported separately. NFC/NFD equivalence need not be a spelling error, while an unnecessary but equivalent normalization can still violate an exact-return instruction. Unnecessary modifications are a disclosed diagnostic derived from changed delivered text plus explicit unjustified/unnecessary reviewer evidence; unknown necessity remains unknown. `noNewErrors=false` covers linguistic, semantic and formatting damage. The submissions lack a standardized new-spelling-versus-new-grammar field: exact type-specific totals are **unavailable**, not zero. Metrics retain the heterogeneous categories and an explicit introduced-spelling/grammar-tag lower bound. Source-error categories are never automatically counted as newly introduced errors.

## Selection results

`+u?` denotes u unknown outcomes, never additional successes. Each row attempts the same 70 source passages per language. M/L/F use literal JSON; the unrun combined minimal/few-shot condition has **no results**.

| Condition | NL local repairs /43 | EN local repairs /43 | NL sound /70 | EN sound /70 | NL strict full /70 | EN strict full /70 | NL/EN clean preserved /28 |
|---|---:|---:|---:|---:|---:|---:|---:|
| A production v3 | 28 | 30 | 40 +1? | 43 | 35 +4? | 37 +6? | 15 / 14 |
| B literal JSON, original | 30 | 33 | 45 +1? | 50 +1? | 40 +4? | 42 +9? | 18 / 19 |
| C plain block, original | 29 | 31 | 40 +1? | 44 +2? | 36 +3? | 36 +10? | 14 / 16 |
| M minimal edit | 32 | 33 | 46 +1? | 47 +1? | 41 +4? | 40 +8? | 18 / 16 |
| L language checklist | 36 | 33 | 48 +1? | 46 +2? | 44 +4? | 39 +9? | 16 / 16 |
| F few-shot | 31 | 32 | 47 +1? | 48 +1? | 42 +5? | 39 +10? | 19 / 18 |

B versus A has **2/0 Dutch and 3/0 English paired local-error gains/losses**. These are actual linguistic annotation judgments, not the old reference-only 2/0 and 3/0 counts by coincidence. Safe-repair totals are A NL26+1?/EN30 versus B NL28+1?/EN32+1?: B has three known English safe gains but one previously sound multilingual-formatting case becomes unknown. Its net English safe count rises by two; that unknown cannot be assumed a lost or successful repair. Dutch aspect uncertainty affects both controls equally. B's known sound-passage gains/losses are NL **5/0**, EN **8/0**, with one unknown pair each. Clean-preservation gains/losses are **3/0** and **5/0**. B therefore improves both languages on these descriptive criteria; it is not merely a reference-score improvement.

Concrete B gains preserve backticks while repairing Dutch `wachtworod` and English `request_id ... have`, repair Dutch plural `bestaat`, deliver English `interested in` and `doesn't have` without the guarded apostrophe changes, and preserve clean identifiers, names and multilingual text. A's accepted change from `naïve_mode` to `nãve_mode` is externally marked critical; B preserves the literal identifier. Those two strings are **not canonically equivalent**. This failure was understated in the earlier preparer-only discussion.

Against B, M local-error gains/losses are NL2/0, EN0/0; L NL6/0, EN1/1; F NL2/1, EN0/1. L's English “gain” singularizes `pumps` to `pump` while leaving `Does`; the reviewer credits local concord but cannot verify number/meaning preservation. F's apparent old reference gain for ASCII `didn't know` disappears because B's curly-apostrophe correction was already valid. F loses the real `visitor were → was` agreement repair. NL safe totals are B28+1?, M29+1?, L33+1?, F29+1?; EN B32+1?, M32+1?, L31+2?, F31+1?. Local grammar repair is not permission to damage meaning elsewhere.

L has the best Dutch sound/strict/repair counts, F the best Dutch clean preservation, and B the best English sound/strict/clean counts. **There is no single Dutch winner across repair, preservation and safety.** F's Dutch preservation and lower known critical count are meaningful research tradeoffs, not evidence that its additional malformed `onontvangen` is acceptable. M gains only one known Dutch sound passage, loses three English ones and increases Dutch unnecessary modifications from six to nine. L gains five and loses two Dutch sound passages, while losing four English passages with two unknown pairs. F gains three and loses one Dutch sound passage; English gains one and loses three. These effects do not justify a universal longer prompt.

## Locked confirmation results

Only A and prelocked B were run. No candidate was reselected, no prompt adapted to these failures, and no new confirmation was generated during this integration.

| Condition/language | Local = safe repairs /12 | Missed errors | Sound /20 | Strict full /20 | Eligible clean /8 | Critical delivered | Abstentions /20 |
|---|---:|---:|---:|---:|---:|---:|---:|
| A Dutch | 7 | 5 | 11 | 9 +2? | 4 | 1 +1? | 4 |
| B Dutch | 9 | 3 | 14 | 11 +3? | 5 | 1 | 2 |
| A English | 9 | 3 | 12 +1? | 9 +4? | 3 +1? | 0 | 7 |
| B English | 11 | 1 | 15 +1? | 12 +4? | 4 +1? | 0 | 4 |

B has **2/0 paired error gains/losses in each language**, NL sound gains/losses **4/1**, EN **3/0** plus one unknown pair. Its Dutch clean gains/losses are 2/1; the loss is suppressed `'s Ochtends → ' s Ochtends`. English gains one clean passage, with `chemist's` typography uncertain for both. Both arms still deliver corrupted `hüttenkäse`. The remaining B source errors are three Dutch cases (subordinate-clause order, `naief`, singular `inhoud ... zijn`) and one withheld English `tommorrow` repair. A's valid `haven’t seen` alternative explains why linguistic English repairs are nine rather than the frozen reference count eight.

Across selection plus confirmation, A/B have **74/83 local repaired-error occurrences** out of 110 arm-specific annotated errors: B adds four Dutch and five English local repairs, with zero paired local repair losses. Sound-passage counts are **106/124** of 180 arm-specific attempts, with **2/3 unknowns** respectively. These totals reuse the same source cases across arms; they are not 360 independent passages or population accuracy. Other prompt arms have no confirmation observations and must not be ranked using unmatched pooled panels.

## User-visible safety failures

[Ranked defect record](../benchmarks/phase4/external-gpt6/v1/analysis/defects.md) and [machine records](../benchmarks/phase4/external-gpt6/v1/analysis/defects.jsonl) retain exact source, raw/extracted/delivered text, condition, disposition, subject IDs, rationale and guard reason. Across all arms there are **26 critical delivered occurrences across nine source cases**, and 34 delivered meaning-failure occurrences. These repeat cases across conditions; neither count is a failure probability. The critical threshold is AI-declared, with unknown severity retained.

| Source / change | Responsible conditions | Reached delivery? / external diagnosis |
|---|---|---|
| Correct `hüttenkäse` → `huitkase` / `hijttenkäse` | Confirmation A / B | Both accepted; malformed food name and lost referent, critical. Literal Unicode does not solve Dutch corruption. |
| `hesje` → `hemdje`, `hemd`, `heksje` | Selection A/B/C/F; M; L | All accepted; clothing/referent substitution, critical; `coordinator` remains misspelled. |
| `goesting in frieten` → `geest` | A/B | Accepted; regional correct input damaged, critical. |
| Appetite → `gevoel van misselijkheid` | M | Accepted; nausea replaces appetite, critical. L's hunger rewrite is also marked meaning/new-error damage; F's `zin op frieten` is unnecessary with linguistic/semantic uncertainty retained. |
| `waarom wil ik ... doen` → `waarom ik ... doe` | A/B/M/L/F | Accepted; local order repaired but volition deleted, critical. C's `moet` substitution is withheld. |
| `leave ... where it is` → `... as it is` | M/L | Accepted; file-location constraint becomes state preservation, critical. |
| `bienvenue` → `welkom`, leaving `welkom ... welkom` | A/B/C/M/L | Accepted translation/duplication, critical; C/M/L also repair the Dutch typo, which does not make the passage safe. |
| `καλημέρα` → `kaalméra` | C | Accepted foreign-word corruption, critical. |
| Literal `naïve_mode` → `nãve_mode` | A | Accepted identifier corruption, critical. B and all added-instruction arms preserve it. |
| URL location `versie 2.4 op URL` → `... van URL` | M | Accepted provenance/location reinterpretation, critical in submitted assessment. |

Other delivered introduced-error cases include `onvangen → onontvangen` under M/L/F, A/C's `conciërge → concierge`, C's inserted `behoort behoorlijk`, and M/L deleting `precies` while adding `**zo**`. Original no-new-error flags and rationales are preserved; severity is not inflated to manufacture a uniform critical label. B reduces A's known selection new-error count from NL5/EN1 to NL4/EN0 (B EN has one unknown), but retains four Dutch critical cases. M/L add new English critical location damage absent from B. C creates a distinct Greek corruption. F has fewer known critical labels but adds malformed acknowledgment wording and loses English agreement.

[Unicode diagnostic view](../benchmarks/phase4/external-gpt6/v1/analysis/unicode-diagnostics.jsonl) separates canonical-equivalence checks from lexical judgments. NFD `café → café` is canonically equivalent, unlike identifier mutation or missing trema. B preserves the already correct `conciërge`, `reünie`, `coördinator` sentence, but none repairs `coordinator` in the vest case. A corrupts `Curaçao → Curauçao` in confirmation and is **rejected**; B preserves it unchanged. That is better generation/eligible preservation, not an accepted A delivery corruption. Emoji changes/removals are often blocked, including the confirmation balloon; fallback is abstention. Correct multilingual quotations can be damaged even by an orthographically plausible change: external review marks A's `γεια → γειά` as literal-quotation/meaning damage; the preparer had left orthographic validity uncertain. No new Greek ruling is made here.

## Engineering losses without changing the validator

There are **25 otherwise-valid necessary raw repairs withheld** and **30 otherwise-valid necessary parsed repairs withheld**, all English. Parsing can remove response-only emphasis before validation; these are separate counts of the same generation occurrences. Parsed counts are: selection A6, B4, C6, M4, L4, F4; confirmation A1/B1. These represent **eight distinct source cases** (seven selection, one confirmation), repeated across arms. Two further selection arm occurrences remain uncertain. Candidate-local repairs inside meaning/name/identifier/emoji damage are counted separately and do not enter this “otherwise-valid” total. The validator was not weakened or modified.

The recurrent lost repairs are `owner ... need → needs` (all six arms; modality guard), `mesasge → message` with `I’ll` (all six; capitalized-name guard), and `backup don't → does not` (all six; negation/modality guard). Additional A/C losses involve valid curly contractions; `tomorow → tomorrow` is blocked by temporal/relational/agent anchors in A/B/C/M/F. Confirmation `tommorrow → tomorrow` is blocked in both A/B, with A's curly `We’ll` hitting the name guard and B's ASCII version hitting the temporal-anchor guard. All 30 have a literal external full-candidate assessment and their original rejection reason in the occurrence records. This is evidence of false rejection for those texts, not grounds to remove the guards globally.

The Dutch `koffie-apparaat` alternative is **not counted as a demonstrated lost valid repair**: the preparer credits facultative hyphenation, but external review keeps spelling/repair uncertain. Identifier/name/emoji/quoted-command changes may repair an annotation while still warranting rejection. Extraction-stage raw/parsed losses are reported separately in metrics; no demonstrated otherwise-valid raw correction is lost by extraction on this panel. Optional salutation spacing and curly apostrophe styling on already-clean text are preservation/abstention questions, not withheld necessary repairs.

## Reviewer disagreements and ranking sensitivity

[Full reconciliation](../benchmarks/phase4/external-gpt6/v1/analysis/disagreements.md) retains the original, candidate, condition/layer, exact assessments, differing fields, both linguistic rationales and applicability explanation for every structured disagreement. It matches **154 bound confirmation records plus 33 literal parsed selection observations**: 187 overlapping records, not 187 independent texts. There are 113 records with differing boolean fields, **22 with at least one contradictory known boolean**, and one record with a per-error difference (the uncertain Dutch hyphen). Null/vacuous clean-repair conventions and source-fallback eligibility explain many differences; they are not automatically new linguistic errors. No contradictory known semantic/new-error/repair judgments were found between identical external candidate texts across layers.

Concrete disputes include L's appetite-to-hunger rewrite (preparer meaning/no-new-error true; external false), deleted volition/location constraints (both reject meaning but classify “new error” differently), `trace_id` markup (preparer meaning true versus external false), phrase emphasis (preparer new error versus external unknown), and the literal Greek accent change. Source-fallback clean-preservation disagreements never turn abstention into success. Neither AI is authoritative; qualified further review is warranted on genuine contradictions, orthographic alternatives and severity.

[Counterfactual sensitivity](../benchmarks/phase4/external-gpt6/v1/analysis/disagreement-sensitivity.json) substitutes only retained, known contradictory preparer fields on exact bound confirmation subjects and exact contextual selection pairs. It does not invent preparer coverage, fill nulls or create replacement review decisions. **No sound-passage or strict-full counts change under those known contradictions**, so those descriptive rankings are stable to this specific test. Several damage dimensions are already false under both readings. This is not stability to every possible independent rejudgment or to unreviewed judgments.

Unknown judgments still limit rankings. B's known sound-passage advantage over A survives worst-case assignment of the unresolved pairs: selection net NL4–6 and EN7–9 passages; confirmation NL3 and EN2–4. Strict full success has more unknown necessity judgments: selection English net bounds −3 to +15, and confirmation both languages include a possible reversal. Those bounds are mechanical extremes, not confidence intervals. The paired case bootstrap is descriptive of known observed pairs only: confirmation NL sound difference interval −0.05 to 0.35, EN 0 to about 0.316 on 19 known pairs. Small synthetic author-exposed samples, chosen candidates, multiple comparisons and AI-label uncertainty prevent statistical or population-superiority claims.

The [reference discrepancy records](../benchmarks/phase4/external-gpt6/v1/analysis/local-repairs-vs-reference.jsonl) contain 19 local delivered-error credits absent from references, but only **six sound whole-passage alternative occurrences**, spanning **two original English cases** (valid curly contractions). The rest include repaired order with deleted volition, uncertain aspect and singularized pumps. Calling all nineteen “valid alternatives” would be wrong. Original references remain frozen.

## Recommendation and the ten prompt questions

| Research order | Configuration | Evidence-based recommendation |
|---|---|---|
| 1, balanced two-language follow-up | B literal JSON, original | Best English and strongest balanced, prelocked confirmation evidence. Better measured linguistic delivery than A, with no paired local repair losses, but accepted critical Dutch damage forbids promotion. |
| 2, distinct Dutch tradeoffs | F few-shot / L checklist | F best Dutch clean preservation; L best Dutch repair/sound counts. Neither clearly dominates; both lose English quality and add damage. No general replacement justified. |
| Control retained | A production v3 | Lower delivered quality on this evidence, but changing the production default is not justified by unsafe experimental alternatives. Retention is not endorsement of A's known failures. |
| Lower research priority | C plain text | Worse than B in both languages, longer framing and unique multilingual corruption; no advantage established. |
| Reject as general improvement | M minimal edit | Small Dutch gain, worse English/necessity and accepted nausea/location damage. Explicit minimality instructions are not a safety mechanism. |

1. **Literal JSON versus v3:** yes, the external AI linguistic diagnostics favor B on both languages' known sound passages and preservation; this is not independently human-validated superiority.
2. **Additional repairs:** selection +2 NL/+3 EN local repairs; confirmation +2/+2. Across panels +4/+5. Safe totals and unknowns are separately disclosed above.
3. **Lost repairs:** zero paired local delivered-error losses for B versus A. English multilingual formatting is an unresolved safety loss, not a fabricated failed repair; Dutch confirmation loses one clean eligible passage.
4. **Correct-text preservation:** B selection18/19 versus A15/14; confirmation5/4 versus4/3, with one shared unknown English clean judgment. F is slightly better on Dutch selection clean text.
5. **Semantic corruption:** B selection Dutch four meaning failures plus one unknown, same as A; English zero versus A one. Confirmation Dutch one versus A two; English zero for both. B does not solve semantics.
6. **New critical failures:** no B-only known critical delivered source versus A; both have the corrupted food name. This finite observation cannot certify absence of new risk. Added-instruction arms do introduce B-absent critical failures.
7. **Minimal edit:** no consistent quality/necessity benefit; do not adopt it generally.
8. **Language checklist:** real Dutch repair/sound gains with English losses and new critical constraints damage; a Dutch diagnostic contender, not a safe default.
9. **Few-shot cost:** modest Dutch gains, English loss and malformed acknowledgment do not justify blanket use. Measured selection prompt-content tokens rise roughly82% NL/65% EN and median latency24%/18% versus B; RSS remains measured per occurrence, peak2.78GiB. No new timing experiment or speed claim is made.
10. **Replace v3:** **no**. B is the preferred balanced research substrate, not a production promotion proposal. All candidates fail essential meaning-safety evidence; human approvals remain zero.

The single highest-value next targeted engineering experiment is a **replay-only, narrowly bounded validator investigation of ordinary English inflection/contraction and temporal-typo false rejections**, retaining every existing meaning/protected-content negative regression and explicitly measuring dangerous candidates newly admitted. Thirty reviewed valid repairs provide concrete positives; current corrupt candidates provide necessary negatives. This is a proposal for a separately authorized change, **not implemented here**, and must not become a blanket relaxation. No new inference is needed to test the first iteration. Semantic safety remains the promotion blocker even if those losses are recovered.

## Reproduction, tests and stopping point

Use only existing saved generations. A Release build is needed for C# adjudication; unset `DIORTHOSIS_TEST_MODEL` to avoid the real-weight test. The importer and analyzer use Python standard library only; no new dependency, model, service, cloud inference or weight download is introduced.

```sh
python3 scripts/integrate-phase4-reviews.py benchmarks/phase4/external-gpt6/v1 /tmp/NEW-phase4-import
python3 scripts/analyze-phase4-external-reviews.py /tmp/NEW-phase4-import /tmp/NEW-phase4-import/analysis
python3 scripts/verify-phase4-external-reviews.py --reproduce /tmp/ANOTHER-NEW-phase4-reproduction
python3 -m unittest discover -s tests/tooling -p 'test_*.py'
env -u DIORTHOSIS_TEST_MODEL dotnet test Diorthosis.sln -c Release
```

The retained artifact manifest binds original archives, imports, derived reports and implementation hashes. CI verifies and fully reproduces all eighteen imports, parent C# adjudication and analysis, in addition to historical checks, 920 deterministic saved-response replays and the existing six cross-platform Native AOT/publish/smoke jobs. Neither deterministic replay nor adjudication is model inference. Local checks passed **232 .NET tests with one real-model test deliberately skipped**, **43 Python tests**, all **920 saved-response replays**, and complete import/C# analysis reproduction. All 84 historical, 165 Phase 2, 68 Phase 3 and 47 external-Phase-3 files remain unchanged. The frozen Phase 4 tracked inventory and production v3 hashes match. See the [validation record](../benchmarks/phase4/external-gpt6/v1/validation.json); current-head CI is pinned by exact commit/run in draft PR #7. Existing untracked local copies of original review packets remain untouched; they are not part of the frozen tracked inventory or new submitted evidence.

Stop at this evidence-based decision checkpoint. No new generation, linguistic assessment, prompt change, reference edit, validator change, promotion, merge or next-phase development is part of this work.
