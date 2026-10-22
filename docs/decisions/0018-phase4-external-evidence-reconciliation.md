# 0018: preserve external AI provenance and separate local repair from passage quality

2026-10-21. Accepted for research analysis only; no production behavior changes.

All eighteen Phase 4 external GPT-6 batches are imported through the existing strict Python preflight and C# adjudicator. Keep each original ZIP/member and provenance separately; reconcile against exact frozen parent subjects and actual generation lineage. A combined diagnostic dataset may concatenate decisions but must not invent one reviewer session, independent reviewers or human adjudications.

Batch 07 contains six source-only supplementary annotations outside the candidate-error contract. Retain their exact records and identify the violated scope. Import a separately named, byte-preserving subset of the 21 candidate records. Reject rebinding them, rewriting judgments or changing a declared hash to authenticate different bytes. The submitted supplementary hash is absent in that batch, which must remain explicit.

Project only retained assessments onto generation occurrences. Report annotated local repairs separately from meaning/no-new-error-qualified repairs, sound passages, explicit edit necessity, clean preservation and abstention. Clean-source all-defects-repaired is inapplicable, often null. Unknown judgments remain unknown; repeated layers/conditions do not create independent observations. Heterogeneous reviewer taxonomies cannot supply a fabricated exact introduced-grammar total.

Evidence: 1075/1075 main subjects, 478/478 expected candidate/error pairs, zero human finalization; 920 saved generations only. Literal JSON has paired local repair gains without local losses versus v3, but both retain critical accepted semantic damage. Dutch checklist/few-shot/English original instructions have different tradeoffs. Reference-only ranking and automatic production promotion are rejected. Keep production v3, preserve the stacked draft PR, and stop at the evidence-based decision checkpoint.

See [full reconciliation](../phase4-external-review-report.md), [validation matrix](../../benchmarks/phase4/external-gpt6/v1/VALIDATION.md), and [paired diagnostic analysis](../../benchmarks/phase4/external-gpt6/v1/analysis/paired.json). No validator relaxation, new inference, new linguistic judgments or new dependency is implemented.
