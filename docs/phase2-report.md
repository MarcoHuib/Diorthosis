# Diorthosis Phase 2 report

## A. Executive summary

The executable engineering work is complete: independent AI technical reviews and fixes, a bound linguistic-review workflow, a 387-case provisional inventory with 80 new diverse candidates, deterministic analysis of 176 stored outputs,48 actual controlled format generations, and context/source-mapping readiness. **No independently human-validated quality result or model selection is claimed.** Production prompt `diorthosis-correction-v3` remains unchanged. No model, dictionary or runtime dependency was downloaded/adopted, no source document overwritten, and no PR merged or release made.

The new measurements give no reason to promote model-generated offsets: all three models deliver zero reference repairs through that protocol on the eight-case exposed panel. The engineering priority is credible source/passage adjudication and semantic coverage, rather than optimizing reference mismatches or accepting malformed output. Hybrid spelling remains postponed for explicit asset/license/evaluation prerequisites.

## B. Technical peer review

Baseline verified: clean synchronized `feat/local-ai-benchmarks` at `9801b57266fa96dd4b42efd731d33b84a177e160`, PR #2 open/unmerged,143/143 local real-model tests and all six CI publish/native-start targets passing.165 pre-existing benchmark artifacts were frozen before changes. Three separate fresh AI reviewer contexts inspected evaluator, safety, dataset/inference/pipeline code, and two reviewed the new foundation. These are adversarial technical AI reviews, not independent human linguistic approval.

PR #2 classification: **Verified defects fixed and retested in the bounded reviewed scope**. [Detailed review](phase2-technical-review.md), [machine findings](../benchmarks/phase2/v1/technical-findings.json), [foundation review](phase2-foundation-review.md). Confirmed defects included canonical Unicode offset crashes, shared edit grouping, unknown passage denominators, annotation-reorder review transfer, protected-span boundary insertions, question reassignment/statement omission, decomposed names/quotes/yearless dates, demonstrated negation/time/agent changes, literal structured candidate reparsing, malformed surrogate exceptions, compare overwrite, replay provenance and dirty cached native source. An inherited mutable protected-span collection was also fixed on PR #2.

New foundation review caught policy/metadata/disposition replay in context mapping, false/unknown metric conjunctions, absent/empty identity collisions, rejected-no-op quality credit, legacy identity caveat loss, parsed-text reconstruction and conflicting annotation selection. Regression tests now cover them. Separate reviewer reruns found no remaining defect in those bounded fixes. No automated approval or merge follows.

Remaining policy costs are explicit: temporal/personal anchors reject some legitimate compounds (`na denken→nadenken`, `voor bereid→voorbereid`, `after noon→afternoon`), Dutch pronoun-case repairs and clause reorder; capitalization can trigger name protection. Heuristics still cannot prove arbitrary meaning, negation scope, argument roles, legal equivalence or numerical relationships. Protection was not broadly weakened to improve acceptance.

## C. Benchmark quality

The [linguistic inventory](phase2-benchmark-inventory.md) covers44 smoke entries,263 exact candidate groups and80 new candidates:387 entries,362 exact distinct texts. It has549 explicit AI screening actions/proposals, with no historical annotation rewrite. Notable proposals include withdrawing grammatical Dutch reorder targets, recognizing British collective agreement, revisiting data/demonstrative alternatives, optional punctuation, additional unannotated article/plural errors, broad spans and unrealistic template substitutions.

New candidates:40/language, including24 proposed clean passages and16 error passages each (17 proposed annotated errors/language). They include varied email, informal, regional, technical, quote, date/number, mixed-language, negation/modal, paragraph, emoji/combining-mark situations. All have AI generation/source-scenario provenance and remain development-exposed. Related NL/EN scenarios and a shared AI author are disclosed; scenario labels do not establish independent origins.

Deterministic new-data audit:80/80 unique inputs,0 exact duplicates,0 near pairs at0.85,0 annotation-span issues. Cross-collection diagnostic:126 old smoke/synthetic overlaps,0 involving new candidates, original families incomplete. This does not prove semantic independence. No selection or held-out split is manufactured. Independently reviewed error/clean counts remain **0 per language**, against targets of200 errors and about300 clean passages/language.

[Blinded packet](../benchmarks/phase2/v1/review-analysis/review.html), [JSONL](../benchmarks/phase2/v1/review-analysis/subjects.jsonl):685 deduplicated work items for case annotations, raw/parsed/delivered passages/proposals and rejections. Every case requires human sign-off. Two reviewer slots, qualifications, alternatives/categories, ambiguity, protected spans, disagreements, versioned adjudication and pending status are supported. Distribute packet files without the separate model-lineage/analysis files; prior exposure/public trace access can still compromise blindness.

## D. Evaluator assessment

Deterministic bug fixes identify as `reference-groups-3.1.0`. Frozen3.0 reports are unchanged; historical v1 arrays reproduce exactly for all four saved runs, and v1/v2 implementation/tests remain compatible. Shared canonical edit components, actual candidate positions through normalization, unknown complete-reference judgment coverage and content-bound proposal IDs resolve demonstrated deficiencies. Complete reference matching remains limited by annotations and the256-combination cap; unknown is not failure.

The separate [review workflow](linguistic-review-workflow.md) imports judgments without altering predictions. Finalized linguistic metrics require two distinct independent human-declared reviewers and a separate human-declared adjudicator, qualifications/role metadata and matching content/reviewer versions. AI votes cannot approve. Identities and competence are external declarations, not authenticated by this software. Histories/disagreements are retained; stale approvals fail. No real people or approvals were invented.

Proposal precision, whole-passage success, clean preservation, critical semantic failures and rejection appropriateness use explicit judgments, each with numerator/denominator, unknown judgment coverage and descriptive Wilson interval. Definite failures stay failures even if another prerequisite is unknown. **All current linguistic estimates are null.** The legacy reference evaluator's optional imported proposal reviews are caller-supplied and unverified; only the separate workflow enforces the new review contract.

Raw availability, parsed candidates, assessable validated delivery, rejections and review-required states are separate. Rejected source no-ops cannot receive successful-delivery quality credit. Parsed predictions reconstruct exactly the extracted text, including incomplete candidates. Per-model reports project through separate checksummed declared lineage; shared judgments are reused without claiming independent observations. Source-case counts differ from review-subject counts; original source families and documents remain unknown. Twelve paired tables use jointly assessable, known reference judgments only, not edit-hunk independence or population significance claims.

## E. Model failure analysis

[Machine analysis](../benchmarks/phase2/v1/review-analysis/analysis.json), [176 linked output records](../benchmarks/phase2/v1/review-analysis/failure-analysis.jsonl), [summary](../benchmarks/phase2/v1/review-analysis/summary.md). Every record identifies original case, trace line/hash, report/hash, declared model revision/checksum, evaluator, current raw/parsed/delivered result and decision state. Historical timings remain solely historical claims. Legacy reports lack trace-binding hashes: that association is explicitly **unverified**, even though dataset/input alignment is verified. New format experiments separately verify actual local model hashes before load.

| Stored model | NL reference repairs | EN reference repairs | Parsed | Assessable delivered | Rejected | Missing inference predictions |
|---|---:|---:|---:|---:|---:|---:|
| Ministral 3B |11/16|11/16|44/44|42/44|2/44|0/44|
| Ministral 3 8B |12/16|11/16|44/44|39/44|5/44|0/44|
| Ministral 3 14B |14/16|12/16|44/44|42/44|2/44|0/44|
| EuroLLM 9B |9/16|14/16|44/44|44/44|0/44|0/44|

These are exposed, provisional reference observations, not linguistic recall/precision or a model ranking. Each language has22 cases and7 proposed clean inputs; source groups/documents are not known independent units. Exact source identities are retained:3B revision `be4e90342e4468f37d4bea40ec143997f9256475`;8B `0102285ad796bd99af90f58de616092e5630e970`;14B `fb49df4a3cde2c774da8def12437118a66c4f5cf`; EuroLLM source`f7ae2bc3bcbb538c0b93fa6cfbf388d9898b1ace` and quantizer`55e5590cc457e42ed197c34c5b10403472b05d54`. EuroLLM is the saved pre-2512 artifact.

All16 requested taxonomy categories are present, with counts/examples and unknown-status limitations. Current historical-AI/mechanical labels include12 output-level missing-reference alternatives,5 unnecessary rewrites,1 meaning change,1 incorrect grammatical correction,10 response-protocol deviations and2 ambiguous judgments. These categories can overlap; counts are not human-confirmed. Specific missed spelling/grammar/word-order/punctuation symptoms are separately reported. Zero in a category means unobserved/unassessed here, not absence of risk. Original AI records are preserved, including obsolete uncertainty flags that never constituted human approval.

Shared reference misses: `nl-12`, `nl-15`, `en-01`, `en-04`. They have different mechanisms: causal joining alternatives, modal coordination, tense/aspect, and a valid initial-Tomorrow alternative. A shared miss does not prove a model-training weakness.9 current stored-trace rejections preserve exact reasons; symbol/emoji and protected identifier changes need distinction from linguistic judgment. No unsupported document structure conclusion is possible from plain-text smoke cases.

## F. New experiments

[Predeclared plan](../benchmarks/phase2/v1/format-experiment/plan.md), [complete results](../benchmarks/phase2/v1/format-experiment/summary.md), [machine summary](../benchmarks/phase2/v1/format-experiment/summary.json).48 generations actually ran:3 available local models ×2 existing protocols ×8 exposed cases. Same model/settings/cases within each contrast; principal variable is full-passage output versus the existing structured-offset prompt/grammar protocol. No broad prompt search or new contract. Model/native/prompt/dataset hashes, source commit, runtime/OS/hardware, greedy CPU 9 threads/context 4096/token cap 256/90 s timeout, raw traces, termination, validation, latency and sampled load/inference memory are recorded. OS network denial was self-tested; no assets downloaded.

| Model | Protocol | EOG | Valid JSON | Assessable delivery | NL/EN reference repairs | Mean ms | Sampled peak GiB |
|---|---|---:|---:|---:|---:|---:|---:|
|3B|baseline|8/8|0/8|8/8|2/2;2/3|1431|2.73|
|3B|offsets|8/8|8/8|0/8|0/2;0/3|2722|2.84|
|14B|baseline|8/8|0/8|7/8|2/2;2/3|5179|8.86|
|14B|offsets|8/8|8/8|6/8|0/2;0/3|6973|8.98|
|EuroLLM 9B|baseline|8/8|1/8|8/8|2/2;2/3|3830|6.25|
|EuroLLM 9B|offsets|6/8|6/8|0/8|0/2;0/3|11806|6.39|

Baseline does not request JSON, so baseline JSON counts are not a quality criterion.14B offsets return six unchanged empty arrays and two rejected candidates, repairing nothing. EuroLLM offsets include two truncated outputs and six natural-EOG JSON responses that still fail delivery. Larger local models did not rescue direct offsets on this panel.3B adds the typo `startt` to correct Dutch `start`, accepted by the validator;14B's `OAuth2→OAuth 2` attempt is rejected. Conversely baseline en-04 is a grammatical reference-missing alternative. These expose the need for actual linguistic judgments.

No promotion or model choice. Sampling 100 ms can miss peaks; OS cache and competing development load were uncontrolled. Timings are diagnostic process measurements, not speed superiority, minimum RAM requirements or new versions of historical timings.

## G. Hybrid spelling assessment

[Follow-up and reproducible plan](phase2-hybrid-and-document-readiness.md), [bounded local inspection](../benchmarks/phase2/v1/hybrid-local-inspection.json). Prior license/version research was reused, not replaced with repeated speculative research. No PATH Hunspell/Nuspell or reviewed matching engine/dictionary bundle was found in inspected project/model/NuGet paths. Exact inspected versions/disclosures and missing immutable pins/native-transitive/dictionary/notice obligations are explicit.

Decision: **postpone because licensing/asset and evaluation prerequisites are unavailable**. No adapter/dependency is adopted and no dictionary-only/hybrid repair, overhead, latency or memory benefit is claimed. Later three-arm trial must compare actual paired repair gains/losses and false alerts, with untrusted hints and unchanged validation. Commercial permission is not assumed to satisfy permissive-native redistribution requirements.

## H. Context and document readiness

Opt-in `ContextCorrectionEngine`: explicit forced-language target, stable paragraph/segment identity, immutable document revision/hash, complete request-policy binding, bounded before/after read-only neighbors, source-aligned UTF-16/grapheme edits and copied opaque metadata. Mapping revalidates protection policy and refuses stale source, changed language/metadata, non-accepted edits and out-of-target changes. Tests include NL/EN, emoji/NFD, duplicate/overlapping identities, source mismatch, protected boundaries and translation attempts.

Native AOT exercises grammar/EOG with the real local3B and context mapping with a fake adapter; two additional real NL/EN context contract probes are recorded separately. These are isolation/startup checks, not evidence of improved linguistic quality or universal translation prevention. The [fixture design](../benchmarks/phase2/v1/context-structure-fixtures.json) covers paragraphs, tables, headings, lists, links, headers/footers, footnotes, mixed formatting, identifiers, embedded content and tracked changes.

Opaque metadata is retained, not reformatted or run-offset-remapped after text changes. No parser/editor/exporter, `.docx` round trip or legacy binary `.doc` preservation was implemented/claimed. Those require a separate licensed-parser and representative-file fidelity milestone. Original documents remain untouched.

## I. Security and privacy

All model inference stayed local, CPU-only, without a listening inference service. macOS OS network denial was self-tested to reject loopback connections with permission denial before inference checks; diagnostics were disabled. Native AOT CLI and probes launched with nonexistent `DOTNET_ROOT`, requiring no installed .NET runtime. Python is developer tooling only. No Java, cloud language API, telemetry, automatic download, production dependency or license notice change was introduced.

Compare/replay outputs use exclusive creation; dataset/available trace bindings are validated; legacy model/resource claims are isolated. Native CI builds use fresh clean pinned official source with origin/revision/dirty checks. Protected content has programmatic safeguards and all user suggestions remain reviewable. Tests are bounded evidence, not a complete security audit or semantic proof. No real employer/private documents or model binaries were committed.

## J. Testing and CI

- Baseline:143/143 real-model tests. After PR #2 fixes:170/170; inherited immutability regression additionally passed.
- Final dependent-branch suite: **185/185 passed,0 skipped**, including available real local3B EOG, token cap, cancellation/recovery and request isolation. Model-free run184passed/1explicit real-weight skip.
- Python tooling:10/10 passed. Dataset audits, generated packet bindings, per-model projections and no-rejected-no-op credit checks passed. Separate reviewer reruns passed their concrete evaluator/context/analysis probes.
- Native AOT CLI publication and launch/native-backend/NL/EN/injection checks passed under network denial and missing runtime lookup. Grammar reached EOG with valid JSON; context mapping/metadata checks passed. Actual-format fidelity remains unrun because no parser/exporter is in scope.
-84 historical artifact checks plus165-file Phase2 baseline verified unchanged. Four saved historical v1 metric arrays reproduce exactly; v1/v2 regression tests pass.
- PR #2 at`91af4e14ec65ab6fed3aaf305fd0ce6fc52bc6f6`: tests and all six publish/native-start targets green, open/unmerged. Dependent PR CI status is recorded in the final delivery; full-weight inference has only been exercised locally on macOSARM64. CI native-start success on other RIDs is not real-weight inference validation.

[Validation record](../benchmarks/phase2/v1/validation.json) and [evidence manifest](../benchmarks/phase2/v1/manifest.json) provide exact checks/hashes. CI now verifies Phase2 evidence and imports the frozen review packet, alongside prior dataset/historical tests.

## K. Git delivery

Confirmed PR #2 defects remain on [PR #2](https://github.com/MarcoHuib/Diorthosis/pull/2), branch `feat/local-ai-benchmarks`, with baseline/fix/immutability commits. Substantial new features/data/analysis live on separate `feat/linguistic-review-foundation`, stacked on that unmerged branch. The dependent PR targets `feat/local-ai-benchmarks`, not `main`. The inherited policy fix was merged from the base branch without force-pushing; changes are not duplicated in the dependent diff. Logical checkpoint commits preserve technical review and source/evidence lineage.

No PR merge, auto-release, force push or protection-setting change. Human code review and an explicit human merge decision remain required. Historical evidence is immutable; final packet/artifact versions are newly checksummed.

## L. Decision matrix

| Component/decision | Outcome | Evidence needed next |
|---|---|---|
|Confirmed evaluator/safety/artifact defects|Fixes retained and tested|Human code review; broader semantic regressions|
|Reference evaluator3.1|Current diagnostic matcher|Independent annotations/alternatives; no precision proxy promotion|
|Production promptv3|Retained unchanged|Credible paired improvements on reviewed selection data|
|Direct model offsets|No promotion|Current small-panel evidence is negative across3models|
|Official model choice|Deferred|Reviewed held-out NL/EN quality, safety and portable resources|
|Human linguistic metrics|Implemented, all currently unknown|Qualified independent reviewers and adjudicator declarations|
|New candidate data|Development-only provisional|Real annotation/clean-label and source-family approval|
|Hybrid spelling|Postpone|Compatible reviewed native/dictionary assets plus actual repair benefit|
|Context API|Opt-in readiness only|Adjudicated context quality and representative document mapping|
|`.docx`/`.doc` fidelity|Separate milestone|Licensed parsers and demonstrated real-format round trips|

## M. Remaining blockers

Independent human linguistic validation is unavailable: no real qualified Dutch/English reviewer identities, two independent decision sets or adjudicator approvals were supplied. Therefore true linguistic precision, complete-passage quality, meaning preservation,99% clean preservation,90% repair targets, approved selection data and final held-out model choice remain blocked. All engineering independent of those inputs proceeded.

No compatible reviewed local spelling-engine/dictionary bundle exists in inspected paths; hybrid trials remain blocked by exact asset/license/native packaging prerequisites. Corpus independence/realism and incomplete alternatives need human resolution. Format-preserving document work lacks parsers and representative rights-cleared round-trip fixtures. Broader semantic robustness, false-rejection tradeoffs, Windows/Linux real-weight inference and minimum-device resource limits remain unproved. No AI screening or statistical interval substitutes for these prerequisites.

## N. Recommended next milestone

Recruit real qualified NL/EN reviewers and a separate adjudicator, verify declarations/blindness, review source annotations and 685 bound work items, and resolve proposed withdrawals, alternatives, clean labels and semantic disputes. Develop genuinely independent rights-cleared sources toward 200 errors and 300 clean passages/language; assign source families before freezing selection/final held-out splits and protect final predictions from tuning.

Then compare unchanged full-passage baselines using paired independently judged repairs/precision/passage success/rejection quality and critical semantic outcomes. Focus on the demonstrated modal/tense/causal-joining failures and false rejections, without accepting offsets or weakening protected content merely to increase coverage. Acquire a compatible spelling bundle only after review, run the predeclared three-arm repair comparison if justified, and keep document parser/export fidelity as its own coherent milestone. No production model or strategy should be selected until that evidence exists.
