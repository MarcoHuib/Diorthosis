# ADR 0008: Audited correction safety and evaluator v2

- **Status:** Implemented locally in PR #2
- **Date:** 2026-10-20

## Decision

Keep raw model output, extracted passage, candidate edits, validated edits and final disposition distinct. Model passages and structured JSON edits share one source-aware safety policy for protected technical content, written dates, critical negation/modality markers, speech-act punctuation and independent-statement omission. Uncertain or unsafe changes produce no applicable edits.

Inference adapters return an `InferenceResult` containing raw text, termination and sampled-token count. Only end-of-generation responses are eligible for correction analysis; maximum-token and unknown termination are incomplete.

The historical evaluator remains available through its existing metric rows. A complementary `v2-error-level` result scores complete annotated passages before category attribution and preserves undefined ratios as null with their numerator and denominator.

## Rationale and limits

The safety rules protect known critical structures and fail closed; they do not prove semantic equivalence for arbitrary prose. Sentence restructuring is allowed when source propositions remain represented, rather than requiring equal sentence counts. Accepted references and historical artifacts are not changed.

The evaluator's complete-case matching is bounded to 256 accepted-reference combinations and 32 alternatives per annotation. It remains an exact reference comparison, not a semantic judge; true correction precision and semantic safety still require adjudicated labels.

The negation/modality protection is intentionally a bounded marker list, not comprehensive semantic analysis. Outer whitespace is preserved when the model returns the source-aligned boundaries; wrapper recovery remains conservative for chatty output. Category-specific evaluator precision attributes edits touching that category's annotations; unannotated or cross-category edits still require interpretation through the all-category and historical rows.
