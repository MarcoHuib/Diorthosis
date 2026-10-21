# Phase 4: evidence-based prompt quality optimization

Research checkpoint, 2026-10-21. **Retain production `diorthosis-correction-v3`. No prompt is promoted and no PR is merged.** Literal-Unicode JSON is a promising experimental substrate; it is not an independently validated winner. All new correction generations use the existing local Ministral checkpoint. Independent ChatGPT assessment is pending; human approvals remain zero.

## Identity and controlled design

The Phase 3 base is verified PR #5 head `6f78219ef57332b88c047cac51338b8604adadb3`, with all seven prior CI jobs passing. Phase 4 is [PR #7](https://github.com/MarcoHuib/Diorthosis/pull/7), branch `feat/phase4-prompt-quality`, targeting the Phase 3 branch. The protocol, panel and prompt implementations were frozen at `cf8fdfe` before inference. User checkpoint commits `53be095` and `de0742d` preserve the initial experiment and tooling. They do not change the prompt or generation implementation.

Model: official `mistralai/Ministral-3-3B-Instruct-2512-GGUF`, revision `be4e90342e4468f37d4bea40ec143997f9256475`, Q4_K_M, 2,147,023,008 bytes, SHA-256 `9ed150d4367e68df0ac8e1540f6ddc65b42d0ee26378329d1ecbca60f93fc5f8`. Existing European-origin/license review is retained in [model validation](model-validation.md); no new model, tokenizer, dependency or weight download was introduced. Official upstream checkpoint status is not Diorthosis production-model approval.

Runtime: LLamaSharp/CPU backend 0.27.0, pinned llama.cpp `3f7c29d318e317b63f54c558bc69803963d7d88c`, existing CPU backend and exact native hashes retained per run. Hardware: Apple M4, macOS ARM64, 34,359,738,368 bytes physical memory, .NET 10.0.12. Fixed settings: context 4096, nine CPU threads, zero GPU layers, greedy sampling, 256 maximum output tokens, 90-second timeout, natural sampled EOG required. Calls are stateless, case-major, with arm order rotated by case index. There is no voting or sampling expansion.

Every generation retains source/language, version, literal prompt/hash, model/runtime/software binding, prompt-content and sampled-output token counts, termination, raw output, extracted correction, disposition, delivered text, rejection reason, latency and sampled RSS. Run manifests bind the common model/native identities; failures are retained rather than dropped. The inference process runs under `sandbox-exec (deny network*)`; a loopback preflight establishes EPERM. No service or external inference API is used.

The same three loaded assembly hashes are retained through the paired generation stages. Their informational version still names `6f78219`, because the experimental source was compiled before its freeze commit. This is metadata, not evidence that old production source generated the experiment. Start-of-run assembly hashes and complete source hashes distinguish those facts. The experimental source is unchanged from the freeze; later analysis/replay code is identified separately. Do not infer a new generation from deterministic replay.

## Frozen panel, authority and exposure

There are 100 original fictional passages per language: ten prompt-development, seventy selection and twenty reserved confirmation. Each language has forty proposed clean passages, distributed 4/28/8 across those splits. Selection contains 42 erroneous passages and 43 annotated errors per language, including one two-error passage. The four few-shot examples per language use only development items 1, 2, 7 and 8. Neither selection nor confirmation material appears in examples.

[Panel identity](../benchmarks/phase4/v1/freeze.json) binds exact texts, labels, acceptable alternatives, authoring source and overlap screening before inference. No exact old fixture matches or internal character-similarity pairs above the declared screening threshold were found. This does not establish independent linguistic origins. The author knows Phase 3 failure categories; all fixtures and labels are AI-authored/preparer-inspected, with zero independently AI-reviewed or human-validated labels. Regional/informal wording, technical terms, names, contractions, multilingual passages, paragraph breaks, NFC/NFD accents, emoji and control characters are included. Most errors are local; long realistic multi-error documents are underrepresented.

The confirmation panel is inference-unseen until one locked comparison. Its author/preparer has seen its text, and it is committed in a public repository. It is not an independently sealed or author-unseen population test. Confirmation does not repair those limitations.

[Errata](../benchmarks/phase4/v1/panel-errata.json) records two selection fixtures, `p4-nl-selection-066` and `p4-en-selection-065`: authoring assembly interpreted backslash-t/backslash-n inside intended Windows paths as control characters. Their frozen texts, labels and every unsuccessful case remain unchanged. Original-label mechanical scores include them with this caveat; they cannot support clean, valid Windows-path claims. Linguistic assessment may dispute their labels. They are neither silently excluded nor retrospectively relabeled. A future dataset version must correct the authoring issue before fresh evaluation.

[Public-source investigation](phase4-dataset-research.md) examines BLiMP-NL, Dutch L1/L2 writing products and the GitHub Typo Corpus. BLiMP-NL is a useful syntax diagnostic, not a proofreading benchmark; immutable provenance and applicable redistribution notices still need checking before import. No external corpus passages were imported and no external human labels were transferred to these fixtures.

## Representation comparison

A is exact production v3 with `JsonEncodedText.Encode`; B retains byte-identical correction instructions and JSON framing while using literal-Unicode JSON serialization; C preserves the correction instructions but supplies a delimited UTF-8 block with an explicit untrusted-data boundary. Tests establish A parity, JSON roundtrip and exact C payload/boundaries. No Unicode normalization is applied to inputs.

B's relaxed JSON encoder still escapes supplementary-plane emoji as surrogate escape pairs in this run; it does not make every Unicode scalar literal. It also preserves safe ASCII punctuation such as apostrophes/backticks and changes the spelling of quote escapes, while still correctly escaping JSON controls, quotes and backslashes. Therefore A/B differences are effects of this serialization treatment, not proof that escaped diacritics alone caused failures. C necessarily adds boundary instructions and longer framing; a C result cannot be attributed purely to plain Unicode. A delimiter does not prove that a language model will obey the trust boundary. Embedded-command fixtures and all existing validators remain separate evidence.

All 420 paired selection generations completed with natural EOG and no generation failure. The original [run](../benchmarks/phase4/v1/representation/) and [initial analysis](../benchmarks/phase4/v1/representation-analysis/summary.json) are retained. The instruction experiment reuses B's exact original-instruction outputs as its control, with lineage; these are not new generations or independent observations.

| Representation | NL reference repairs / 43 | EN reference repairs / 43 | NL eligible clean literal / 28 | EN eligible clean literal / 28 | NL eligible reference-complete / 70 | EN eligible reference-complete / 70 |
|---|---:|---:|---:|---:|---:|---:|
| A: production escaped JSON | 26 | 29 | 13 | 13 | 38 | 41 |
| B: literal JSON | 28 | 32 | 16 | 18 | 43 | 48 |
| C: delimited plain text | 28 | 29 | 12 | 14 | 37 | 41 |

These are original-label reference diagnostics, not linguistic accuracy. The denominator includes every attempted passage. Rejected/review-required/suppressed outputs are abstentions and earn no delivered repair. Literal clean preservation excludes abstention even when source fallback is safe; fallback-inclusive preservation is reported separately in machine-readable summaries. A reference-complete output can still require independent semantic review. An unlisted valid alternative can be under-credited.

B's concrete gains include preserving `retry_count` while repairing `wachtworod`, repairing plural agreement beside `/var/cache`, retaining correct `conciërge`, preserving multilingual Greek, and keeping `naïve_mode` intact. A removes the trema from `conciërge`; B retains it. This is a real example, not a general diacritic-corruption rate. All arms unnecessarily normalize some decomposed accents; canonical equivalence is distinguished from malformed spelling. B does not solve the accepted `hesje → hemdje`, `goesting → geest`, deleted volition in `waarom ik ... doe`, or French translation failures. C adds further damage, including `pumps → pump`, `behoort behoorlijk` and Greek `καλημέρα → kaalméra`.

[Substrate decision](../benchmarks/phase4/v1/substrate-decision.json): use B only for further controlled prompt research, consistent with the user continuation. Retain A in production. All three instruction interventions were implemented before representation results, with no case-specific tuning.

## Isolated instruction comparisons

The original B outputs are the zero-shot control. Each isolated intervention adds only its declared instructions or four development examples; all remaining input framing, model, decoding and validation stay fixed. The 420 new generations completed with no failures and natural EOG. [Full selection analysis](../benchmarks/phase4/v1/selection-analysis/summary.json) compares both production A and original B; use B for the causal instruction comparison.

| Literal-JSON instructions | NL reference repairs / 43 | EN reference repairs / 43 | NL eligible clean literal / 28 | EN eligible clean literal / 28 | NL / EN accepted changes to proposed clean text | NL / EN abstentions out of 70 |
|---|---:|---:|---:|---:|---:|---:|
| Original control | 28 | 32 | 16 | 18 | 3 / 3 | 12 / 13 |
| Added minimal-edit policy | 30 | 32 | 16 | 14 | 5 / 5 | 10 / 15 |
| Added language checklist | 34 | 31 | 15 | 15 | 4 / 4 | 11 / 16 |
| Four development-only examples | 29 | 32 | 18 | 17 | 2 / 2 | 12 / 15 |

Paired reference-error gains/losses versus original B are NL minimal 2/0, checklist 6/0, few-shot 2/1; EN minimal 0/0, checklist 0/1, few-shot 1/1. Complete-reference passage gains/losses are NL 1/0, 6/2, 4/1 and EN 0/4, 0/4, 2/3 respectively. These are paired source-case observations, not independent output-subject trials.

The checklist repairs `verhuisd → verhuist`, `geverft → geverfd`, `ruine → ruïne`, `grote probleem → groot probleem`, the compound `koffieapparaat`, and the Dutch invitation typo. The last repair also translates `bienvenue → welkom`, so local recall credit does not make its whole passage acceptable. It changes `hesje → heksje` and English plural `pumps → pump`, both accepted. It also rewrites a clean location constraint, `leave the original file where it is`, as `leave ... as it is`, which changes location preservation into state/content preservation.

Minimal-edit instructions do not reduce unnecessary changes here: accepted modifications to proposed clean text rise from 3 to 5 in each language, and English eligible clean preservation falls. Most seriously, informal `goesting in frieten` becomes `een gevoel van misselijkheid bij frieten`: appetite becomes nausea. It also produces the file-location rewrite and a new `onontvangen` replacement. Explicit preservation instructions are not a semantic safeguard.

Few-shot repairs `geverft` and `onmiddelijk`, but drops `moet` from a Dutch word-order case and the validator withholds it. It loses English agreement in `The visitor were waiting ...`, where original B delivered `was waiting`. Its apparent English reference gain is the ASCII version of `We didn't know ...`; original B's curly-apostrophe repair was already linguistically valid. Thus the matched-reference gain is not a new linguistic repair. Few-shot preserves more Dutch clean passages but fewer English ones, and still introduces `onontvangen`, misses the coordinator's trema and changes `hesje → hemdje`. `goesting` becomes the incorrect collocation `zin op frieten`.

All three added-instruction arms introduce `onontvangen` from `onvangen`; original B leaves the original typo. This adds malformed/nonreceipt wording to an acknowledgment. The exposed preparer treats that as meaning damage, not a successful spelling repair. No dictionary, blacklist, voting rule or new acceptance heuristic suppresses it.

**Combined minimal+few-shot was not run.** The frozen trigger required promising isolated repair, clean-preservation and safety evidence. Minimal's accepted nausea/location changes and the mixed few-shot results fail that trigger. No longer prompt is assumed better.

## Locked one-pass confirmation

[Candidate lock](../benchmarks/phase4/v1/candidate-lock.json) was written at `2026-10-21T11:53:38.249931+00:00`, before the confirmation process started. It selects **literal JSON with original instructions**, not an instruction intervention. Both-language selection gains and concrete Unicode/identifier preservation justify one exploratory check; unresolved safety failures prevent promotion. The lock binds panel, source, selection evidence and prompt implementation hashes. No prompt, example or reference was changed after the lock. The panel received one paired run, 80 generations, no failures, all EOG. It is now exposed and must not be reused as unseen data for future tuning.

| Language / condition | Reference repairs / 12 | Exposed preparer-AI repairs / 12 | Exposed AI eligible complete passages / 20 | Eligible clean literal / 8 | Abstentions / 20 |
|---|---:|---:|---:|---:|---:|
| NL production A | 7 | 7 | 11 | 4 | 4 |
| NL locked B | 9 | 9 | 14 | 5 | 2 |
| EN production A | 8 | 9 | 12 | 3 | 7 |
| EN locked B | 11 | 11 | 15 | 4 | 4 |

The preparer read all extracted/delivered confirmation pairs once after inference and retained **154 bound AI decisions and 65 supplementary error decisions**, validated/imported with zero human finalization. [AI summary](../benchmarks/phase4/v1/confirmation-preparer-ai/summary.json), [provenance](../benchmarks/phase4/v1/confirmation-preparer-ai/provenance.json) and [import](../benchmarks/phase4/v1/confirmation-preparer-import/validation.json) disclose full project/condition exposure, exact-build unavailability, partial overall packet coverage and no raw-response assessment. This is complete preparer coverage of the confirmation extracted/delivered pairs, **not a blind or independent ChatGPT review**. The helper verifies literal inspected bindings and projects exact contextual reuse; it does not turn reference matches into independent linguistic judgments. Greek orthographic uncertainty is retained.

A's `We haven’t seen that warning before.` is a valid past-participle repair with a curly apostrophe, explaining the difference from the frozen reference count. B's English advantage therefore comprises two additional delivered linguistic repairs in this preparer reading, rather than the three additional reference credits. Both are diagnostic AI counts, not human-validated recall.

Concrete B gains are retaining `trace_id` while repairing `verzondenne`, preserving the umbrella emoji while repairing `totdatd`, retaining `client_secret`/`public_key` on clean Dutch, retaining `session_token` while fixing English agreement, and repairing `exhibiton` without changing the name `Chloë`. B also preserves `Curaçao` where A corrupts it and is rejected. The Dutch reference-passage comparison has four gains and one loss: B unnecessarily spaces `'s Ochtends` and is suppressed, while A leaves it unchanged. This loss is not hidden by source fallback.

**Critical unresolved confirmation failure:** correct `hüttenkäse` becomes `huitkase` under A and `hijttenkäse` under B; both are accepted. The preparer labels this a critical lexical/meaning failure in the dietary sentence. There is one such accepted source case per arm, not two independent cases or a general failure rate. No additional critical regression versus A was identified in this exposed confirmation reading, which is insufficient to establish safety.

For B, three Dutch original errors remain: subordinate-clause order (`Ze vraagt of hij kan ...`), missing trema in `naief`, and singular `inhoud ... zijn` agreement. Its remaining English original error is withheld: both arms produce an otherwise valid `We'll meet outside the café tomorrow morning.` repair, but the validator rejects it. English otherwise-valid necessary candidates withheld: one per arm out of twenty attempts. Valid local repairs also occur inside appropriately rejected identifier/name/emoji-damaging candidates; those are not valid whole-passage repairs or delivery successes.

## Safety, alternatives and uncertainty

The [33 inspected selection source/candidate pairs](../benchmarks/phase4/v1/preparer-inspection/preparer-observations.jsonl) retain literal sources, candidate hashes, full condition/run/trace membership and delivery eligibility. They are partial exposed AI evidence, not an overall precision estimate. They cover the accepted vest/shirt/witch changes, appetite/nausea drift, deleted volition, translated multilingual content, NFD normalization, phrase emphasis, valid alternatives and lost repairs. Exact contextual reuse across arms is not a fresh reviewer or an independent source observation.

Reference matching undercredits valid curly contractions and some possible Dutch alternatives. `koffie-apparaat` is a plausible facultative-hyphen alternative but is withheld; qualified review remains necessary rather than silently rewriting its reference. `heeft verleend` is grammatical, but changes the aspect of the intended present-tense condition and remains semantically disputed. Correct `betaald heeft → heeft betaald` is an unnecessary valid variant, not a repair. `γεια → γειά` in confirmation is an unnecessary Greek accent change whose orthographic validity was left unknown; it is not automatically labeled corruption. Canonically equivalent accents, malformed words, changed names, changed objects and added formatting remain distinct categories.

Selection source-case paired reference-passage gains/losses for B versus A are NL 5/0, EN 8/1; confirmation NL 4/1, EN 4/0. Unadjusted exact discordance p-values are 0.0625/0.0391 on selection and 0.375/0.125 on confirmation. The descriptive paired-bootstrap intervals for confirmation passage-score differences are NL -0.05 to 0.35 and EN 0.05 to 0.40. These are **reference-proxy** intervals on tiny synthetic author-exposed samples, with candidate selection, multiple comparisons, uncertain labels and nonrepresentative prevalence. They do not establish population-level superiority or linguistic quality percentages. In particular, 11/12 English reference repairs does not establish the 90% held-out acceptance target.

Meaning preservation, introduced errors, unjustified changes, valid alternatives and full-passage quality cannot be inferred solely from matched references. Selection has partial preparer coverage; confirmation has exposed preparer coverage; independent AI and qualified human judgments remain unavailable. Unknowns are not converted into successes. Abstention remains separate from repair even when delivered source text is safe. Neither majority agreement nor a dictionary was used to select or accept edits.

## Measured runtime cost

Token counts are measured with the checkpoint vocabulary. Prompt counts exclude the chat template and BOS; sampled-output counts include a sampled EOG token. The 920 quality generations sampled **14,569 output tokens**. Process RSS is sampled every 50 ms and includes resident model, runtime and caches; it is not an isolated allocation or minimum-device RAM measurement. Peak across these runs was **2,983,297,024 bytes (2.78 GiB)**. Model load time is retained separately per run.

| Selection condition | NL / EN mean prompt-content tokens | NL / EN median latency ms | NL / EN mean latency ms |
|---|---:|---:|---:|
| Production A | 122.1 / 118.6 | 1530 / 1342 | 1565 / 1440 |
| Original B | 119.0 / 115.1 | 1483 / 1340 | 1527 / 1450 |
| Plain C | 188.3 / 184.0 | 1781 / 1587 | 1810 / 1732 |
| B + minimal | 188.0 / 184.1 | 1689 / 1580 | 1770 / 1662 |
| B + language checklist | 176.0 / 158.1 | 1698 / 1497 | 1767 / 1540 |
| B + few-shot | 217.0 / 190.1 | 1838 / 1585 | 1887 / 1638 |

Few-shot adds about 82% Dutch and 65% English prompt-content tokens, with observed median latency increases of about 24% and 18% over original B. Mixed repair/preservation results and new damage do not justify that cost as a general improvement. Small A/B latency differences do not establish a speed gain: warm file cache, thermal state and background load were uncontrolled, and original B's control is reused across separate stage processes. No distribution/package-size or minimum-RAM claim follows from sampled RSS. Foreign-platform real-weight latency and memory remain unmeasured.

## Ranked research recommendation and production decision

| Research rank | Configuration | Recommendation and limits |
|---|---|---|
| 1 | Original instructions, literal JSON | Advance the locked configuration to external review. Both-language delivered gains and stronger identifier/diacritic preservation, with accepted corrupted words and semantic substitutions still unresolved. Promising substrate, not an independently validated winner. |
| 2 | Production v3 escaped JSON | Retain the production default and comparison control. Its known failures remain; this is not release approval. |
| 3, tied | Few-shot on literal JSON | Better Dutch clean preservation, but English agreement loss, new malformed wording and greater cost. No blanket advantage. |
| 3, tied | Language checklist on literal JSON | Most Dutch reference repairs, but English repair/preservation losses and material referent changes. The evidence cannot reliably rank its linguistic quality against few-shot. |
| 5 | Plain text | No demonstrated advantage over literal JSON; longer framing and accepted multilingual/plural/clean-text damage. |
| 6 | Added minimal-edit policy | Negative evidence: higher Dutch reference recall does not outweigh accepted appetite-to-nausea/location changes and increased unnecessary clean changes. |

**Decision: retain production prompt v3; do not propose replacing it on this evidence.** No combined arm, promotion, model change, fine-tuning, production validator change, merge or release is authorized by these results. The external-review checkpoint is the stopping point. A future promotion requires separately approved reproducible delivered-quality evidence, no critical semantic regression, acceptable clean preservation and resources, regression tests and passing current-head cross-platform/AOT CI. Independent assessment may reverse provisional rankings or dispute source labels; retain those disagreements without modifying frozen evidence.

## External review package

[Complete blinded ChatGPT review ZIP](../benchmarks/phase4/v1/review/chatgpt-review.zip): **180 sources, 1,075 bound subjects**, comprising 180 annotation, 287 raw, 275 parsed, 247 delivered and 86 rejection subjects. They represent **920 generation occurrences**, not 1,075 independent trials. The 20 development examples are excluded from inference/review. The ZIP contains only literal subjects, escaped offline HTML, exact import/provenance instructions and a public member-hash manifest. It excludes model/condition identities, source-ID lineage, research reports and preparer judgments. Prior exposure and absolute-blinding limits must be declared.

[Optional batch instructions](../benchmarks/phase4/v1/review/BATCHES.md) point to eighteen ZIPs of at most ten source cases each. Batch subjects preserve exact parent bindings. Import against the corresponding batch directory; complete batch coverage does not imply complete experiment coverage or a new reviewer identity. Main and batch ZIP members are checked against their hashes and full union coverage.

Keep `lineage-DO-NOT-SEND-TO-REVIEWERS.json`, `preparer-inspection`, `confirmation-preparer-ai`, imported preparer judgments and this report away from external reviewers until decisions are locked. No automatic external upload was performed. All reviewer decisions must remain AI-labeled, with actual model/build/context or explicit unavailability. Main imports use existing C# adjudication; supplementary per-error bindings/indices/types/duplicates are preflighted. AI evidence never authorizes a human-finalized metric.

## Verification and reproduction

Local results: **233/233 .NET tests**, no failures/skips, including the existing available-weight integration test; **32/32 Python tooling tests**; **920/920 deterministic prompt/termination/extraction/validation/delivery replays**, zero mismatches. macOS ARM64 Native AOT CLI and experimental probe published with no new warning suppression. Network-denied native checks run with nonexistent `DOTNET_ROOT`, cover the new Unicode/framing/few-shot contracts, existing dictionaries and CPU backend, and execute three separate baseline CLI smoke generations. Those smoke/integration calls are validation only, not extra quality-panel observations.

The restricted first AOT publish stalled before native compilation and was stopped; the retry with build-tool permissions succeeded. The initial restricted MSBuild/VSTest IPC attempts and hardware-inspection launch also failed before the relevant tests/inference, and the successful retries are distinguished. Enforced network denial applies to actual inference/native validation, independent of build-tool permission. No unsuccessful quality generation is omitted; there were zero quality generation failures. [Local validation record](../benchmarks/phase4/v1/validation.json) retains logs, hashes and counts.

All **84 historical, 165 Phase 2, 68 Phase 3 and 47 external-GPT-6 evidence files** remain unchanged. Production v3 is byte-identical. The CI additions verify Phase 4 artifact hashes, replay all three runs, strictly reimport the exposed AI decisions, run the tooling tests, and retain all six Windows/macOS/Linux x64/ARM64 publish/AOT probe jobs. CI has no model download or foreign-platform real-weight inference. Current-head CI status and its exact commit/run are pinned in [PR #7](https://github.com/MarcoHuib/Diorthosis/pull/7); a passing old base is not new-head evidence. Nothing is merged automatically.

Reproduce retained results after a Release build, always using new output directories:

```sh
python3 scripts/verify-prompt-quality-artifacts.py
python3 -m unittest discover -s tests/tooling -p 'test_*.py'
dotnet src/Diorthosis.Benchmarks/bin/Release/net10.0/Diorthosis.Benchmarks.dll prompt-quality-replay benchmarks/phase4/v1/selection.jsonl benchmarks/phase4/v1/representation /tmp/NEW-representation-replay.json
dotnet src/Diorthosis.Benchmarks/bin/Release/net10.0/Diorthosis.Benchmarks.dll prompt-quality-replay benchmarks/phase4/v1/selection.jsonl benchmarks/phase4/v1/isolated /tmp/NEW-isolated-replay.json
dotnet src/Diorthosis.Benchmarks/bin/Release/net10.0/Diorthosis.Benchmarks.dll prompt-quality-replay benchmarks/phase4/v1/confirmation.jsonl benchmarks/phase4/v1/confirmation-run /tmp/NEW-confirmation-replay.json
python3 scripts/analyze-prompt-quality.py benchmarks/phase4/v1/selection.jsonl /tmp/NEW-selection-analysis benchmarks/phase4/v1/representation benchmarks/phase4/v1/isolated
python3 scripts/build-prompt-quality-review.py benchmarks/phase4/v1/review-spec.json /tmp/NEW-blind-review
```

To rerun **selection** inference as a new experiment, use the reviewed existing GGUF and exact CPU backend, build from the frozen implementation or reproduce its behavior, then invoke `run-prompt-quality.py` with the declared arms and new output paths. Its macOS driver enforces network denial; other hosts need equivalently verified isolation. Do not rerun confirmation to tune or select another prompt. Archived initial analysis 1.0 is retained; final comparison 1.1 adds direct original-B control comparisons and records its own script identity. Native hashes and assembly/source identities are separate from later analysis/replay builds. Fresh hardware/software runs may differ; do not rewrite original observations to match them.

External-assessment import:

```sh
python3 scripts/import-prompt-quality-review.py benchmarks/phase4/v1/review /path/decisions.jsonl /path/provenance.json /tmp/NEW-external-review --error-decisions /path/error-decisions.jsonl
```

The optional per-error file requires exact bound indices and does not turn a local repair inside a rejected candidate into a delivered repair. Retain original external bytes, context and disagreements. Complete the blinded assessment before deciding any next prompt or constructing a genuinely new independent confirmation panel.


## External GPT-6 reviews: 2026-10-21 preflight supplement

[External review preflight, linguistic-quality findings and limitations](phase4-external-gpt6-review-preflight.md) now records the eighteen ChatGPT ZIP assessments. **1,075 of 1,075 submitted AI decisions match the frozen batch bindings at preflight**, but this does **not** mean they have been officially imported: **zero of eighteen strict .NET imports were executed** in the available environment. Six of 484 supplementary error decisions in batch 07 improperly reference source-only `kind=case` subjects; a derivative excludes them for future strict import, retaining all original submitted bytes. 478 supplementary records bind to actual candidate annotations.

On the AI-reviewed selection panel, literal JSON B shows NL/EN 30/33 delivered annotated repairs versus production v3 A 28/30, with 45/50 versus 40/43 fully acceptable eligible delivered passages. On the separate confirmation panel B shows NL/EN 9/11 delivered repairs versus A 7/9, with 14/15 versus 11/12 acceptable delivered passages. These are descriptive external-AI development evidence, not independently human-adjudicated accuracy or gross paired gains/losses. Critical meaning corruptions persist in accepted outputs, including `hüttenkäse` damage under both A and B, `hesje` substitutions, volition deletion and technical/foreign-language changes. **Production v3 stays unchanged; no experimental arm is production-ready.** Prompt L increases Dutch local repairs but has unacceptable safety regressions. Follow-up official import, condition-paired diagnostics, full disagreement sensitivity and affected current-head .NET/CI tests remain outstanding. Do not merge PR #7 automatically.
