# ADR 0014: Bound external linguistic judgments and isolate context targets

Status: development foundation, 2026-10-20.

Reference evaluator 3.1 remains a diagnostic reference matcher. It cannot establish true precision or meaning preservation. External proposal/passage/rejection decisions enter a separate versioned review layer. Two independent declared human reviewers and a separate adjudicator are required for finalized human metrics. AI labels stay explicitly AI; unknown judgments have no numeric estimate. Model identity is kept in a separate lineage file for blind packet delivery. Content and reviewer-version binding prevent accidental stale imports; software cannot authenticate real people. A documents-only workflow was rejected because it cannot enforce these contracts.

New candidate passages remain exposed development proposals, with source scenario and generation provenance. Distinct scenario IDs are not independent human sources. No current material is relabeled held-out. Stored traces are replayed before bounded inference, with exact source hashes and no reused timings.

Context readiness is an opt-in API using an immutable document snapshot, stable target/paragraph IDs, explicit language, bounded read-only neighbors and copied opaque metadata. All local edits are validated and mapped inside the target using literal UTF-16/grapheme boundaries. Stale snapshots fail. No parser, exporter or round-trip fidelity claim follows. Extending .doc/.docx editing is a separate milestone. The production prompt remains byte-for-byte unchanged.
