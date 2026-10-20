# Diorthosis quality engineering report

2026-10-20. Implementation checkpoint: `72d6f35`. Evidence is deterministic computation or local inference unless explicitly identified otherwise. No independent linguistic or legal approval is supplied.

## 1. Executive summary

The benchmark pipeline is more auditable: the 600-record collection is transparently grouped into 263 exact-input groups with complete provenance; v3 scoring separates local repair, reference groups, coverage and passage success; public correction results preserve rejection/review diagnostics; and source/Unicode/meaning-scope safeguards have regression coverage. All 143 local tests pass, including the formerly skipped real-weight integration test. Real inference, the published Native AOT CLI and the experimental grammar path executed with OS-enforced network denial.

The controlled prompt/edit panel did **not** justify a new production prompt or structured-output default. The grammar termination defect was corrected, but generated offsets remained unreliable. Default prompt v3 and LLM-only operation are retained. There is no independently validated held-out dataset, model selection, release approval, spelling-engine integration, or document round-trip support.

## 2. Repository changes

- `DatasetAudit.cs` / `DatasetCommands.cs`: exact/near audit, annotation diagnostics, provenance groups, bounded input and split checks; versioned artifacts under `benchmarks/rehabilitation/v1/dataset/`.
- `ReferenceEvaluator.cs`: opt-in `reference-groups-3.0.0` and explicit proposal-review API. V1/v2 methods remain available. Separate deterministic replays are in `benchmarks/rehabilitation/v1/evaluator/`.
- Core `CorrectionModels.cs`, `SourceEdits.cs`, `ModelCorrectionEngine.cs`: observable result states, exact protected spans, grapheme validation, scope/quotation review and shared termination gating. CLI JSON exposes disposition and reason without raw candidates.
- `ExperimentalCorrection.cs`, `CorrectionExperiment.cs`: opt-in language/few-shot/offset/fragment prompts, optional pinned-runtime GBNF and complete experiment manifests. No ordinary correction prompt change.
- `SegmentEditMapper.cs`: exact target-to-source offset validation only. `tests/NativeGrammarProbe/`: a small standalone AOT runtime probe, outside ordinary model-free CI.
- New regression suites and developer replay/validation scripts; CI verifies historical hashes and deterministic dataset regeneration, while retaining all six publish/native-start targets. ADRs 0009–0012 and focused review/research documents record decisions and limitations.

## 3. Implementation summary by phase

| Phase | Status | Evidence / remaining work |
| --- | --- | --- |
| 0: audit/baseline | Completed | Clean initial branch `91dd454`, PR #2 open/unmerged at `968f4dc`, no open issues; original 81 pass / 1 skip. Implementation matrix in `quality-engineering-plan.md`. |
| 1: dataset rehabilitation | Completed deterministic deliverables; linguistic approval pending | Exact/near reports, consistency report, grouped candidate, ID map, manifests, split checker, protocol, gap analysis and tests. No invented held-out split or repetitive generator. |
| 2: evaluator | Partially completed | Versioned reference evaluator and regressions/replay implemented; true bilingual linguistic grouping, full-passage alternatives, adjudication governance and independent precision remain unresolved. |
| 3: outcomes/safety | Completed bounded implementation; semantic validation pending | Every outcome observable; non-deliverable candidates retained separately; termination, explicit spans, strict JSON/Unicode and scope regressions. General semantic equivalence is not solved. |
| 4: prompt experiments | Completed bounded experiment | Three full-passage configurations compared on eight development cases. No promotion. Larger independent comparison pending. |
| 5: structured output/grammar | Completed deterministic pipeline and bounded native experiments; promotion withheld | Offsets and uniquely anchored fragments tested; syntax/offset failures measured; grammar v1 preserved and v1.1 tested; macOS ARM64 Native AOT grammar probe passed. Foreign-platform grammar, long-output and broad quality evidence pending. |
| 6: spelling/hybrid | Partially completed | Primary-source feasibility/license study and explicit postpone decision. No approved pinned dictionary/engine bundle, three-arm repair experiment, or package-resource measurement. |
| 7: model comparison/profiles | Partially completed | Four saved runs rescored consistently; existing 3B reused for new behavior. No new weights; 8GB/balanced/quality profiles and language-specific pairing remain hypotheses. |
| 8: document context | Partially completed | Target-mapping tests and context/format design; no context-assisted inference or document parser/exporter. |
| 9: advanced techniques | Research note completed; experiments postponed | Paper identified; no voting, self-verification dependency, training or LoRA. Trusted data and resource evidence come first. |

## 4. Benchmark rehabilitation

| Property | Recomputed |
| --- | ---: |
| Records | 600 (300 NL, 300 EN) |
| Exact unique input texts | 263 (116 NL, 147 EN) |
| Duplicate records beyond first | 337 |
| Duplicate groups | 86 |
| Records in duplicate groups | 423 |
| Maximum multiplicity | 23 |
| Exact-group annotation disagreements | 0 |
| Invalid annotation/source/grapheme issues detected | 0 |
| Diagnostic near pairs at 0.85 Dice threshold | 56 |
| Derived review groups | 263; all 600 IDs mapped |
| Unique clean inputs | 15 per language |

No historical dataset was rewritten. Near pairs are never collapsed. Zero exact-group disagreements means literal gold annotations agree within those groups; it does not establish their linguistic correctness. Case/punctuation/normalization diagnostics preserve literal alternatives. The current tool regenerates all frozen candidate files byte-for-byte.

All reviewed development material remains development-exposed. No final held-out split exists. Cross-partition exact/near/source-family overlaps fail the checker; unknown template families are explicitly reported. Reviewers must provide family metadata and fresh material. Targets of 200 independently reviewed errors and about 300 varied clean passages per language remain unmet. See `dataset-review-protocol.md` for two-reviewer adjudication, rights checks, dependency annotations and the detailed coverage gaps.

## 5. Evaluator changes

V1 is available through `evaluate` / `EvaluateHistorical`; existing v2 methods/constants remain unchanged. The four saved model v1 metric arrays reproduce exactly. V3 reconstructs candidate output, derives a canonical comparison diff, scores source annotation/dependency groups and reports unrelated residual groups separately. Representation regression tests use local, compound and complete-passage edits producing the identical output. Mixed-category repairs retain category recall; a mixed dependency proposal contributes to the all-category precision denominator once.

V3 preserves local repair recall alongside an unrelated lexical or speech-act change. Complete-reference passage success remains separate. NFC is a comparison rule only; sources, offsets and delivered text are not normalized. Apostrophe/word-order alternatives remain explicit. Missing predictions lower coverage and cannot masquerade as observed clean no-ops. Empty ratios are null; counts and descriptive Wilson intervals accompany proportions.

Reference-group precision is **not independently assessed necessary correction precision**. Unmatched proposals may be valid alternatives and require adjudication. Candidate-bound review labels can be recorded, but none were supplied. True linguistic precision and semantic acceptability therefore remain unknown. Canonical residual grouping can still reflect alignment choices; arbitrary bilingual proposal grouping is not solved. Annotation dependencies must be independently reviewed. Paired superiority inference is not warranted on these tiny dependent development samples.

## 6. Safety changes

Results now distinguish Accepted, Unchanged, Suppressed, Rejected and NeedsReview while preserving the `Edits` compatibility surface. Raw generation, parsed candidate, raw candidate edits and deliverable edits are separate. Only natural EOG permits processing; replay also uses this gate. Token-limited, unknown or cancelled generations deliver no edits.

Strict JSON rejects wrong shapes/types, extra/duplicate properties, invalid original strings, overlap, ambiguous insertions, oversized data and invalid UTF-16/grapheme boundaries. Optional literal protected spans protect legal/factual text while permitting valid independent changes outside those spans. Detected changes in negation/modality association across contrast/coordination/conditional clauses, quotations or paragraphs require review. Interior question/exclamation changes are blocked. Existing name/number/date/URL/identifier/emoji protections remain; Windows directory names with spaces now remain part of a protected extensionless path.

Replay of 176 historical generations retained the known `node-04` block. This was already fixed by earlier safety work; it is not claimed as newly discovered quality gain. Additional rules are demonstrated by synthetic adversarial tests. No partial hunk salvage is enabled because linguistic dependencies cannot be inferred safely from a diff.

Residual risks include same-clause scope, temporal relations, added propositions using familiar vocabulary, unrecognized names/legal terms, ambiguous unquoted paths and false rejections of legitimate quotation/paragraph/marker repairs. Finite tests do not establish zero semantic risk. No source document export or mutation was introduced.

## 7. Prompt experiments

The predeclared eight-case panel contains four inputs per language, with two clean per language and respectively two NL / three EN annotated errors. It is exposed, purposively selected development data. Same 3B artifact, CPU backend, context 4096, nine threads, greedy sampling, 256-token cap, 90-second timeout and safety policy were used. All runs had OS network denial. Metrics use the original provisional references; unknown alternatives remain uncredited.

| Configuration | NL repairs | EN repairs | Clean preservation NL / EN | Rejected / 8 | Mean latency | Sampled process peak |
| --- | --- | --- | --- | ---: | ---: | ---: |
| Frozen baseline | 2/2 | 2/3 | 1/2 / 2/2 | 0 | 1.421 s | 2.932 GB |
| Language minimal | 1/2 | 2/3 | 1/2 / 2/2 | 2 | 1.750 s | 2.946 GB |
| Few-shot | 2/2 | 2/3 | 1/2 / 2/2 | 3 | 1.880 s | 2.964 GB |

Minimal prompting made a Dutch subordinate-clause failure; few-shot matched observed repair counts while increasing latency/rejections. No prompt improved this small panel. Clean preservation is delivered behavior and must be read with rejection counts. `en-04` includes the known incomplete-reference issue, so 2/3 is a reference-match count, not a linguistic verdict. No winner is selected or quality gate claimed. Process-load measurements are cache-dependent, not cold boot; all raw traces, prompt hashes and settings are retained.

## 8. Edit-based experiments

| Configuration | Valid JSON / 8 | Strict offset shape / 8 | Natural EOG / 8 | Delivered repairs NL / EN | Mean latency | Sampled peak |
| --- | ---: | ---: | ---: | --- | ---: | ---: |
| Unconstrained offsets | 0 | 0 | 8 | 0/2 / 0/3 | 2.331 s | 2.967 GB |
| Unconstrained fragments | 0 | N/A (fragment contract) | 8 | 0/2 / 0/3 | 2.123 s | 2.964 GB |
| Offset grammar v1 | 8 | 8 | 6 | 0/2 / 0/3 | 4.934 s | 3.046 GB |
| Offset grammar 1.1 | 8 | 8 | 8 | 0/2 / 0/3 | 2.732 s | 3.053 GB |

Unconstrained responses were Markdown-fenced, rejected under the declared strict format. Grammar enforces syntax, while wrong lengths/original strings/offsets still fail. V1 had one literally aligned empty array that reached the token cap, and it was rejected; an empty array is not a repair. V1.1 had zero literally aligned responses. V1 sampled 717 total tokens across eight inputs; v1.1 sampled 266, versus baseline 129. All four structured configurations rejected all eight responses; their delivered clean preservation cannot be presented as model quality.

V1's root allowed unlimited trailing whitespace after a complete array. Removing only that suffix resolved the two observed cap terminations; original grammar/traces remain frozen. V1.1 is retained as a contract fix, with no correction-quality promotion. A standalone Native AOT grammar probe emitted a valid empty array, reached EOG in two sampled tokens and returned Unchanged with .NET discovery disabled and networking denied. This does not establish all JSON Schema features or other-platform grammar inference.

## 9. Hybrid spelling research

No engine or dictionary is integrated. The [study](spelling-and-future-research.md) records primary-source Hunspell v1.7.2 tri-license, Nuspell/ICU disclosures, OpenTaal 2.20.21 affix/license evidence and English database variant/licensing documentation. Moving-branch evidence and a license label are insufficient pins or approvals. No new native dependency or notices entry is added.

A future three-arm comparison must measure real contextual repairs and false positives, including Dutch compounds/separable verbs/participles/dt/diacritics/regional words and technical clean text. Dynamic/static copyleft obligations, native C ABI, ICU/compiler runtimes, dictionary provenance and six-platform artifacts remain unresolved. There are no measured hybrid quality, package-size, memory or latency results. Decision: postpone integration.

## 10. Model evaluation

Four frozen predictions were rescored under the same v3 reference evaluator and original 44-case dataset. These are diagnostic counts over 16 annotated errors per language, not fresh inference or independent quality evidence:

| Existing artifact | V3 reference repairs NL / EN | Historical mean case latency | Historical sampled process peak |
| --- | --- | ---: | ---: |
| Ministral 3B | 11/16 / 11/16 | 1.373 s | 2.933 GB |
| Ministral 3 8B | 12/16 / 11/16 | 3.148 s | 6.269 GB |
| Ministral 3 14B | 14/16 / 12/16 | 4.975 s | 9.525 GB |
| EuroLLM 9B | 9/16 / 14/16 | 3.448 s | 11.322 GB |

The saved report/checksum/revision lineage is linked in the replay manifest. Historical timings/memory were not remeasured during replay and cannot be treated as the costs of the new experiments. EuroLLM 9B here is the previously evaluated `utter-project/EuroLLM-9B-Instruct` artifact, **not** an untested 2512 replacement. No automatic download or model upgrade occurred. EuroLLM 1.7B earlier results have runtime/history comparability limits and were not added to this four-run comparison.

Fresh inference used only the existing reviewed-for-evaluation Ministral 3B file, 2,147,023,008 bytes, SHA-256 `9ed150d4367e68df0ac8e1540f6ddc65b42d0ee26378329d1ecbca60f93fc5f8`, revision `be4e90342e4468f37d4bea40ec143997f9256475`. Hardware: Apple M4, 34,359,738,368 physical bytes, macOS ARM64, .NET 10.0.12, pinned LLamaSharp 0.27.0 / llama.cpp `3f7c29d318e317b63f54c558bc69803963d7d88c`, Metal-disabled CPU backend. Native hashes and per-case prompt/token/latency details are recorded. No 8GB-machine profile, model-switch benchmark, independent semantic comparison or language-specialized pairing is substantiated.

## 11. Testing and CI

Local final Release build: 0 warnings, 0 errors. Full suite with the existing local 3B model: **143 total, 143 passed, 0 failed, 0 skipped**. Earlier model-free checkpoints correctly reported one skip. Tests cover dataset conflicts/splits, historic reproduction, categories/grouping/dependencies, missing/rejected results, normalization, strict JSON/Unicode, protected spans, scope, fragment ambiguity, target boundaries, grammar forwarding, real EOG/isolation/cancellation and unchanged text. `test-evidence.json` preserves counters and source hashes; full TRX files are local ignored developer artifacts.

Published macOS ARM64 Native AOT CLI: version/native backend checks, Dutch and English spelling probes and an embedded-instruction source probe passed with OS network denial and nonexistent DOTNET_ROOT. The embedded instruction remained unchanged, including `node-04`; this single example does not prove prompt-injection immunity. A loopback-connect self-test verified that the isolation policy actively rejects network use. Grammar Native AOT probe passed separately. No Windows/Linux real-weight or grammar inference was executed locally.

Initial AOT probe compilation encountered the already-documented LLamaSharp IL3000 fallback-resolver warning; the same narrow CLI suppression was applied, with AppContext-based native search paths retained. One native-CLI development driver attempt failed decoding non-UTF-8 native stderr; its failed record is preserved, and the corrected driver completed the checks. A mislabeled checksum in the first grammar-probe metadata was preserved and corrected in `metadata-correction.json`; it had identified the model rather than the probe binary. These recording/build issues are not omitted or counted as passing tests.

All 84 frozen historical benchmark hashes remain unchanged. CI now verifies those hashes and byte-identical dataset regeneration, retaining Windows/macOS/Linux x64/ARM64 publish/native-start jobs. Baseline GitHub run 39385781648 was green at remote `968f4dc`; validation of the pushed implementation is recorded in the CI addendum below. Startup checks do not establish production inference on every target.

## 12. Security and licensing

No cloud inference, document upload, telemetry, automatic download, listening app service, Java dependency, prerequisite end-user runtime, model upgrade or new third-party package was added. The only new Python scripts are developer tooling. Existing model/native assets were reused outside Git; no weights or private documents were committed. The application does not fetch the sources linked by research documentation.

Licenses/provenance of selected release assets, complete existing native redistribution notices, training-data lineage and signing/notarization obligations remain release work. No research source is marked legally approved. THIRD_PARTY_NOTICES stays unchanged because no dependency is adopted. Model-European-origin policy is preserved; upstream disclosures are not relabeled an independent supply-chain audit.

## 13. Git changes

Continued `feat/local-ai-benchmarks` and existing PR #2. No history rewrite, force push, merge, release or protection-setting change. Checkpoints:

- `95d5b96`: baseline, historical hashes and acceptance matrix.
- `b9e1be4`: dataset audit/provenance/split checks.
- `976a807`: v3 evaluator and historical replay.
- `1efe94b`: result outcomes and conservative scope review.
- `5acab13`: versioned prompt/edit/grammar experiment contracts.
- `bbcf355`: negative panel evidence, grammar follow-up, context mapping and research decisions.
- `72d6f35`: final offline/AOT/model-test evidence and Windows path safeguard.

Initial local commit `91dd454` was also ahead of origin and is preserved. The final report/documentation commit follows these implementation checkpoints; inspect `git log` for its SHA. PR remains open/unmerged. Final working-tree/remote state is recorded in the delivery message.

## 14. Remaining risks

Independent bilingual review, varied clean coverage, reliable source/template families and a fresh held-out benchmark are absent. References are incomplete and annotations have not been independently adjudicated. Reference-group precision and Wilson intervals must not be used as production guarantees. Automatic general semantic/linguistic validation and bilingual proposal grouping remain unsolved.

Structured output from this 3B model is operationally inferior on the fixed panel. Strict rejection protects delivery while masking raw quality if counts are omitted. The panel is tiny and exposed; larger models/hardware profiles are not validated by parameter count. Real-weight testing remains macOS ARM64 only. Spelling licensing/native packaging and real document fidelity remain unresolved. No finite suite proves zero residual unsafe accepted edits.

## 15. Recommendations

Retain deterministic auditing, v3 as an explicit diagnostic evaluator, observable outcomes and validated safety/grammar fixes. Retain the frozen default prompt and LLM-only baseline. Do not promote language/few-shot/structured experiments or a larger model from these data. Highest-value next work: independent annotation/adjudication and family metadata, fresh varied clean material, followed by predeclared same-setting held-out comparisons. Resolve linguistic proposal grouping and temporal/scope failure coverage before further aggregate-score optimization. Acquire spelling assets only after exact license/provenance review and a justified isolated experiment. Keep document parsing/export separate until fidelity can be demonstrated.

## 16. Acceptance decision

**Experimental results insufficient for promotion.** The bounded deterministic implementation is ready for code review; linguistic quality, semantic safety at the product gate, cross-platform real inference and release obligations remain unverified. No 90%, 95%, 98% or 99% product target is claimed achieved.

### CI addendum

Implementation commit `72d6f35f2951b26a52c85fbf00f2e20ed021cbe4` passed both GitHub CI runs [39400360075](https://github.com/MarcoHuib/Diorthosis/actions/runs/39400360075) and [39400365515](https://github.com/MarcoHuib/Diorthosis/actions/runs/39400365515). Each passed the tests job (including frozen hashes and byte-identical dataset regeneration) and all six publish/native-start jobs. The recorded source-checkpoint job results are in `benchmarks/rehabilitation/v1/ci-source-checkpoint.json`. These model-free cross-platform checks complement macOS ARM64 real-weight tests; they do not establish foreign-platform model or grammar quality. The following documentation/evidence-only commit does not change the tested application source.
