# ADR 0013: Correct bounded review defects without relabeling evidence

Status: accepted for development, 2026-10-20.

Separate technical AI reviews exposed canonical Unicode offset drift, shared edit grouping, unknown-denominator and review-identity defects in reference evaluator 3.0. New evaluations identify as `reference-groups-3.1.0`; historical v1/v2 and frozen v3 reports remain unchanged. Stable proposal identities bind source and annotations as well as candidate edits.

Literal parsed candidates bypass chat extraction. Semantic anchors conservatively block demonstrated unsafe changes and expose rejection reasons; their known compound/pronoun recall costs remain explicit. Broadly exempting whitespace or pronoun substitutions was rejected: those exemptions can also change protected meaning. The production prompt is unchanged.

Replay verifies available dataset/trace bindings, isolates unverifiable legacy run identity/resource claims, and hashes its own emitted trace. New outputs use exclusive creation. Native builds require freshly cloned clean pinned official source. Reusing a cached checkout after checkout alone was rejected because dirty tracked content can survive. No additional runtime dependency. See phase2-technical-review.md for reproductions, evidence and limitations.
