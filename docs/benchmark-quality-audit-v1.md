# Deep benchmark quality audit — proposal v1

**2026-10-20 · AI-authored investigation, not independent bilingual validation.** Repository `MarcoHuib/Diorthosis`, branch `feat/local-ai-benchmarks`, commit `bd28ef5e9d584e849edb39d8566194a231d68f58`, open PR #2. All recommendations below await human review. Nothing is adopted by this report.

The benchmark currently **undercredits several valid corrections and also exposes genuine model errors and a protected-identifier failure**. Correcting four reference omissions substantially changes the diagnostic precision figures. It does not establish production quality. Ministral 14B remains the strongest quality candidate in this small comparison, with unresolved temporal ambiguity, a missed Dutch repair, and an unnecessary edit to correct Dutch text.

The complete evidence is in the [44-case reference and 132-output review](../benchmarks/model-validation/audits/quality-v1/case-review.md), [separate annotation proposal and exact changelog](../benchmarks/model-validation/audits/quality-v1/README.md), [machine-readable sensitivity analysis](../benchmarks/model-validation/audits/quality-v1/sensitivity.json), and [deterministic probe observations](../benchmarks/model-validation/audits/quality-v1/probe-observations.json). The original dataset and all historical results remain unchanged.

## 1. Scope, evidence and review procedure

**Verified facts** in this report mean directly inspected repository behavior or reproducible computations over saved files. **Linguistic judgments** are this AI reviewer's assessments under a declared standard-prose policy. **Hypotheses** identify plausible explanations or generalization risks not established by these data. **Recommendations** require approval and independent review before adoption.

The investigation read `AGENTS.md`, architecture/roadmap and benchmark documentation, model-validation records, the prior pipeline investigation, ADRs 0006/0007, the schema, evaluator, parser/validator, existing tests, all 44 v2 references, and all 132 top-level v4 traces/predictions. The GitHub issue lookup returned no open issues; PR #2 is open and unmerged, with the same head as the local checkout. Its description still contains the superseded v3 comparison. No GitHub content was edited. A future approved PR-description update should clearly separate v4 historical scores from this proposed sensitivity scenario.

The plan was bounded to: inspect scoring; record all reference judgments before v4 trace inspection; assess anonymized outputs; reproduce historical scores; replay only saved text; construct a separate proposal; quantify annotation sensitivity and characterize scoring failures. The alternatives were (a) leave references untouched and report disputes only, or (b) change the scoring implementation. This audit uses a conservative annotation-only scenario for measurable effects and documents implementation proposals separately. The principal risks are reference incompleteness, model-output anchoring, context ambiguity, and overinterpreting a tiny development set. Acceptance required coverage of 44/44 references and 132/132 outputs, exact historical-score reproduction, trace/prediction consistency, and no production/historical modifications; these checks passed.

Reference judgments were recorded in `reference-notes.json` and hashed before opening the v4 trace/prediction files. The user prompt and historical documentation already disclosed some outputs, so this was **not fully blind**. A seeded shuffle assigned 132 response IDs; model identities, scores and latency were hidden during the linguistic assessment. Identical answers received the same judgment. The identity key was then restored for reporting. All work was by one AI reviewer; there was no second reviewer, inter-annotator agreement measurement or independent validation. The published packet/key permits inspection of this process but cannot retrospectively eliminate exposure bias.

The policy is standard edited Dutch/English prose, preserving valid NL/BE and UK/US conventions already present. No house style is imposed. Fragments are judged as continuous prose unless context establishes dialogue or deliberate fragmentation. Optional elegant wording is not a necessary correction. All 44 references need independent human sign-off; the case appendix flags additional context and regional questions individually.

## 2. What the current evaluator actually measures

### Representation and matching — verified

Each error is a UTF-16 source span with exact `originalText`, a category, and an array of alternative **replacement strings for that span**. There is no semantic representation, dependency relation between errors, context field, protected-span annotation, or independent severity field. Clean cases have an empty `errors` array. There are 22 cases per language: 15 erroneous cases with 16 annotated errors, plus seven clean cases. The two-error cases are Dutch `nl-06` and English `en-03`.

[`BenchmarkEvaluator`](../src/Diorthosis.Benchmarks/BenchmarkEvaluator.cs) checks case/prediction IDs, source alignment, edit bounds and prediction overlap. For each annotation, it gathers all as-yet-unmatched edits touching that span. Insertions at either endpoint count as touching. It applies only that set of edits to the input, then compares the complete resulting string ordinally against input-with-one-accepted-replacement. All gathered hunks receive credit if the strings match. Different edit boundaries can therefore receive credit, **provided they jointly reproduce the exact accepted repair for that one annotation**.

It does not recognize arbitrary semantic equivalents. “Complete text comparison” is more limited than independently judging the final full output: matching occurs per annotation, using only its related edits. A global unmatched-edit check follows. If any unmatched edit is lexical, all provisional recall and correct-suggestion credit in the case is withheld. Unmatched case-only edits, and edits whose old and new strings contain no letters or numbers, are called cosmetic; these do not cancel repair recall, although they still count as false-positive hunks.

Precision is **credited `TextEdit` hunks / all suggested hunks**. False positives are all uncredited hunks, including incorrect attempted repairs of genuine errors. They are not exclusively edits to error-free text. Recall is resolved annotations / annotations. A clean case is marked changed if it has any edit. Clean false-positive rate and preservation are consequently case-level measures. Missing predictions act as no edits, which reduces recall but can make preservation look good. A zero denominator returns zero, including precision when no edits were suggested; future reporting should distinguish undefined from measured zero.

The schema permits replacement alternatives but offers no case-level semantic adjudication state. Also, its enum spellings are lowercase/camelCase, whereas the historical file uses capitalized values accepted by the C# JSON reader. This is schema/serialization drift, not the explanation for any v4 score. The separate proposal preserves historical casing and records this compatibility issue explicitly.

### Actual methodological and implementation weaknesses

| Finding | Evidence / consequence | Type and scope |
| --- | --- | --- |
| Missing legitimate alternatives | `nl-12`, `en-04`, `en-05`, `en-09`; current code can score them correctly once explicitly listed | Annotation omissions, measurable now without a code change |
| Hunk-weighted precision | A valid `nl-12` restructuring yields four hunks; a local spelling repair often yields one. Accepting the alternative credits all four, so grouping affects both penalty and reward | Metric design, not evidence that the matcher always rejects restructuring |
| Cross-annotation edits fail | The same fully correct `She has received it.` gets 2/2 recall as separate edits but 0/2 as one sentence-spanning edit | Evaluator matching limitation; deterministic probe; not an observed cause of this v4 aggregate |
| Category filtering loses context | For `She have recieved it.`, both fixes score 2/2 in `all`, but the filtered Grammar and Spelling rows each score zero because the other correct lexical fix becomes unmatched | Implementation defect in category reporting; current 44 cases do not mix error categories within a case, so their language totals are unaffected |
| Lexical veto conflates repair and preservation | `Please recieve...` → `Kindly receive...` gets zero repair recall, although the spelling error is fixed and the extra edit is stylistic | Deliberate conservative policy, insufficiently separated metrics; not a general semantic detector |
| “Cosmetic” does not imply harmless | Changing a statement's final full stop to a question mark still permits recall credit for a matched spelling repair | Semantic measurement gap; probe demonstrates speech-act change, not a new inference result |
| No canonical Unicode equivalence | Precomposed `ï` reference versus decomposed `i` + diaeresis yields a mismatch despite equivalent visible spelling | Exact-matching limitation; not observed in these saved outputs |
| Error units are inconsistent | `en-03` has two dependent grammar annotations, while `en-01` bundles several morphology/tense problems; `en-05/09` bundle a separator and apostrophe | Annotation ontology/denominator problem; do not silently split units during sensitivity analysis |
| Clean preservation is conditional on annotation quality and filtering | Any accepted edit on a labeled clean case counts against preservation; rejected bad proposals can still leave accepted preservation high | Appropriate distinction for product versus model, provided both layers and rejection coverage are shown |
| Validation is not a meaning guarantee | Name/number/URL heuristics catch some changes but miss others; details below | Product safety coverage, distinct from reference matching |

The hunk problem is an established evaluation concern, not specific to these models. The M² research addresses edit-representation sensitivity in grammatical-error correction. It is useful methodological background, not a recommendation to adopt its scorer or dependencies without a separate assessment. [Dahlmeier and Ng, 2012](https://aclanthology.org/N12-1067.pdf). ERRANT likewise distinguishes linguistic edits/types from arbitrary string differences; its English framework should not be assumed to transfer unvalidated to Dutch. [Bryant, Felice and Briscoe, 2017](https://aclanthology.org/P17-1074/).

## 3. Reference judgments and the proposed revision

**Linguistic judgment:** No existing target in the 44 cases is clearly invalid under the declared standard-prose assumptions. Four cases omit defensible minimally invasive alternatives. Other cases need contextual qualifications, rather than automatic relabeling. The [case appendix](../benchmarks/model-validation/audits/quality-v1/case-review.md) gives the input, original annotation and category, validity judgment, alternatives, regional conventions, ambiguity, meaning risks and proposal for every case, including every clean case.

The proposed file appends five alternatives across four cases. It changes no input, source span, category, existing alternative, error count, clean label or model prediction. Its hash is `5b1810b0dfff1156facba81c4a9d0e72487e9652c11c637badd9bca7488c9e55`; the original reviewed-v2 hash remains `2190c109f5f8e8ce1979637a1beea9fcce615959033aa76576b32ba537465fa2`. It is a conservative, explicit sensitivity scenario, not an exhaustive set of acceptable sentences.

**`nl-12`: legitimate structural repair.** The file's input has no leading space: `Omdat de server opnieuw opstartte. De taak werd onderbroken.` In continuous prose, the causal subordinate clause is stranded. `Omdat de server opnieuw opstartte, werd de taak onderbroken.` joins it to its main clause, with the required inversion, and preserves causal direction, tense and original clause order. The existing `De taak werd onderbroken omdat de server opnieuw opstartte.` is also valid but moves the subordinate clause. The fronted-clause pattern follows normal Dutch clause ordering. [Onze Taal](https://onzetaal.nl/taalloket/woordvolgorde-in-bijzinnen-en-hoofdzinnen).

Both 8B and 14B produce this legitimate alternative. Their four false-positive hunks are **not four independent linguistic mistakes**. Under the proposed reference, the unchanged evaluator resolves one annotation and credits four hunks. That fixes the missing-reference penalty while retaining the metric's disproportionate hunk weight. 3B leaves the fragment unchanged. A dialogue context could license a deliberate fragment; independent reviewers should confirm the benchmark's continuous-prose assumption.

**`en-04`: legitimate alternative with optional comma.** `Tomorrow I will send the package.` and `Tomorrow, I will send the package.` preserve agent, modality, object and time. They minimally correct verb-object order and retain the initial temporal focus. Both are at least as defensible as moving `tomorrow` to the end, as the existing reference does. 3B supplies the version without a comma; 8B and 14B supply the comma version. Add both uniformly. Original scoring counted respectively two, three and three false-positive hunks for these valid repairs.

**`en-05` and `en-09`: apostrophe representation.** All three models supply `Wait, let’s ...` with U+2019, while the reference has U+0027 in `Wait, let's`. Both represent the same contraction; the comma and apostrophe repair the intended defects. Unicode documents the use of U+2019 as the punctuation apostrophe and the keyboard prevalence of U+0027. [Unicode 16, chapter 6](https://www.unicode.org/versions/Unicode16.0.0/core-spec/chapter-6/). The strict comparison currently rejects the whole two-hunk repair, including its correct comma. Adding the typographic form explicitly is safer for this limited proposal than globally normalizing apostrophes in identifiers, URLs, names or quotations. These two nearly identical cases overweight one punctuation construction.

**`en-01`: do not erase temporal ambiguity.** `She sent the documents yesterday.` is the least assumptive repair of an isolated completed past event. `She had sent the documents yesterday.` is grammatically possible with an understood past reference point; it adds a temporal relation that the isolated input does not establish. The 8B/14B answer therefore receives **grammatically acceptable but semantically ambiguous**, not “ungrammatical” and not automatic equivalence. It remains uncredited in the strict sensitivity scenario. The 3B `She has sent ... yesterday` still leaves a standard-prose tense/time problem. Cambridge's treatment supports past simple with definite past-time expressions; this does not decide the writer's unobserved intention. [Cambridge Grammar](https://dictionary.cambridge.org/uk/grammar/british-grammar/past-simple-or-present-perfect).

Other priority judgments: `nl-11` is a standard-edited-register correction, not a claim that regional/spoken `beter als` never occurs; [Taaladvies.net](https://taaladvies.net/groter-als-of-dan/) distinguishes these usages. `en-07/13` also require a standard-prose rather than dialect-transcription target. `en-03` assumes ordinary mass-noun *information*, not specialized legal *informations*. `nl-05`, `en-05/09` and the fragment cases have prosodic/context choices. `nl-15`'s current `en me laten` is valid; all models miss it. The name, date-format, scheduled-present-tense and technical-wording clean cases remain clean. US `Ms.` and month-day dates must not be “corrected” into another house style.

## 4. Model-output adjudication and safety

All 132 raw generations, extracted candidates and accepted results are retained in the [adjudication JSONL](../benchmarks/model-validation/audits/quality-v1/output-adjudications.jsonl), with a reason for each classification. The eight requested labels are used; successful clean no-ops are explicitly a `correct preservation` subtype under “Correct minimal correction,” since the eight-label list has no dedicated successful-preservation category. They do not count as repaired errors or suggested edits. Optional rewrites on already-correct text use the more specific “Unnecessary change to already-correct text” label. No observed erroneous-source output needed the separate “Correct but unnecessarily rewritten” label.

Key disagreements that are **not missing-reference bugs**:

| Model / case | Judgment | Practical implication |
| --- | --- | --- |
| 8B `nl-02` | `vergader ruimte` → `vergadering`: grammatically plausible, altered meaning (room → meeting) | Do not add the output to accepted alternatives; accepted pipeline edit needs semantic review |
| 8B `nl-03` | `dat de afspraak morgen vindt plaats`: incorrect subordinate-clause order | Genuine grammar failure |
| 3B `nl-17` | Correct `start` → incorrect `startt` | New error introduced on clean text |
| 8B/14B `nl-19` | Adds `een` before `HTTP 404` | Output can be grammatical but input needed no repair; preservation failure remains |
| 3B/8B `nl-22` | Adds `het` before `team Atlas` | Unnecessary article insertion; no accepted-alternative expansion for clean text |
| All `nl-15` | Leaves mixed modal/imperative coordination unchanged | Genuine missed repair under the stated reference interpretation |
| 3B `en-13` | Leaves `There is several reasons` unchanged | Genuine missed repair |
| 8B `en-13` | Repeats wrong sentence, then puts correct `There are...` in a note | Raw response requires review; parser selects the wrong first sentence. Accepted result is incomplete. A correct string buried in commentary is not a delivered correction |

The last case limits claims about EOG: all saved outputs record natural EOG, but EOG termination does not guarantee a single answer, no explanation, or semantic correctness. The correct alternative in 8B's note already exists in the reference. Adding annotations cannot repair that response-format/extraction issue. It must not receive a retrospective accepted-output score based on cherry-picking the note.

### Protected content — verified behavior, qualified linguistic severity

The actual v4 accepted violation is **8B `en-17`: `node-04` → `node 04`**. A hyphen can be part of a host/resource identifier, so literal preservation matters even if a reader recognizes the intended node. It passes the current validator. The identifier regex protects some alphanumeric tokens but does not capture this whole hyphenated identifier; its numeric fragment `04` remains unchanged. Treat this as potentially critical pending application-context severity review, not harmless typography.

All three raw Dutch URL responses wrap the URL in Markdown, and 3B/8B do so for the English URL. These duplicate the URL and alter the plain-text surface; they do not demonstrate a changed destination. The validator rejects them and preserves the source. 8B also proposes `OAuth2` → `OAuth 2.0` in Dutch and English; both are rejected. 14B's Dutch raw `OAuth2` → `OAuth 2` follows another path: whitespace-only normalization emits **no edits**, preserving the accepted source without a rejection. Its raw edit list is also empty. Consequently even “raw candidate” metrics do not fully measure literal raw-output preservation.

The current safeguards compare regex-extracted numbers, URLs, likely identifiers, symbols and selected capitalized names, with a sentence-initial function-word exception list. They are not complete name, date or terminology recognition. The isolated analysis probes reproduce accepted changes from `3 mei 2025` to `3 juni 2025` and from `She must not send it.` to `She must send it.` No v4 model is alleged to have generated these probe outputs: they are synthetic deterministic tests of the unchanged validation function. Positive controls reject an ordinary capitalized-name substitution, a digit-time change and a URL destination change.

Thus the existing clean cases exercise some protected content but cannot establish protection of lowercased names, written dates, negation, legal modality, attribution, participant roles, quotations, or technical identifiers generally. Accepted suggestions remain review proposals. No independently adjudicated “zero critical semantic alterations” claim is available for any model.

## 5. Provisional annotation-only sensitivity analysis

**Measured from saved v4 edit lists using the unchanged evaluator; no new inference.** All 132 traces agree with their prediction files and inputs. Replaying their raw text through the unchanged parser reproduces extraction, raw edits, accepted edits and rejection reasons exactly. Historical metrics reproduce exactly for accepted and parser-derived raw candidates. All traces record EOG with fewer than 128 sampled tokens. This verifies recorded evidence; it is not a rerun of the runtime termination test.

The report metadata identifies Q4_K_M, prompt `diorthosis-correction-v3`, LLamaSharp 0.27.0 with pinned llama.cpp, context 4096, nine CPU threads, greedy decoding and maximum 128 tokens. The documented host is Apple M4, 32 GB, macOS 26.6.2 ARM64; saved reports identify .NET 10.0.12. Exact model revisions/hashes are preserved in `sensitivity.json`, `source-manifest.json` and the original reports. Audit calculations use installed .NET SDK 10.0.401; they do not reproduce model inference or performance measurements.

### Accepted edits: original → proposed

| Model | Language | Error recall | Hunk precision | False-positive hunks |
| --- | --- | --- | --- | --- |
| 3B | Dutch | 11/16 (68.8%) → 11/16 (68.8%) | 11/13 (84.6%) → 11/13 (84.6%) | 2 → 2 |
| 3B | English | 11/16 (68.8%) → 14/16 (87.5%) | 12/20 (60.0%) → 18/20 (90.0%) | 8 → 2 |
| 8B | Dutch | 12/16 (75.0%) → 13/16 (81.3%) | 13/21 (61.9%) → 17/21 (81.0%) | 8 → 4 |
| 8B | English | 11/16 (68.8%) → 14/16 (87.5%) | 12/22 (54.5%) → 19/22 (86.4%) | 10 → 3 |
| 14B | Dutch | 14/16 (87.5%) → 15/16 (93.8%) | 15/20 (75.0%) → 19/20 (95.0%) | 5 → 1 |
| 14B | English | 12/16 (75.0%) → 15/16 (93.8%) | 13/22 (59.1%) → 20/22 (90.9%) | 9 → 2 |

| Model | Pooled recall, original → proposed | Pooled hunk precision, original → proposed | Clean preservation NL / EN (unchanged) | Clean false-positive rate NL / EN (unchanged) |
| --- | --- | --- | --- | --- |
| 3B | 22/32 (68.8%) → 25/32 (78.1%) | 23/33 (69.7%) → 29/33 (87.9%) | 5/7 (71.4%) / 7/7 (100%) | 2/7 (28.6%) / 0/7 (0%) |
| 8B | 23/32 (71.9%) → 27/32 (84.4%) | 25/43 (58.1%) → 36/43 (83.7%) | 5/7 (71.4%) / 6/7 (85.7%) | 2/7 (28.6%) / 1/7 (14.3%) |
| 14B | 26/32 (81.3%) → 30/32 (93.8%) | 28/42 (66.7%) → 39/42 (92.9%) | 6/7 (85.7%) / 7/7 (100%) | 1/7 (14.3%) / 0/7 (0%) |

Pooled figures are sums of numerators/denominators, not averages of percentages. Do not substitute pooled recall for the separate Dutch/English gates.

Exactly **11 model/case score classifications** change; all are missed repair → matched repair with the same output and edit counts:

| Case | Affected models | Added resolved errors per affected model | False-positive hunks removed (3B / 8B / 14B) | Reason |
| --- | --- | ---: | --- | --- |
| `nl-12` | 8B, 14B | 1 | — / 4 / 4 | Valid fronted causal repair |
| `en-04` | All | 1 | 2 / 3 / 3 | Valid retained initial time adjunct; optional comma |
| `en-05` | All | 1 | 2 / 2 / 2 | Equivalent apostrophe in corrected contraction |
| `en-09` | All | 1 | 2 / 2 / 2 | Same apostrophe policy |

No other case scores change. In particular, `en-01` remains uncertain and uncredited; `nl-15`, unnecessary clean edits and the 8B identifier change remain failures. These changes correct annotation coverage, not the model outputs themselves. The JSON includes every original/proposed case delta and reason.

### Raw candidate and sentence-level views

For the proposed annotations, parser-derived raw-candidate recall equals accepted recall, but raw hunk precision is lower because rejected attempts remain visible: 3B NL 11/16 hunks (68.8%), EN 18/23 (78.3%); 8B NL 17/25 (68.0%), EN 19/26 (73.1%); 14B NL 19/23 (82.6%), EN 20/22 (90.9%). Raw-candidate clean preservation is respectively NL/EN 4/7 and 6/7, 3/7 and 4/7, 5/7 and 7/7. These lists exclude the swallowed whitespace-only identifier change and commentary selected away by extraction; literal-output adjudication is therefore indispensable. Saved rejection counts remain 2, 4 and 1 out of 44.

An additional **AI-adjudicated descriptive sentence view**, not a replacement benchmark metric, counts error-containing cases completely repaired with minimal meaning-preserving output: 3B NL 10/15, EN 13/15; 8B NL 12/15, EN 13/15; 14B NL 14/15, EN 14/15. Clean preservation remains the table above. The ambiguous past-perfect answers are not counted as strict successes. All-layer labels and zero-count categories are retained in `sensitivity.json`. These are judgments awaiting human review, not independent semantic measurements.

### Ranking and uncertainty

Recall ordering is unchanged: 14B > 8B > 3B in Dutch; 14B > 3B = 8B in English. Hunk-precision ordering changes from 3B > 14B > 8B to 14B > 3B > 8B in both languages and pooled. The revised English precision difference between 14B and 3B is only 20/22 versus 18/20; neither statistical superiority nor generalization follows from that difference. The heavier restructuring hunk weights also influence precision ranking.

One resolved annotation moves recall by 6.25 percentage points. A single clean case moves preservation by 14.29 points. Illustrative two-sided Wilson 95% intervals are 71.7–98.9% for 15/16 recall, 48.7–97.4% for 6/7 preservation, and 64.6–100% for 7/7. These intervals assume independent Bernoulli observations; near-duplicates, dependent annotations and convenience sampling violate that simple population model. They illustrate imprecision, not validate inference from this dataset. Diff hunks are correlated, so a binomial interval treating 20/22 hunks as independent linguistic decisions would be misleading.

The four reference changes were motivated before trace inspection and apply identically across models, but the disputed constructions were already disclosed by the user and historical results. This remains post-hoc diagnostic sensitivity. Do not replace historical v4 scores or claim the held-out 90% gate, aspirational 95% recall, 98% precision, 99% preservation, or semantic-safety goal has been met. The strict proposed 14B recall is still below 95% even as a point estimate.

## 6. Explicit proposals for metric/evaluator changes — not implemented

Retain versioned historical `error_recall_v1` and `hunk_precision_v1` for comparability. Before new headline metrics, agree on the unit of a proofreading proposal. Recommended future reporting separates:

1. **Error-level repair recall:** each preannotated defect/dependency group resolved, with a declared partial-repair policy. Measure local repair independently of unrelated stylistic edits; report a separate stricter safe-complete-case outcome. Do not silently remove the current lexical veto without adding the preservation measure.
2. **Necessary-correction precision:** correct, justified linguistic proposal groups / all proposal groups. A word-order repair can be one group spanning several hunks. Define deterministic grouping, adjudicate contested groups, and freeze the protocol before comparing models. Do not group every sentence into one proposal: that would hide a bad extra change beside a valid repair.
3. **Sentence/passage success:** all original defects repaired, no new error, meaning/protected content preserved; separately label “grammatical but unnecessary rewrite” and “ambiguous.” Include error-only and clean-case denominators separately so no-op systems cannot appear strong.
4. **False-positive reporting:** count incorrect repair attempts, unnecessary edits on erroneous passages, and edits on clean passages separately. Retain clean-case false-positive rate, exact source preservation, and preservation free of unjustified changes as distinct concepts where formatting policy matters.
5. **Semantic/protected preservation:** human labels for facts, agency, number, time/aspect, negation, modality, reference and legal effect; programmatic exact checks for supplied protected spans. Report critical violations and unresolved cases explicitly, never infer semantics from token overlap.
6. **Pipeline coverage:** raw-format compliance, extraction success, safe rejection, withheld proposals, accepted suggestions and parser/validator false rejections. Score literal raw text and delivered suggestions separately.

A replacement matching algorithm should handle dependent annotations and edits spanning multiple annotations, compute category attribution after globally matching against the full annotation context, and avoid greedy boundary conflicts. A narrowly scoped Unicode/typographic equivalence policy must preserve exact source offsets and protected bytes. Merely adding more whole-sentence references can mask these implementation issues and risks accepting extra wording inside broad spans. General “semantic similarity” scoring or an uncalibrated LLM judge is not an acceptable substitute.

Expected effects of these proposals are qualitative until implemented and reviewed: category matching should restore legitimate mixed-category credit; grouped proposal precision would remove arbitrary hunk weighting; independent local recall could rise for repairs accompanied by stylistic changes while safe-case success stays failed. **No hypothetical new-metric model scores are reported.** The measured tables above use only current code.

Deterministic regression specifications for an approved follow-up:

| Test | Required future behavior / decision |
| --- | --- |
| `nl-12`, original four-hunk repair versus one replacement | One resolved error with either representation; one necessary linguistic proposal if that grouping policy is adopted; preserve v1 hunk count separately |
| Both `en-04` variants | Same repair success and meaning decision, independent of optional introductory comma |
| Straight/curly `en-05/09` and protected URL containing punctuation | Equal contraction repair credit; no global mutation of protected URL or literal identifier |
| `She have recieved it.` with split/combined repairs | Same two resolved annotations in `all`; correct Grammar and Spelling attribution with full case context |
| Correct repair plus unrelated lexical stylistic rewrite | Local error repaired, extra proposal unjustified, strict safe/minimal case fails; translation still fails target-language and semantic criteria |
| Correct spelling plus statement→question punctuation change | Do not treat “cosmetic” as evidence of semantic preservation |
| Precomposed/decomposed diaeresis, with surrogate/combining offsets | Equivalent spelling under an approved normalization policy, source offsets remain valid and protected content untouched |
| `sent` versus `had sent` versus `has sent ... yesterday` | Preserve distinct temporal/adjudication outcomes; never blanket tense equivalence |
| `node-04`, `OAuth2`, written months, names, numbers, negation, legal modal verbs | Exact protected-span changes or clear semantic reversals cannot become automatically safe proposals; include paired legitimate grammatical edits nearby |
| 8B `en-13` two-answer response | Report the actual delivered result; do not cherry-pick a later answer for scoring. Any parser change needs a general ambiguous-response policy |
| Missing prediction / zero suggestions / clean-only sample | Report abstention/coverage and undefined denominators explicitly; no invented perfect precision |

The [analysis driver](../benchmarks/model-validation/audits/quality-v1/Probe/Program.cs) already reproduces current cross-category, cross-boundary, hunk-weighting, lexical-veto, punctuation, Unicode and safety behaviors with assertions. These are characterization probes outside the production solution, not fixes or adoption of the proposed metric definitions. The existing unit tests pass while these limitations remain; test success is not proof of methodological validity.

## 7. Held-out benchmark design and prioritized action plan

**Priority 0 — resolve this review before using scores for selection.** Have independent Dutch and English specialists assess the references first, blinded to outputs and model names. Use at least two independent judgments per language plus an adjudicator for disagreements; include bilingual review of meaning preservation and cross-language consistency. Resolve `en-01` temporal intent, standard-register boundaries, dependent-error units and punctuation policy. Confirm or reject the five additions in a new version; keep v2 and v4 intact. Review the accepted 8B meaning/identifier changes as concrete safety failures. Objective completion: signed/versioned decisions for all 44 references and disputed outputs, with reviewer qualifications/roles and disagreement logs, not an AI “validated” label.

**Priority 1 — approve general evaluator/safety follow-ups separately.** Fix category attribution and representation-sensitive matching with the deterministic specifications above. Agree on proposal grouping and metrics before changing denominators. Expand protected-span coverage and test semantic-risk cases, without model-specific or language-specific output patching. Address raw/extracted/accepted observability, including whitespace no-ops and multi-answer outputs. Record consequential adopted decisions in an ADR only after approval; this report proposes options and adopts none. Objective completion: deterministic fixtures pass agreed expected outcomes, historical scores remain reproducible, and compatibility changes are documented.

**Priority 2 — build independently reviewed development, selection and final held-out partitions.** Keep these 44 cases as disclosed regression/smoke material. They are useful for preliminary screening of format compliance, obvious grammar, overcorrection and safety regressions; they are not representative enough for reliable acceptance/rejection or fine ranking. The existing 600-case synthetic candidate also requires independent review and cannot become held-out merely by changing its filename. Group split by source, template and near-duplicate construction (especially `lets` cases), not random sentence rows. Exclude all prompt/parser-development material from the final test. Do not tune against selection/final outputs repeatedly.

Use rights-cleared nonconfidential texts representative of intended local writing: email, technical documentation, reports and ordinary prose, including short passages with sufficient context to resolve tense, reference and ambiguity. Stratify Dutch NL/BE and English UK/US usage, error type, difficulty, single/multiple/dependent errors, sentence length, context and technical terms. Do not “correct” legitimate dialect, quoted speech or regional standard variants unless the requested register warrants it. Add deliberately clean but unusual sentences, dates/URLs/names/identifiers embedded beside genuine errors, and adversarial contrasts for negation, modality, numbers, entity roles and causal direction. Assess text correction separately from document extraction/fidelity; this set contains no evidence about DOC/DOCX round trips.

Before outputs are inspected, annotate error/dependency IDs, spans, categories/subtypes, valid minimal alternatives, explanatory rationale, ambiguity, register/region, protected spans and permissible transformations. Include multiple independent references where necessary, but distinguish genuine correction alternatives from style preferences. On previously unlisted outputs, use blinded adjudication with a versioned appeals protocol applied equally to all systems; keep original and adjudicated results separately. Record coverage, agreement before adjudication, unresolved rates, and how exclusions affect denominators. Ambiguous cases belong in an explicit context/abstention track until resolved, not silently in or out of a model's score.

Pre-register sample sizes and uncertainty. With zero failures and ideal independent sampling, a one-sided 95% exact lower bound exceeds 95% only after 59 successes, 98% after 149, and 99% after 299 (`n >= ceil(log(0.05)/log(target))`). These are optimistic mathematical minima, not sufficient representative-design recommendations; errors, clustering, multiple languages and stratification require larger sets. The proposed 75 clean cases per language in the existing 600-case plan cannot substantiate 99% preservation at that confidence level even with zero observed failures. Design for per-language uncertainty, clustered/paired comparisons, and rare critical-risk coverage, not just a round total of 600. No finite sample proves zero critical semantic risk.

Freeze the model revision/checksum, quantization, prompt, parser, scorer, dataset, hardware, context and decoding settings. Independently evaluate recall, proposal precision, preservation, semantic severity, latency, peak memory, offline behavior and document fidelity. Human linguistic adjudication and operational tests need separate evidence. Use case- or document-level paired uncertainty for model comparisons; do not treat diff hunks as independent samples. Keep data access and any future inference fully offline.

**Priority 3 — compare 14B with a distinct reviewed European candidate.** 14B is the strongest **provisional accuracy candidate among these three**, not a selected release model. Its strict revised diagnostic recall is 15/16 per language, hunk precision 19/20 NL and 20/22 EN, and Dutch preservation 6/7. All remain short of one or more aspirational goals; semantic judgment is unresolved. Saved resource measurements also matter: 14B averages 4.975 s/case and 9.525 GB sampled process working set, versus 1.373 s and 2.933 GB for 3B on this host. These historical measurements are neither new measurements nor portable performance guarantees. Retain 3B as a cost/latency comparison if the next evaluation budget permits.

EuroLLM 9B Instruct 2512 is a reasonable **next family to evaluate after benchmark/protocol review**, not an evidence-based prediction that it will beat 14B. Its official card declares Dutch and English, EU funding, a European university/industry development consortium, and Apache-2.0. [Official model card](https://huggingface.co/utter-project/EuroLLM-9B-Instruct-2512). Those declarations support investigating it; they do not prove proofreading competence, all training-data rights, or supply-chain integrity. The repository's pinned candidate is `def82454026b0353f12e48a0de081a9ed380d91e`; the pinned card endpoint was unavailable through this audit's browser, so the live card check is not represented as a new pinned-artifact audit. Before any separately approved acquisition, verify that exact checkpoint lineage, tokenizer/config/license/notices, conversion toolchain and resulting GGUF/template/checksum. Language coverage including Russian or Chinese does not by itself establish model origin; inspect developer and base-model lineage. No weights were downloaded, no license was newly approved, and no EuroLLM 9B inference was performed here. Prior 1.7B performance neither validates nor rules out the 9B model.

## 8. Verification and stopping point

The reproducible analysis command is:

```sh
python3 benchmarks/model-validation/audits/quality-v1/reproduce.py
```

It compiles the unchanged evaluator/parser through an analysis-only project with cleared package feeds and no package/native inference dependencies. It verifies frozen hashes, 44 references, all 132 input/trace/prediction pairs, 132 exact parser replays, EOG metadata, exact historical accepted/raw metrics, proposal scope, all case deltas and complete adjudication coverage. No model inference can be invoked by that driver. The saved generations themselves remain the source of evidence.

Validation completed: analysis build and deterministic probes passed; existing evaluator/parser targeted tests **39 passed**; broader existing deterministic suite **42 passed, 1 skipped**. The skipped test requires real model inference and was deliberately disabled by removing `DIORTHOSIS_TEST_MODEL` from the test environment. Initial sandbox test attempts failed or stalled on .NET local IPC permissions; they were cancelled, and the deterministic tests passed with the approved sandbox exception and no restore. No cross-platform build, new inference, full document-fidelity test or new end-to-end offline model test was run: none is necessary for these analysis-only artifacts, and new inference is explicitly out of scope. Existing offline evidence was inspected, not re-certified; CI's network trace checks cover CLI startup/native initialization rather than arbitrary real-model workloads.

Only new audit/report artifacts were created. Production code, frozen prompt, dependencies, original dataset, historical results, PR state and protection settings are unchanged. No merge, release, model download, GUI/PWA/DOCX work or fine-tuning occurred. **Stop here for review and explicit approval before adopting annotations, changing scoring/safety behavior, or beginning further model evaluation.**
