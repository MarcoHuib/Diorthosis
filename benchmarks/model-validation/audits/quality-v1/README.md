# Benchmark quality audit v1 — proposed only

Prepared 2026-10-20 against commit `bd28ef5e9d584e849edb39d8566194a231d68f58`, branch `feat/local-ai-benchmarks`, PR #2. This is an AI-authored audit, not independent human validation. No benchmark revision, evaluator change or model is adopted.

Start with the [technical and linguistic report](../../../../docs/benchmark-quality-audit-v1.md) and [all 44 case reviews / 132 output adjudications](case-review.md).

## Contents

- `reference-notes.json`: reference-first linguistic decisions, regional/context qualifications and rationale.
- `reviewed-cases.jsonl`: all 44 original cases, proposed versions, judgments and human-review flags.
- `smoke-44-proposed-v3.jsonl`: **unapproved** separate dataset proposal, accepted by the current evaluator. “v3” refers to annotations, not the frozen v3 prompt or historical v3 inference run.
- `blind-packet.jsonl`, `blind-key.json`, `adjudication-notes.json`: shuffled assessment packet, disclosure key and decisions written before unmasking. Key is intentionally published for auditability, not concealment from subsequent readers.
- `output-adjudications.jsonl`: all 132 responses, including raw, extracted and accepted text, layer-specific labels/reasons, rejected outputs and actual edit hunks.
- `sensitivity.json`: original and provisional language/pooled metrics, before/after case deltas, raw-candidate versus accepted metrics, and explicitly AI-adjudicated sentence counts.
- `Probe/`: analysis-only driver that compiles unchanged source files by reference. It neither references a native inference runtime nor instantiates an adapter. Its observations characterize current defects; they are not assertions that those behaviors are desirable.
- `probe-observations.json`: deterministic scoring and safety observations.
- `source-manifest.json`: frozen hashes of the original dataset, top-level v4 inputs, evaluator, prompt and reference-first notes.
- `artifact-hashes.json`: source and analysis artifact hashes. Build outputs are ignored by Git.

## Reproduce without inference or downloads

Use the already-installed repository .NET SDK and Python 3 (analysis tooling only, not product dependencies):

```sh
python3 benchmarks/model-validation/audits/quality-v1/reproduce.py
```

The driver has no package dependencies; `Probe/NuGet.Config` clears package feeds. The script verifies the frozen inputs, constructs the separate proposal, builds linked unchanged evaluator/parser sources, replays all 132 saved generations through the parser, evaluates the existing saved predictions against both annotations, verifies exact historical metric agreement, and regenerates analysis files. It never calls `run`, loads weights or performs inference. It uses temporary files for intermediate scoring, not historical result paths. The published blind packet is verified against the saved traces; seed `20261020` shuffled rows originally collected from sorted top-level `*-trace.jsonl` files.

Parser replay is a verification step only: sensitivity scores use the saved edit lists, not regenerated edits. `rawCandidateEdits` means the trace's parser-derived raw edit list, which can omit parts of the literal model response; it is not a direct measure of raw-response correctness. Read the literal generations as well.

## Exact annotation changelog: reviewed v2 → proposed v3

All 44 case IDs, input strings, language values, annotation spans, error counts, categories, old alternatives and order are retained. No clean case is relabeled. The only semantic JSON differences are these **five appended alternatives in four cases**:

| Case | Existing replacement retained | Appended replacement(s) | Reason |
| --- | --- | --- | --- |
| `nl-12` | `De taak werd onderbroken omdat de server opnieuw opstartte.` | `Omdat de server opnieuw opstartte, werd de taak onderbroken.` | Same causal relationship and tense; fronted subordinate clause requires main-clause inversion. Preserves source clause order. |
| `en-04` | `I will send the package tomorrow.` | `Tomorrow I will send the package.`; `Tomorrow, I will send the package.` | Repairs verb-object order while preserving the fronted temporal adjunct. Introductory comma is optional here. |
| `en-05` | `Wait, let's` | `Wait, let’s` | U+0027 and U+2019 are valid apostrophe representations in this contraction. |
| `en-09` | `Wait, let's` | `Wait, let’s` | Identical orthographic policy to en-05. |

The JSONL serialization also changes nonsemantic whitespace: all lines are compact UTF-8 with a terminal newline. Source hash: `2190c109f5f8e8ce1979637a1beea9fcce615959033aa76576b32ba537465fa2`. Proposal hash: `5b1810b0dfff1156facba81c4a9d0e72487e9652c11c637badd9bca7488c9e55`.

The proposal deliberately preserves historical enum casing (`Dutch`, `Grammar`, etc.). The C# reader accepts it; the published JSON schema lists lowercase/camelCase enums and therefore does not literally validate either the historical v2 file or this proposal. This pre-existing schema/serialization mismatch needs a separate approved compatibility decision, not a hidden casing migration in this audit.

`en-01` past-perfect equivalence is **not** added. Context-sensitive punctuation, register, tense and information-structure alternatives remain documented for human review. These five additions are a conservative sensitivity scenario, not a claim that all possible acceptable alternatives have been enumerated.

## Review status

All 44 references and all AI judgments need independent bilingual human sign-off. Priority cases include the four changed references, `en-01`, register/context-sensitive `nl-04/05/11/12/15`, `en-03/05/07/09/11/13`, and the meaning/identifier failures documented in the report. No output was labeled correct solely because a model generated it. Multiple identical outputs were judged consistently under the reference-first policy.

No production source, prompt, dependency, source dataset, historical results, PR metadata or repository protection settings were changed. No new inference, model download, fine-tuning, release or merge was performed.
