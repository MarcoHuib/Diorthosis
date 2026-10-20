using Diorthosis.Core.Corrections;
using Diorthosis.Core.Inference;
using Xunit;

namespace Diorthosis.Tests;

public sealed class CorrectionPipelineTests
{
    [Fact]
    public void PromptPinsVersionLanguageAndEscapedInput()
    {
        var prompt = CorrectionPrompt.Build(new CorrectionRequest(CorrectionLanguage.Dutch, "regel\n\"tekst\""));
        Assert.Equal("diorthosis-correction-v3", CorrectionPrompt.Version);
        Assert.Contains("Dutch", prompt);
        Assert.Contains("\\n", prompt);
        Assert.Contains("Return only the complete corrected passage", prompt);
        Assert.DoesNotContain("Example input", prompt);
    }

    [Fact]
    public void ParsesUtf16PositionsAndChecksExactUnicodeSource()
    {
        const string source = "🌷 Ik heb het document verstuurt.";
        var offset = source.IndexOf("verstuurt", StringComparison.Ordinal);
        var json = $$"""[{"start":{{offset}},"length":9,"originalText":"verstuurt","replacementText":"verstuurd"}]""";
        var edits = ModelCorrectionEngine.ParseAndValidate(json, source);
        Assert.Equal(offset, edits[0].Start);
        Assert.Equal("verstuurt", edits[0].OriginalText);
    }

    [Fact]
    public void DerivesUtf16ProposalFromCorrectedPassageAndRemovesModelNote()
    {
        const string source = "🌷 Ik heb het document gisteren verstuurt.";
        const string output = "🌷 Ik heb het document gisteren **verstuurd**.\n*(Note: the participle is corrected.)*";
        var edits = ModelCorrectionEngine.ParseModelResponse(output, source);
        var offset = source.IndexOf("verstuurt", StringComparison.Ordinal);
        Assert.Contains(edits, edit => edit.Start >= offset && edit.Start < offset + "verstuurt".Length);
        Assert.Equal("🌷 Ik heb het document gisteren verstuurd.", ApplyEdits(source, edits));
    }

    [Fact]
    public void RemovesRepeatedOutputBlocksFromChattyModel()
    {
        const string source = "Ik heb het document gisteren verstuurt.";
        var edits = ModelCorrectionEngine.ParseModelResponse(
            "Ik heb het document gisteren verstuurd.\n\nOutput:\n\n\"Ik heb het document gisteren verstuurd.\"",
            source);
        Assert.Equal("Ik heb het document gisteren verstuurd.", ApplyEdits(source, edits));
    }

    [Fact]
    public void UsesCompletePassageBeforeCorrectedTextAndExplanationMarkers()
    {
        const string source = "Please recieve the signed form.";
        var edits = ModelCorrectionEngine.ParseModelResponse(
            "Please receive the signed form.\n\nCorrected text: \"Please receive the signed form.\"\n\nExplanation: the typo was repaired.",
            source);
        Assert.Equal("Please receive the signed form.", ApplyEdits(source, edits));
    }

    [Fact]
    public void ExtractsCompleteQuotedCorrectionBeforeModelAnalysis()
    {
        const string source = "Als hij morgen kom, beginnen we om negen uur.";
        var output = "\"Als hij morgen komt, beginnen we om negen uur.\"\n\nThe input text is grammatically correct. Output: \"Als hij morgen komt, beginnen we om negen uur.\"";
        var edits = ModelCorrectionEngine.ParseModelResponse(output, source);
        Assert.Equal("Als hij morgen komt, beginnen we om negen uur.", ApplyEdits(source, edits));
    }

    [Fact]
    public void ExtractsRepeatedCompleteFirstAnswerBeforeModelLoop()
    {
        const string source = "The informations are included in the report.";
        var output = "The information is included in the report.\n\n" + source
            + "\nThe information is included in the report.\nThe information is included in the report.";
        var edits = ModelCorrectionEngine.ParseModelResponse(output, source);
        Assert.Equal("The information is included in the report.", ApplyEdits(source, edits));
    }

    [Fact]
    public void ExtractsNamedQuotedOutputAndRejectsIncompleteTailWithoutScoringItAsRawCorrection()
    {
        const string source = "On May 3, 2025, ApiGateway-v2 starts at 09:30.";
        var output = "Output text (JSON string): \"On May 3, 2025, ApiGateway-v2 will start at 09:30.\"\n\nPlease provide the JSON input.";
        var analysis = ModelCorrectionEngine.AnalyzeModelResponse(output, source);
        Assert.Equal("On May 3, 2025, ApiGateway-v2 will start at 09:30.", analysis.ExtractedCorrection);
        Assert.Null(analysis.RejectionStage);
        Assert.NotEmpty(analysis.Edits);

        var incomplete = ModelCorrectionEngine.AnalyzeModelResponse("The corrected sentence is not finished", source);
        Assert.Equal("extraction", incomplete.RejectionStage);
        Assert.Empty(incomplete.RawCandidateEdits);
    }

    [Fact]
    public void KeepsExactOrWhitespaceOnlyCorrectTextUnchanged()
    {
        const string source = "De server start opnieuw.";
        Assert.Empty(ModelCorrectionEngine.ParseModelResponse(source, source));
        Assert.Empty(ModelCorrectionEngine.ParseModelResponse("De   server start opnieuw.", source));
        Assert.Empty(ModelCorrectionEngine.ParseModelResponse("De server start opnieuw.\nDe server start opnieuw.", source));
    }

    [Theory]
    [InlineData("node-04", "node 04")]
    [InlineData("ApiGateway-v2", "ApiGateway v2")]
    [InlineData("OAuth2", "OAuth 2")]
    [InlineData("NL-2026-041", "NL 2026 041")]
    [InlineData("help@company.test", "help@other.test")]
    [InlineData("/etc/diorthosis/config.yaml", "/etc/diorthosis/other.yaml")]
    [InlineData("10.0.0.1", "10.0.0.2")]
    [InlineData("v2.1.0", "v2.2.0")]
    [InlineData("2001:db8::1", "2001:db8::2")]
    [InlineData("server_name", "server-name")]
    [InlineData("example.test", "example.invalid")]
    [InlineData(@"C:\\Program Files\\Diorthosis\\config.yaml", @"C:\\Program Files\\Diorthosis\\other.yaml")]
    public void ProtectedSourceTokensAreNeverAccepted(string sourceFragment, string candidateFragment)
    {
        const string prefix = "Use ";
        var source = prefix + sourceFragment + ".";
        var candidate = prefix + candidateFragment + ".";
        var analysis = ModelCorrectionEngine.AnalyzeModelResponse(candidate, source);
        Assert.NotEqual(CorrectionDisposition.Accepted, analysis.Disposition);
        Assert.Empty(analysis.Edits);
    }

    [Theory]
    [InlineData("The server is not available.", "The server is available.")]
    [InlineData("She must not send it.", "She must send it.")]
    [InlineData("You must submit the form.", "You may submit the form.")]
    [InlineData("De server is niet bereikbaar.", "De server is bereikbaar.")]
    public void NegationAndModalityChangesAreNotAccepted(string source, string candidate)
    {
        var analysis = ModelCorrectionEngine.AnalyzeModelResponse(candidate, source);
        Assert.NotEqual(CorrectionDisposition.Accepted, analysis.Disposition);
        Assert.Empty(analysis.Edits);
    }

    [Theory]
    [InlineData("I can't send it.", "I can send it.")]
    [InlineData("She doesn't agree.", "She does agree.")]
    [InlineData("The service isn't ready.", "The service is ready.")]
    [InlineData("We won't deploy it.", "We will deploy it.")]
    [InlineData("Er is geen risico.", "Er is risico.")]
    public void ContractedAndDutchNegationChangesAreNotAccepted(string source, string candidate)
    {
        var analysis = ModelCorrectionEngine.AnalyzeModelResponse(candidate, source);
        Assert.NotEqual(CorrectionDisposition.Accepted, analysis.Disposition);
        Assert.Empty(analysis.Edits);
    }

    [Fact]
    public void CorrectedPassagePreservesMeaningfulOuterWhitespace()
    {
        const string source = "  Ik heb het document verstuurt.  ";
        const string corrected = "  Ik heb het document verstuurd.  ";
        var edits = ModelCorrectionEngine.ParseModelResponse(corrected, source);
        Assert.Equal(corrected, ApplyEdits(source, edits));
        Assert.DoesNotContain(edits, edit => edit.OriginalText is " " or "\n");
    }

    [Fact]
    public void CompleteIndependentStatementRemovalIsRejectedButClauseJoiningIsAllowed()
    {
        var omitted = ModelCorrectionEngine.AnalyzeModelResponse("The server starts.", "The server starts. It runs.");
        Assert.NotEqual(CorrectionDisposition.Accepted, omitted.Disposition);
        Assert.Empty(omitted.Edits);

        var joined = ModelCorrectionEngine.ParseModelResponse(
            "Omdat de server opnieuw opstartte, werd de taak onderbroken.",
            "Omdat de server opnieuw opstartte. De taak werd onderbroken.");
        Assert.NotEmpty(joined);
    }

    [Theory]
    [InlineData("Het start op 3 mei 2025.", "Het start op 3 juni 2025.")]
    [InlineData("It starts on May 3, 2025.", "It starts on June 3, 2025.")]
    [InlineData("The meeting is on 3 May 2025.", "The meeting is on 3 June 2025.")]
    public void WrittenDatesRemainSourceDerived(string source, string candidate)
    {
        var analysis = ModelCorrectionEngine.AnalyzeModelResponse(candidate, source);
        Assert.NotEqual(CorrectionDisposition.Accepted, analysis.Disposition);
    }

    [Theory]
    [InlineData("vergader ruimte", "vergaderruimte")]
    [InlineData("alot", "a lot")]
    [InlineData("Ik heb teveel betaald.", "Ik heb te veel betaald.")]
    public void LegitimateWhitespaceCorrectionsRemainOperational(string source, string candidate)
    {
        var edits = ModelCorrectionEngine.ParseModelResponse(candidate, source);
        Assert.NotEmpty(edits);
    }

    [Fact]
    public void CosmeticWhitespaceSuppressionIsExplicitAndDoesNotEraseSourceFormatting()
    {
        var source = "  The **server**  is `ready`.  ";
        var unchanged = ModelCorrectionEngine.AnalyzeModelResponse(source, source);
        Assert.Equal(CorrectionDisposition.Unchanged, unchanged.Disposition);
        Assert.Empty(unchanged.Edits);

        var cosmetic = ModelCorrectionEngine.AnalyzeModelResponse("The  server is ready.", "The server is ready.");
        Assert.Equal(CorrectionDisposition.Suppressed, cosmetic.Disposition);
        Assert.Empty(cosmetic.Edits);
    }

    [Fact]
    public void StatementToQuestionIsNotCosmetic()
    {
        var analysis = ModelCorrectionEngine.AnalyzeModelResponse("The server is ready?", "The server is ready.");
        Assert.NotEqual(CorrectionDisposition.Accepted, analysis.Disposition);
    }

    [Fact]
    public void StructuredJsonUsesTheSamePassageSafetyPolicy()
    {
        const string source = "The server is not available.";
        var json = "[{\"start\":14,\"length\":3,\"originalText\":\"not\",\"replacementText\":\"\"}]";
        Assert.Throws<InvalidDataException>(() => ModelCorrectionEngine.ParseAndValidate(json, source));
    }

    [Fact]
    public void StructuredJsonUsesTheSamePlausibilityPolicyAsPassageResponses()
    {
        const string source = "The server is ready for maintenance.";
        const string candidate = "A completely unrelated correction is ready.";
        var output = $$"""[{"start":0,"length":{{source.Length}},"originalText":"The server is ready for maintenance.","replacementText":"A completely unrelated correction is ready."}]""";

        var passage = ModelCorrectionEngine.AnalyzeModelResponse(candidate, source);
        Assert.NotEqual(CorrectionDisposition.Accepted, passage.Disposition);
        Assert.Throws<InvalidDataException>(() => ModelCorrectionEngine.ParseAndValidate(output, source));
    }

    [Fact]
    public async Task MaximumTokenInferenceCannotBecomeAcceptedCorrection()
    {
        var adapter = new FakeAdapter("She has received it.", GenerationTermination.MaximumTokens);
        var result = await new ModelCorrectionEngine(adapter).CorrectAsync(
            new CorrectionRequest(CorrectionLanguage.English, "She have recieved it."));
        Assert.Empty(result.Edits);
    }

    [Fact]
    public void AcceptsDutchCompoundCorrectionThatOnlyRemovesWhitespace()
    {
        const string source = "De vergader ruimte is gereserveerd.";
        var edits = ModelCorrectionEngine.ParseModelResponse("De vergaderruimte is gereserveerd.", source);
        var edit = Assert.Single(edits);
        Assert.Equal(" ", edit.OriginalText);
        Assert.Equal(string.Empty, edit.ReplacementText);
    }

    [Fact]
    public void ProducesMultipleUnicodeSafeEditsForIndependentCorrections()
    {
        const string source = "🌷 She have recieved the forms and teh list.";
        const string corrected = "🌷 She has received the forms and the list.";
        var edits = ModelCorrectionEngine.ParseModelResponse(corrected, source);
        Assert.True(edits.Count >= 2);
        var result = source;
        foreach (var edit in edits.OrderByDescending(edit => edit.Start))
            result = result.Remove(edit.Start, edit.Length).Insert(edit.Start, edit.ReplacementText);
        Assert.Equal(corrected, result);
        Assert.All(edits, edit => Assert.Equal(edit.OriginalText, source.Substring(edit.Start, edit.Length)));
    }

    [Fact]
    public void AllowsSentenceInitialFunctionWordCaseCorrectionButProtectsNames()
    {
        Assert.Single(ModelCorrectionEngine.ParseModelResponse("The server is ready.", "the server is ready."));
        Assert.Throws<InvalidDataException>(() => ModelCorrectionEngine.ParseModelResponse(
            "Alicia reviewed the file.", "Alice reviewed the file."));
    }

    [Fact]
    public void AnalysisSeparatesRawCandidateFromSafetyRejectedEdits()
    {
        var analysis = ModelCorrectionEngine.AnalyzeModelResponse(
            "Alicia reviewed the file.", "Alice reviewed the file.");
        Assert.Equal("validation", analysis.RejectionStage);
        Assert.NotEmpty(analysis.RawCandidateEdits);
        Assert.Empty(analysis.Edits);
    }

    [Fact]
    public void TranslationIsRejectedForChangedWordingRatherThanMistakenAsChangedName()
    {
        var analysis = ModelCorrectionEngine.AnalyzeModelResponse(
            "There is a problem with the connection.", "Er zijn een probleem met de verbinding.");
        Assert.Equal("validation", analysis.RejectionStage);
        Assert.Contains("too much source wording", analysis.RejectionReason, StringComparison.Ordinal);
    }

    [Fact]
    public void DoesNotTreatUnrequestedMultilineModelExplanationAsCandidateCorrections()
    {
        var analysis = ModelCorrectionEngine.AnalyzeModelResponse(
            "Please recieve the form.\n\nThis is a repeated commentary paragraph.\nMore unrelated response text.",
            "Please recieve the form.");
        Assert.Equal("extraction", analysis.RejectionStage);
        Assert.Empty(analysis.RawCandidateEdits);
        Assert.Empty(analysis.Edits);
    }

    [Fact]
    public void RejectsModelOutputThatDropsEmojiOrSymbol()
    {
        const string source = "🌷 Ik heb het document gisteren verstuurt.";
        Assert.Throws<InvalidDataException>(() => ModelCorrectionEngine.ParseModelResponse(
            "Ik heb het document gisteren verstuurd.", source));
    }

    [Theory]
    [InlineData("")]
    [InlineData("[]")]
    [InlineData("This is a completely unrelated explanation.")]
    [InlineData("She sent all the data to a different server.")]
    [InlineData("Ik heb het document gisteren verstuurd")]
    public void RejectsMalformedOrUnverifiableModelPassages(string output) =>
        Assert.Throws<InvalidDataException>(() => ModelCorrectionEngine.ParseModelResponse(output, "Ik heb het document gisteren verstuurt."));

    [Theory]
    [InlineData("[{\"start\":99,\"length\":1,\"originalText\":\"x\",\"replacementText\":\"y\"}]")]
    [InlineData("[{\"start\":0,\"length\":2,\"originalText\":\"zz\",\"replacementText\":\"xx\"}]")]
    [InlineData("[{\"start\":0,\"length\":1,\"originalText\":\"a\",\"replacementText\":\"b\"},{\"start\":0,\"length\":1,\"originalText\":\"a\",\"replacementText\":\"c\"}]")]
    [InlineData("[{\"start\":0,\"length\":1,\"originalText\":\"1\",\"replacementText\":\"2\"}]")]
    [InlineData("[{\"start\":0,\"length\":4,\"originalText\":\"2024\",\"replacementText\":\"2025\"}]")]
    [InlineData("[{\"start\":0,\"length\":14,\"originalText\":\"https://a.test\",\"replacementText\":\"https://b.test\"}]")]
    [InlineData("[{\"start\":0,\"length\":6,\"originalText\":\"AcmeID\",\"replacementText\":\"AcneID\"}]")]
    [InlineData("[{\"start\":0,\"length\":0,\"originalText\":\"\",\"replacementText\":\"x\"},{\"start\":0,\"length\":0,\"originalText\":\"\",\"replacementText\":\"y\"}]")]
    public void RejectsUnsafeEdits(string json) => Assert.Throws<InvalidDataException>(() => ModelCorrectionEngine.ParseAndValidate(json, "a 12"));

    [Fact]
    public async Task CorrectTextCanBePreservedAndCancellationFlowsToAdapter()
    {
        var source = "Correct text.";
        var adapter = new FakeAdapter(source);
        var engine = new ModelCorrectionEngine(adapter);
        var result = await engine.CorrectAsync(new CorrectionRequest(CorrectionLanguage.English, source));
        Assert.Empty(result.Edits);
        Assert.Equal(source, adapter.LastResponse);

        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var cancelAdapter = new FakeAdapter("", cancel: true);
        await Assert.ThrowsAnyAsync<OperationCanceledException>(async () => await new ModelCorrectionEngine(cancelAdapter)
            .CorrectAsync(new CorrectionRequest(CorrectionLanguage.English, "Text."), cts.Token));
    }

    [Fact]
    public async Task UnverifiableInferenceOutputLeavesSourceUnchanged()
    {
        var engine = new ModelCorrectionEngine(new FakeAdapter(""));
        var result = await engine.CorrectAsync(new CorrectionRequest(CorrectionLanguage.Dutch, "Ik heb het document verstuurt."));
        Assert.Empty(result.Edits);
        Assert.Equal(1, engine.RejectedOutputCount);
    }

    [Fact]
    public void RejectsMissingModelFileWithoutLeakingContent()
    {
        var error = Assert.Throws<FileNotFoundException>(() => new LlamaSharpInferenceAdapter(Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".gguf")));
        Assert.Contains("local GGUF", error.Message);
    }

    [Fact]
    public void ValidatesModelConfigurationBeforeNativeInitialization()
    {
        var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid() + ".gguf");
        File.WriteAllBytes(path, []);
        try
        {
            Assert.Throws<InvalidDataException>(() => new LlamaSharpInferenceAdapter(path));
            File.WriteAllBytes(path, "GGUF"u8.ToArray());
            Assert.Throws<ArgumentOutOfRangeException>(() => new LlamaSharpInferenceAdapter(path, contextSize: 128));
            Assert.Throws<ArgumentOutOfRangeException>(() => new LlamaSharpInferenceAdapter(path, threads: 0));
            Assert.Throws<ArgumentException>(() => new LlamaSharpInferenceAdapter(path + ".txt"));
        }
        finally { File.Delete(path); }
    }

    private sealed class FakeAdapter(string response, GenerationTermination termination = GenerationTermination.EndOfGeneration, bool cancel = false) : IModelInferenceAdapter
    {
        public string LastResponse { get; private set; } = string.Empty;
        public ValueTask<InferenceResult> GenerateAsync(InferenceRequest request, CancellationToken cancellationToken = default)
        {
            LastResponse = response;
            if (cancel) cancellationToken.ThrowIfCancellationRequested();
            return ValueTask.FromResult(new InferenceResult(response, termination, 1));
        }
    }

    private static string ApplyEdits(string source, IReadOnlyList<TextEdit> edits)
    {
        foreach (var edit in edits.OrderByDescending(edit => edit.Start))
            source = source.Remove(edit.Start, edit.Length).Insert(edit.Start, edit.ReplacementText);
        return source;
    }
}
