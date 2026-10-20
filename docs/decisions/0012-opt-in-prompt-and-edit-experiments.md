# ADR 0012: Gate prompt, edit and grammar research behind explicit experiments

Date: 2026-10-20. Status: implemented experimental contracts; no production prompt promotion.

Preserve the byte-identical baseline. Use a separate engine/prompt family for language minimal, two-example few-shot, strict UTF-16 offsets and uniquely anchored fragments. Forward optional narrow GBNF through the pinned greedy sampler, retaining the same EOG tracking. Never trust generated offsets or JSON grammar as semantic evidence. Ambiguous fragments require review; invalid edits are rejected. Shared passage validation keeps dependent changes atomic.

Rejected: upgrading LLamaSharp for a generic JSON mode; silently replacing full-passage output; choosing a repeated fragment occurrence; using arbitrary model-provided rationale as confidence; expanding prompt examples after seeing failures. See `docs/correction-experiments.md` for the predeclared eight-case development comparison and fixed settings. Real-weight results are recorded after the implementation checkpoint, using its exact commit. Deterministic suite: 137 passed, 1 weight-dependent skip, 0 failed. No quality/latency advantage is inferred from those tests. Cross-platform native grammar and independent linguistic validation remain outstanding.
