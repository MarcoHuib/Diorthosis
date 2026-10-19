# ADR 0003: Phase 1 foundation and small-model shortlist

- **Status:** Accepted for implementation
- **Date:** 2026-10-19

## Context

The first implementation milestone should establish independently testable CLI, correction, inference, and benchmark foundations without selecting an inference runtime or processing documents prematurely.

## Decision

- Use a .NET 10 C# solution with a minimal CLI and Native AOT publishing configured per target runtime identifier.
- Define minimal correction request/result types, `ICorrectionEngine`, and `IModelInferenceAdapter` in Core.
- Evaluate proposed edits using rights-cleared JSONL cases, source spans, and one or more accepted alternatives. Report metrics separately by language and error category; review semantic changes separately.
- Keep model files external to the executable. Baseline inference is planned for CPU, offline, and in-process. Do not add an external service, GPU dependencies, or a spelling engine in Phase 1.
- Prioritize `mistralai/Ministral-3-3B-Instruct-2512-GGUF` (upstream lists Apache 2.0, Dutch and English, official llama.cpp instructions, and Q4_K_M at 2.15 GB) and `utter-project/EuroLLM-1.7B-Instruct` (upstream lists Apache 2.0, Dutch and English, and EU funding). EuroLLM GGUF conversion remains a validation task. Neither model is benchmark-approved.

## Consequences

The benchmark harness is usable before model integration. Native AOT and launch checks must run on target-compatible environments. The synthetic smoke fixture verifies evaluator behavior only; it is not evidence of correction quality. No model weights or inference packages are added.

## Primary sources

- [Ministral 3 3B Instruct 2512 GGUF](https://huggingface.co/mistralai/Ministral-3-3B-Instruct-2512-GGUF)
- [EuroLLM 1.7B Instruct](https://huggingface.co/utter-project/EuroLLM-1.7B-Instruct)
- [.NET Native AOT deployment](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
