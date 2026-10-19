# ADR 0001: Product and licensing constraints

- **Status:** Accepted
- **Date:** 2026-10-19

## Context

The intended users may need spelling and grammar support while working with confidential material that cannot be submitted to hosted AI products. The application should be portable across mainstream desktop operating systems and legally reusable in commercial settings.

## Decision

- Build an offline-first correction tool for Dutch and English.
- Support Windows, Linux and macOS, with architecture-specific builds as needed.
- Require no separately installed language runtimes or AI servers; Java is excluded as a runtime dependency.
- Allow only commercially reusable open-source components and properly licensed AI assets.
- Restrict AI-model origin to Europe, excluding China and Russia.
- Preserve original document files and provide reviewable suggestions.
- Require data-driven quality validation; 90% repaired error recall is a proposed minimum, not an established result.

## Consequences

We may reject otherwise convenient models, libraries, and document converters. Packaging effort is greater because several platform builds are necessary. Proof of correctness, provenance, and document preservation is part of implementation, not optional polish.
