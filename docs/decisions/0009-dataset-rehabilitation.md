# ADR 0009: Preserve grouped development evidence

Date: 2026-10-20. Status: implemented diagnostic infrastructure, no linguistic approval.

Use deterministic .NET auditing with no new dependency. Preserve the 600-line original, all annotations, conflict variants, and ID mappings in a separately versioned group candidate. NFC/whitespace/case and token-bigram Dice are diagnostics only. Explicit template metadata overrides sentence-level partitioning. Reject detected split overlap by default. Do not invent a held-out split from development-exposed data.

Rejected: deleting duplicate records without provenance; choosing one conflicting gold reference; automatically merging near matches; random sentence splits; generating repetitive examples to reach a nominal sample size. These hide disagreement or contamination. Human review remains mandatory. The checked report reproduces 600 records, 263 exact unique texts, 337 extras, 86 duplicate groups, 423 records in those groups and maximum multiplicity 23; 56 near pairs at threshold 0.85, 0 malformed annotation spans. See the versioned report for language/category and disagreement details. Deterministic suite: 90 pass, 1 local-model skip, 0 failures. No inference, latency or quality improvement is asserted.
