# ADR 0002: Candidate implementation stack

- **Status:** Proposed, not accepted
- **Date:** 2026-10-19

## Context

The tool needs local inference and Word document processing without a separate runtime installation. Earlier discussions identified a C# app, Native AOT, and llama.cpp as promising options, but their suitability must be proven.

## Options to evaluate

1. C# / .NET self-contained or Native AOT versus another portable app runtime.
2. In-process llama.cpp versus a different fully offline, commercially permissible inference engine.
3. Multiple European-origin AI model checkpoints; test language performance and provenance explicitly.
4. Pure AI correction versus hybrid rule/dictionary support, based on measured quality.
5. Independent `.docx` and `.doc` handling; test cross-platform libraries for license compliance and formatting fidelity.

## Decision gate

Do not promote any option to accepted before documenting benchmark results, licenses, operational footprint and limitations across Windows, macOS and Linux.
