# Phase 3: linguistic quality experiments and AI feedback

2026-10-21. **Keep production prompt v3 and greedy LLM-only defaults.** On this development panel, dictionary hints gain a Dutch repair but lose an English grammar repair. Three-sample voting loses a Dutch repair versus greedy and reduces assessable English clean-text coverage. These are provisional AI findings, not independent human validation or model-selection evidence.

The [blinded ChatGPT review ZIP](../benchmarks/phase3/v1/chatgpt-review.zip) contains only the review instructions, literal before/after subjects and offline HTML cards. It includes **119 bound work items** covering source annotations, raw responses, delivered passages and rejected candidates. Source IDs and model/arm identities are withheld; fictional text stays literal. This is not a general personal-data anonymizer. No text was uploaded to ChatGPT or another external evaluator. Independent ChatGPT assessment is pending; keep the separate preparer decisions and model lineage withheld until an external reviewer locks their decisions.

## A. Existing baseline and evidence identity

Base: PR #4 head `09e89d164b2a6a3711044fc007ad4749ddec36e4`, open and unmerged over PR #2. The [predeclared protocol](phase3-plan.md) froze 20 Phase 2 fictional development passages: ten per language, six error passages and four proposed clean passages each, with **seven annotated errors per language**. Full original source/provenance metadata is retained in [panel.jsonl](../benchmarks/phase3/v1/panel.jsonl). The existing audit found 20 exact unique inputs, zero near pairs at its declared threshold and zero annotation-span issues. The panel is development-exposed, not independent held-out material.

The bounded run at implementation commit `9c4a0ac` generated **100 actual outputs**: production greedy, greedy dictionary hints, and three stochastic samples per input. Sample 101 is also the single-sample control; it is not a separate fourth stochastic generation. Twenty voted outcomes are derived results. No new model weights were downloaded.

Model: existing official `mistralai/Ministral-3-3B-Instruct-2512-GGUF`, revision `be4e90342e4468f37d4bea40ec143997f9256475`, Q4_K_M, SHA-256 `9ed150d4367e68df0ac8e1540f6ddc65b42d0ee26378329d1ecbca60f93fc5f8`, 2,147,023,008 bytes. Existing origin/license review is in [model validation](model-validation.md). Runtime: LLamaSharp/CPU backend 0.27.0, llama.cpp `3f7c29d318e317b63f54c558bc69803963d7d88c`. [Run manifest](../benchmarks/phase3/v1/run/manifest.json) and [literal trace](../benchmarks/phase3/v1/run/trace.jsonl) retain native hashes, framework/OS/CPU/RAM, prompt strings/hashes, seeds, completion, raw/extracted/delivered layers and resources.

Software-identity correction: a developer rebuild replaced on-disk assemblies while the already-loaded process was executing. Preserve both manifests and use the start-of-run hashes in [software-identity-correction.json](../benchmarks/phase3/v1/run/software-identity-correction.json), not the final on-disk experimental assembly hash. The harness now captures software hashes once at start. A later AOT fix writes the identical hint JSON using Utf8JsonWriter. Replay confirms every literal prompt and delivered analysis agrees; it does not turn the old run into inference by the later binary.

## B. Spelling engine, dictionaries and licenses

An isolated `Diorthosis.Experiments` assembly uses **WeCantSpell.Hunspell 7.0.1**, source `ab5709d95b2d23541984d22baa0ab2d1e783582f`. The net10.0 package has no transitive package dependencies and no unmanaged dependency. Select its MPL-1.1 license path; this is commercially usable copyleft, **not permissive**. Preserve the exact package notice and provide covered source at the pinned upstream revision if distributing this experimental binary. No upstream code was modified. The CLI does not reference this assembly or package. Production release packaging is not approved.

OpenTaal `.dic`/`.aff`/license/date files are matched at `ad0351b7c1a862f05d88131046a054f1cc84944a`; the date file identifies 2.20.23. BSD-3-Clause is selected, with the full attribution retained. English uses official SCOWL-derived size-60 US and GB-ise 2026.02.25 releases at source `7e99edab8e32f9f9ea2b15f249ca8d4d67237410`. Both full README copyright/affix notices are preserved, including the BSD affix terms. The source discloses COCA-derived information and asserts redistribution rights; no independent data-rights audit is claimed. [Asset hashes/URLs/bytes](../benchmarks/phase3/v1/assets.json), [notices](../benchmarks/phase3/v1/licenses/English-Copyright.txt), [THIRD_PARTY_NOTICES](../THIRD_PARTY_NOTICES.md) and [ADR 0015](decisions/0015-isolated-dictionary-and-guarded-voting.md) record the review. Reacquisition into a fresh temporary directory reproduced every pinned hash.

The adapter checks words, returns at most five suggestions, supports explicit US/GB/both English configuration, reads matching local pairs, reports missing pairs, and releases managed dictionary references on disposal. Dictionary absence has no automatic authority to edit or reject text. The union policy recognizes both `colour/color` and `organise/organize` without rewriting dialect.

[Actual probes](../benchmarks/phase3/v1/run/dictionary-probes.json) recognize `start`, `onmiddellijk`, `rapport`, `langetermijnplanning`, `opgebeld`, `coördinator`, `reünie` and Belgian `goesting`; reject `startt`, `onmiddelijk`, `raport`; and suggest the intended spelling. Both `word/wordt` and English `is/are` are recognized: dictionary lookup cannot decide contextual agreement. `termijnplanning` is unrecognized while the complete compound is recognized. Source `verstuurt` is a valid word despite being wrong after `heeft` in this passage. `can't/can’t` work. Correct uncommon `sesquipedalian`, product name `Diorthosis` and technical identifiers are unrecognized in the selected data. Those are coverage limitations, not linguistic error judgments.

## C. Hybrid comparison and primary AI feedback

The unchanged production prompt is the baseline. Hints use separately versioned `dictionary-hints-1.0.0`: optional alternatives are untrusted JSON, the source remains authoritative, and all existing validators still run. Post-diagnostics inspect complete changed words even when the canonical diff is a single-character insertion. They identify the historical `start → startt` failure in [known-regression.json](../benchmarks/phase3/v1/run/known-regression.json).

| Method | NL reference repairs | NL preparer-AI repairs | EN reference repairs | EN preparer-AI repairs |
|---|---:|---:|---:|---:|
| Greedy baseline | 5/7 | 5/7 | 6/7 | 6/7 |
| Dictionary hints | 5/7 | 6/7 | 5/7 | 5/7 |
| Single stochastic control | 2/7 | 2/7 | 5/7 | 6/7 |
| Three-sample voting | 4/7 | 4/7 | 6/7 | 6/7 |

**AI linguistic reading is the primary development feedback.** Hints repair `bijalge → bijlage`, which greedy leaves unchanged. Hints also produce valid Dutch subordinate order `het bestand opgeslagen heeft`; the frozen reference accepts only `het bestand heeft opgeslagen`, so the reference score hides the Dutch gain. Conversely hints repair `equiptment` but leave `The volunteers was ...`; greedy repairs both errors. These concrete gains/losses, rather than score-only optimization, guide the decision to keep hints experimental.

The stochastic control repairs the English article locally but adds literal `*experienced*` emphasis that remains in its delivered passage. Its local repair and whole-passage quality differ. Raw bold markup elsewhere is stripped; raw responses and delivered text must be assessed separately. No reference or AI score is a human precision estimate.

Dictionary-only diagnostics flag six NL tokens: four useful error-related warnings and two valid identifiers; four EN tokens: two misspellings and two valid identifiers, according to the preparer AI. They perform **zero repairs by design**. They miss valid-word grammar and do not resolve compound context. Blindly substituting `langetermijnplanning` for `termijnplanning` would leave the preceding `lange`, illustrating why hints cannot be automatic replacements.

Post-diagnostics record seven recognized-source/unrecognized-target transitions across stochastic samples; none in baseline/hints/voting primary outputs. They flag corrupt Dutch replacements, but miss recognized-word lexical drift. They remain diagnostic-only. Zero legitimate repairs blocked by the dictionary here is a consequence of not gating delivery, not proof that a future automatic filter has zero false positives. See [summary-v2.json](../benchmarks/phase3/v1/summary-v2.json) for counts and source bindings.

## D. Local edits and relationship to GECToR

The existing tested grapheme/UTF-16 diff is reused. Local operations retain exact source text/positions and identify INSERT, DELETE, REPLACE, MERGE and SPLIT; unchanged KEEP content remains copied. Reconstruction is verified. Equivalent complete corrected passages canonicalize identically regardless of an upstream full-span versus local-span representation. Passage edits remain dependency-linked; no syntactic independence is inferred from distance alone.

Repeated whole-word alignments that reconstruct the same passage at different source spans are explicitly marked ambiguous and excluded from voting. Invalid spans, overlaps and grapheme splits remain blocked. This is a bounded additional guard, not a claim that every possible bilingual alignment ambiguity is solved.

[GECToR](https://aclanthology.org/2020.bea-1.16/) is a trained token-tagging architecture. This work adapts local-operation ideas; it does not implement its learned transformations or reproduce its performance. No pretrained tagger, model-generated numerical offsets or new diff engine is introduced.

## E. Majority voting

[Goto, Sakai and Watanabe](https://aclanthology.org/2026.bea-1.60/) aggregate edits from sampled candidates. Their paper uses eight candidates, development-tuned thresholds, temperature/top-p 1, four-shot prompts, and ERRANT/JP-ERRANT. The [inspected MIT research implementation](https://github.com/gotutiyan/gec-llm/blob/a2dfc08bb01b4516eb402398c2fe14129ed8f45e/src/gec_llm/ensembles/voting.py) counts edit keys and greedily resolves overlap by descending support. No research code or weights were imported.

Our declared adaptation uses one local model, three seeded generations (101/202/303), temperature 1, top-p 1, no top-k/min-p/typical/repetition filtering, the existing zero-shot prompt and source diff, and strict threshold 2. Each generation counts once per edit. Rejected/truncated candidates never lower the denominator. Overlap rejects the combination. A nonempty voted passage must equal at least one complete validated candidate; otherwise it requires review, because partial grammatical dependencies cannot be safely inferred. The final passage passes the complete validator again. This is deliberately more conservative than unrestricted paper-style edit recombination.

Sixteen inputs have distinct raw sampled outputs; fourteen have distinct extracted outputs. Some diversity is only markup/commentary, and four inputs have identical raw samples. Correlation remains; identical outputs give no confidence bonus. All 100 generations reached natural EOG; no truncation occurred in this actual panel. Truncation, rejection, conflict, duplicate IDs, minority repairs/negation, protected identifiers, dependency mosaics and identical typos are covered deterministically.

Voting removes sampled corruption, the unnecessary British-text `also`, and a sampled invitation `verstuurt → uitgezonden` drift. But versus greedy, it loses the Dutch `controleerd → controleert` repair and adds an English clean-text rejection. **Five samples are deferred:** the predeclared trigger required a gain or unnecessary-change reduction versus greedy, which was not observed. Improvements over the deliberately stochastic control alone do not satisfy that trigger. No superiority or published-method reproduction is claimed.

## F. Combined methods

Post-diagnostics were applied to every generated candidate and voted result without additional inference. They catch corrupt words that sampling can produce, while voting removes some unshared recognized-word rewrites that dictionaries cannot detect. This is complementary diagnostic behavior, not a demonstrated combined repair-quality gain. Pre-hint voting is deferred: hints have mixed language effects and voting offers no measured gain over greedy here. No combinatorial model/prompt/threshold search was run.

## G. Language-specific coverage and uncertainty

| Method | NL assessable literal clean / all 4 | EN assessable literal clean / all 4 | NL rejected / 10 | EN rejected / 10 |
|---|---:|---:|---:|---:|
| Greedy | 3/4 | 3/4 | 1 | 2 |
| Hints | 3/4 | 3/4 | 1 | 2 |
| Single stochastic control | 2/4 | 0/4 | 3 | 4 |
| Three-sample voting | 3/4 | 2/4 | 1 | 3 |

No NeedsReview outcome occurred in the frozen actual run. Rejected passages are not credited as assessable clean successes. The underlying evaluator's literal no-op metric can count rejected empty predictions; retain its original JSON, but use the explicitly disposition-filtered counts above for delivery. Unknowns and coverage remain visible.

AI local repair labels are retained per annotation in [84 AI error decisions](../benchmarks/phase3/v1/ai-error-decisions.jsonl). Full-passage/source/raw/rejection decisions use the existing ReviewDecision schema: [119 preparer decisions](../benchmarks/phase3/v1/preparer-ai-decisions.jsonl), [evaluation metadata](../benchmarks/phase3/v1/ai-evaluation-metadata.json). They are same-session, exposed, non-independent AI judgments. The exact model revision of the evaluator is unavailable, so fresh AI judgments are not guaranteed reproducible; literal decisions and known/unknown metadata are preserved. A separate ChatGPT can independently assess the blinded packet, but still supplies AI evidence. The existing import records all decisions while retaining **zero human approvals** and unknown human metrics. Proposal-level precision, human semantic preservation, generalizable recall and final held-out quality remain unestablished. V3.1 supplies descriptive Wilson intervals for reference counts; tiny exposed and grouped observations do not support population or paired-superiority claims.

## H. Model-specific results

Only the already available Ministral 3B checkpoint above received new linguistic-panel inference. No 14B/EuroLLM rerun was justified by the small mixed/negative technique results. Historical Phase 2 comparisons remain unchanged and are not new Phase 3 evidence. No language-specific model selection or mandatory larger model is adopted; an untested newer EuroLLM release is not substituted for the historical checkpoint.

## I. Safety findings

The stochastic Dutch diacritics sample inserts Armenian characters (`deոց`), corrupts `reünie`, and changes wording while the existing production validator accepts the extracted passage. Other samples produce unrecognized Dutch words and a recognized but unjustified `verzorgt`/`uitgezonden` substitution. These demonstrate limits of existing semantic safeguards. The AI marks the Armenian corruption as a substantial meaning failure; this is not a human-confirmed critical-event rate. Primary greedy/hints/voting delivered outputs have no preparer-assessed critical failure in this tiny panel; that does not prove absence of risk.

Experimental voting 1.1 excludes newly introduced non-Latin letters absent from the original source, preserving existing international text and Latin diacritics. This is a narrow response to observed evidence, not a production-default promotion or general semantic solution. Original 1.0 outputs remain frozen. Current deterministic replay has zero delivered-analysis or prompt mismatches; no new-generation claim is made.

Identifier/code-symbol rewrites are correctly rejected. `Please explain the procedure.` drops the intended `me` recipient and is appropriately rejected. ASCII-to-curly apostrophe changes in an already correct contraction are also rejected with a misleading name-change reason: the change is cosmetic and unnecessary, not a lost necessary repair. No policy was weakened to increase coverage. False-rejection frequency outside these inspected cases is unknown. Dictionary unknown status never bypasses protected content or forces acceptance/rejection.

## J. Performance and footprint

CPU-only, nine threads, context 4096, 256 output-token cap, 90-second per-generation timeout, no server. **1,736 sampled output tokens** across 100 generations. Input/prefill token counts were not measured. Model load was 1,555 ms; sampled peak process working set was **2.82 GiB**, including dictionaries/model/runtime, sampled every 100 ms. It can miss short peaks and is not a minimum-device RAM requirement.

| Method | NL mean / median ms | EN mean / median ms | NL / EN sampled output tokens |
|---|---:|---:|---:|
| Greedy | 1882 / 1797 | 2906 / 1381 | 202 / 130 |
| Hints | 2253 / 2258 | 2203 / 1623 | 201 / 130 |
| Single stochastic control | 2007 / 1734 | 1581 / 1539 | 233 / 141 |
| Three-sample voting | 5771 / 5524 | 4713 / 4604 | 674 / 399 |

Voting time/tokens include the three reused samples, not extra inference; aggregation time is included. Process CPU milliseconds are retained per generation and include validation/post-diagnostics, not only model compute. `summary-v2.json` clarifies that label; the earlier derived summary is retained. Compilation/probe activity and uncontrolled OS cache affected the run: one English greedy generation took 15.9 seconds. The lower English hint mean is therefore **not evidence of a speed gain**. No cold-start or concurrent-load isolation claim is made.

Dictionary initialization was 192 ms. Process snapshot RSS increased from 74,022,912 to 116,129,792 bytes around initialization; that 42,106,880-byte difference includes runtime activity and is not an isolated heap measurement. The six dictionary/affix files total **3,559,696 bytes**; the NuGet archive is **367,195 bytes**. All acquisition files, including duplicate compressed archives and notices, total 4,341,258 bytes. No production package-size increase is claimed because this dependency is not in the CLI.

## K. Technical validation and reproduction

The final validation record is [validation.json](../benchmarks/phase3/v1/validation.json). Tests cover synthetic dictionary affixes/dialects/loading/disposal, unknown valid terms, complete-word diagnostics, cancellation, Unicode/graphemes, repeated alignment, strict thresholds, identical wrong consensus, dependent edits, protected spans, semantic anchors, and introduced foreign-script letters. Available-weight tests cover EOG, request isolation and cancellation/recovery. No human review, held-out corpus, five-sample inference or foreign-platform real-weight test is claimed.

Local results: **213/213 .NET tests passed, zero failures or skips**, including 28 new linguistic tests and the available real-model integration test; **12/12 Python tooling tests passed**. All 84 historical benchmark hashes and 165 inherited Phase 2 artifacts remain unchanged. Native probes and deterministic 100-output/20-vote replay passed. The Phase 3 verifier freezes 68 evidence files and checks that the public ZIP excludes lineage and preparer decisions.

Native AOT initially failed with LLamaSharp's unrelated TensorSplits JSON converter rooted by the hint serializer context. A direct Utf8JsonWriter fixes this without warning suppression; the CLI and isolated probe publish with warnings as errors. The native probe ran real dictionaries and seeded inference twice, cancellation and recovery, under `sandbox-exec (deny network*)` with nonexistent DOTNET_ROOT. The CLI's existing offline/injection checks also passed. The panel driver's loopback denial preflight established EPERM; no document data was transmitted. Python, SDK and acquisition tooling are developer prerequisites only; no end-user installation or automatic application download was added.

Reproduce locally with the reviewed existing model and an equivalent CPU backend:

```sh
python3 scripts/acquire-linguistic-assets.py /tmp/NEW-linguistic-assets
dotnet build Diorthosis.sln -c Release
python3 scripts/run-linguistic-experiment.py /path/to/existing-reviewed.gguf /tmp/NEW-linguistic-assets /tmp/NEW-run
dotnet src/Diorthosis.Benchmarks/bin/Release/net10.0/Diorthosis.Benchmarks.dll linguistic-replay benchmarks/phase3/v1/panel.jsonl benchmarks/phase3/v1/run /tmp/NEW-replay.json
python3 scripts/build-phase3-review.py benchmarks/phase3/v1/panel.jsonl benchmarks/phase3/v1/run /tmp/NEW-review
python3 scripts/summarize-linguistic-experiment.py benchmarks/phase3/v1 /tmp/NEW-summary.json
python3 scripts/verify-phase3-artifacts.py
```

The inference driver requires macOS network isolation; other hosts need equivalent verified OS denial before claiming an offline run. macOS ARM64 needs the existing pinned Metal-disabled backend described in ADR 0006. A fresh run can differ by hardware/native build; exact retained outputs and decisions remain independently replayable. The [software identity correction](../benchmarks/phase3/v1/run/software-identity-correction.json) and original commit identify the frozen run; fresh inference at current code is a new experiment.

## L. Git and CI

Work is on one dependent branch, `feat/linguistic-quality-experiments`, based on PR #4. Logical commits separate implementation, compatibility/safety fixes and evidence. [Base CI evidence](../benchmarks/phase3/v1/base-ci.json) records the cancelled macOS ARM64 job: no steps and 404 log, so the cause is unknown. The requested rerun completed tests and all six publish/native-start jobs successfully. That success belongs to the old base commit.

The new matrix retains all six CLI targets and explicitly acquires the small hash-pinned developer assets to publish/run model-free Native AOT dictionary/edit probes on every target. This is development CI, not product downloading or foreign-platform real-model validation. New-branch CI status and the dependent PR are recorded in `docs/phase3-delivery.md`; do not infer new-head success from the base rerun. No PR is merged, release published, branch force-pushed or protection setting changed.

## M. Scientific interpretation and decisions

Localized source operations and strict validation are technically useful. The observed hint gain supports further Dutch spelling investigation, while the lost English agreement repair contradicts a blanket hybrid-improves-quality claim. Voting suppresses some stochastic over-correction but fails to improve the actual greedy baseline here, and its dependency guard limits recombination. The experiment does not refute the published multilingual method, which uses different prompts, models, extraction and sample counts and did not test Dutch. Larger confirmatory claims remain inconclusive.

| Technique | Decision | Scope and reason |
|---|---|---|
| WeCantSpell.Hunspell | Adopt for isolated developer experiments | Offline .NET 10/AOT execution works; MPL notices/source obligations retained; no product bundling approval |
| OpenTaal Dutch dictionary | Adopt for isolated developer experiments | Matching pinned files and useful observed morphology/typo coverage; not grammar authority |
| English dictionary | Keep experimental | US/GB policy works, but size-60 coverage and technical/uncommon-word gaps remain |
| Pre-inference hints | Keep experimental | One Dutch gain, one English loss; no universal benefit |
| Post-inference diagnostics | Keep experimental | Finds corrupt replacements; unknown vocabulary and valid-word drift prevent automatic gating |
| GECToR-inspired representation | Adopt as experimental source/audit representation | Deterministic local operations and dependency/ambiguity guards; no trained GECToR claim |
| Three-generation voting | Keep experimental | Worse NL repair than greedy, extra EN rejection and cost; useful negative result |
| Five-generation voting | Defer | Predeclared greedy-benefit trigger not met |
| Combined hybrid/voting | Defer pre-hint combination | Post-diagnostic complement observed, combined quality benefit unproved |
| Dutch model selection | Defer | Development/AI evidence only; no reviewed held-out comparison |
| English model selection | Defer | Same limitation; no larger-model requirement justified |

## N. Next development feedback cycle

1. Give only the blinded ZIP to a fresh ChatGPT session, retain its exact AI response/model/version/prompt context, validate bindings with the existing importer and compare disagreements with the preparer decisions. AI feedback remains the primary development loop; qualified independent humans are still required for human validation and quality-gate claims.
2. Investigate targeted Dutch spelling assistance without supplying misleading technical-identifier suggestions; retain the English agreement-loss case as a paired regression. Add genuinely new varied cases only in a new development panel/version. Do not alter the frozen references to improve these scores.
3. Examine accepted recognized-word semantic drift, corrupted diacritics, literal single-star emphasis and misleading apostrophe rejection diagnostics. Any production safety/prompt change needs its own tested review decision. A majority or dictionary alone cannot establish meaning preservation.
4. Revisit voting only with a stronger independently reviewed source-alignment/dependency method and a reason to expect a gain over greedy. Avoid expensive five-sample/larger-model expansion until the smaller comparison justifies it. Document processing remains a separate future milestone.
