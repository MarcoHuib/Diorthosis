# ADR 0017: Controlled prompt quality experiments

2026-10-21. Status: experimental, no production promotion.

Keep v3 byte-identical and put explicit representation/instruction selection in the existing Experiments assembly. B uses the relaxed JSON encoder solely to serialize prompt data; it preserves JSON escaping without applying Unicode normalization and is never embedded in HTML. C uses a source-dependent collision-checked delimiter plus an explicit untrusted-data boundary. This boundary is defense in depth, not a proof of model instruction isolation. Keep the same downstream passage validation for every arm.

Separate representation, added minimal-edit policy, added language checklist and development-only few-shot examples. Reject a full factorial search and new post-processing guards because they obscure the causal comparison and expand tuning opportunities. Do not silently use a proxy-selected prompt in production. Retain local inference records, failure attempts and source/assembly/native identities; content tokenizer measurements exclude template/BOS and do not alter generation.

Use original fictional AI fixtures under the project license rather than importing uncertain corpora. Reserve confirmation from inference until one locked comparison; disclose creator exposure. External assessments stay AI-labeled and separate from lineage and preparer judgments. BLiMP-NL investigation and adoption limits are recorded in the Phase 4 research report; no external dataset or new dependency is redistributed.

Acceptance and stopping rules: [protocol](../phase4-plan.md). Production requires separately approved linguistic evidence and current-head cross-platform/AOT checks. No model architecture, validation policy, end-user runtime or network behavior changes.

Evidence checkpoint: 920 greedy local quality generations completed, all EOG, with 200 frozen fixtures and one locked 40-case confirmation pass. Literal JSON with original instructions advances only to external review. Added language instructions improve Dutch reference recall but lose English repair/clean preservation; few-shot is mixed and more expensive; minimal instructions introduce accepted appetite-to-nausea drift. Combined minimal+few-shot trigger failed. Retain v3. The blinded package has 1,075 bound subjects and eighteen batches; same-session exposed AI observations remain separate. See [full report](../phase4-report.md). No independent/human quality gate is met or promotion authorized.
