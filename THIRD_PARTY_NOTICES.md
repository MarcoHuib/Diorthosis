# Third-party licenses and redistribution notices

No third-party runtime binaries, dictionaries, or model weights are bundled in this repository starter.

Phase 1 uses test-only NuGet packages: Microsoft.NET.Test.Sdk 17.14.1, xUnit 2.9.3, and xunit.runner.visualstudio 3.1.4. They are not included in the CLI or benchmark release artifacts. Their package license files and transitive notices should be captured in CI dependency review before redistribution of any future test tooling.

Before introducing a dependency, record:

| Item/version | Project/homepage | Code/asset license | Commercial redistribution checked? | Model origin verified? | Required notices | Package location |
| --- | --- | --- | --- | --- | --- | --- |

License identifiers alone are not sufficient: verify the exact version, binaries, weights, tokenizer, dictionary data, transitive dependencies and any special model terms. Include required license text/attributions in releases. Update this manifest as dependencies are added.
