# Third-party licenses and redistribution notices

No third-party runtime binaries, dictionaries, or model weights are committed to this repository. Runtime dependencies are resolved from the pinned NuGet packages when building or publishing.

The CLI references LLamaSharp 0.27.0 and LLamaSharp.Backend.Cpu 0.27.0 (MIT). LLamaSharp 0.27.0's NuGet release notes pin llama.cpp commit `3f7c29d318e317b63f54c558bc69803963d7d88c`; the LLamaSharp repository commit is `7cbbc45e421d55794d5050d126e0b96511007007`. The CPU package supplies platform-specific llama.cpp / ggml native libraries at publish time. Its current package also carries `libmtmd` assets; Diorthosis does not call its multimodal APIs or load a vision projector, but release packaging should assess whether those assets can be excluded. LLamaSharp's managed transitive dependencies are CommunityToolkit.HighPerformance 8.4.2, Microsoft.Bcl.AsyncInterfaces 10.0.5, Microsoft.Bcl.Memory 10.0.5, Microsoft.Extensions.AI.Abstractions 10.4.1, Microsoft.Extensions.Logging.Abstractions 10.0.5, System.Linq.Async 7.0.0, System.Linq.AsyncEnumerable 10.0.2, and System.Numerics.Tensors 10.0.5. Before distributing artifacts, capture each license, the exact backend binary hashes, required MIT notices, and a complete resolved dependency inventory. No model or tokenizer assets are included.

Phase 1 uses test-only NuGet packages: Microsoft.NET.Test.Sdk 17.14.1, xUnit 2.9.3, and xunit.runner.visualstudio 3.1.4. They are not included in the CLI or benchmark release artifacts. Their package license files and transitive notices should be captured in CI dependency review before redistribution of any future test tooling.

Before introducing a dependency, record:

| Item/version | Project/homepage | Code/asset license | Commercial redistribution checked? | Model origin verified? | Required notices | Package location |
| --- | --- | --- | --- | --- | --- | --- |

License identifiers alone are not sufficient: verify the exact version, binaries, weights, tokenizer, dictionary data, transitive dependencies and any special model terms. Include required license text/attributions in releases. Update this manifest as dependencies are added.
