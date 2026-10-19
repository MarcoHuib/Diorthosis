# Diorthosis

**An offline, privacy-first writing assistant for Dutch and English.**

Diorthosis aims to help people confidently review spelling, grammar, and sentence structure in documents without uploading confidential content to a cloud service.

> **Project status:** planning / repository bootstrap. No working correction engine or document converter is included in this initial commit. Model accuracy, format preservation, and platform compatibility are targets that still require testing.

## Project goals

- Run entirely on the user's own computer, with no network access or telemetry.
- Support Windows, macOS, and Linux, with portable, platform-specific releases.
- Require no separate installation of Java, Python, .NET, Docker, Ollama, or an AI runtime.
- Check Dutch and English, with a manually selectable language.
- Read `.txt`, `.docx`, and `.doc`. Produce reviewable suggestions; preserve originals and document formatting wherever verifiably supported.
- Use open-source components and AI models whose terms permit commercial use and redistribution.
- Use only European-origin AI models, excluding models originating in China or Russia.
- Aim for **90–100%** correct repairs on a defined test set, while separately measuring false positives and unwanted semantic changes.

## Proposed architecture

A shared C# command-line application is the starting point to evaluate. A pluggable local inference engine and an independently tested document-processing layer keep model selection and file-format support replaceable.

See [architecture](docs/architecture.md), [benchmarks](docs/benchmarks.md), and [roadmap](docs/roadmap.md).

## Planned usage (not implemented yet)

```console
# Illustrative CLI only; commands will be finalized after the prototype
corrector document.docx --language nl --review
corrector document.txt --language en --report corrections.html
```

## Contributing

Read [AGENTS.md](AGENTS.md) before proposing design or code changes. Significant architecture choices must be recorded under `docs/decisions/`. Contributions must not introduce network dependencies or unapproved licensing requirements.

## License

The original code and project documentation in this repository are intended to be published under **Apache License 2.0**, see [LICENSE](LICENSE). Dependencies, third-party data, model weights, and dictionaries retain their own licenses and require independent approval and notices before redistribution. No AI-model weights are bundled in this starter repository.
