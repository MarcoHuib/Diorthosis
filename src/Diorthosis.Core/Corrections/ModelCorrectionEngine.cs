using System.Text.Json;
using System.Text.RegularExpressions;
using System.Text;
using System.Globalization;
using Diorthosis.Core.Inference;

namespace Diorthosis.Core.Corrections;

public sealed class ModelCorrectionEngine(IModelInferenceAdapter adapter, int maximumOutputTokens = 512) : ICorrectionEngine
{
    private static readonly Regex UrlToken = new(@"(?:https?://|www\.)[^\s<>]+", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex EmailToken = new(@"(?<![\p{L}\p{N}])[\p{L}\p{N}._%+\-]+@[\p{L}\p{N}.\-]+\.[\p{L}]{2,}(?![\p{L}\p{N}])", RegexOptions.CultureInvariant);
    private static readonly Regex PathToken = new(@"(?<![\p{L}\p{N}])(?:[A-Za-z]:\\(?:[^\\<>\r\n]+\\)*[^\s<>\\]+|/(?:[^\s<>/]+/)*[^\s<>/]+)(?![\p{L}\p{N}])", RegexOptions.CultureInvariant);
    private static readonly Regex DomainToken = new(@"(?<![\p{L}\p{N}])(?:[\p{L}\p{N}-]+\.)+[\p{L}]{2,}(?![\p{L}\p{N}])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex IpToken = new(@"(?<![\p{L}\p{N}])(?:\d{1,3}\.){3}\d{1,3}|(?<![\p{L}\p{N}])(?:[0-9A-Fa-f]{0,4}:){2,7}[0-9A-Fa-f]{0,4}(?![\p{L}\p{N}])", RegexOptions.CultureInvariant);
    private static readonly Regex WrittenDateToken = new(@"(?<![\p{L}\p{N}])(?:[0-3]?\d\s+(?:jan(?:uary|uari)?|feb(?:ruary|ruari)?|march|mar|maart|mrt|apr(?:il)?|may|mei|june|jun|juni|july|jul|juli|aug(?:ust|ustus)?|sep(?:tember)?|okt(?:ober)?|oct(?:ober)?|nov(?:ember)?|dec(?:ember)?)[\s,]+\d{4}|(?:jan(?:uary|uari)?|feb(?:ruary|ruari)?|march|mar|maart|mrt|apr(?:il)?|may|mei|june|jun|juni|july|jul|juli|aug(?:ust|ustus)?|sep(?:tember)?|okt(?:ober)?|oct(?:ober)?|nov(?:ember)?|dec(?:ember)?)\s+[0-3]?\d(?:st|nd|rd|th)?[\s,]+\d{4})(?![\p{L}\p{N}])", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
    private static readonly Regex NumberToken = new(@"\d+(?:[.,:/-]\d+)*", RegexOptions.CultureInvariant);
    private static readonly Regex ProtectedIdentifier = new(@"\b(?:[A-Za-z0-9]*[a-z][A-Z][A-Za-z0-9]*|[A-Z]{2,}[A-Z0-9]*|[A-Za-z0-9]*\d+[A-Za-z0-9]*|[A-Za-z0-9]+(?:[-_][A-Za-z0-9]+)+)\b", RegexOptions.CultureInvariant);
    private static readonly Regex WordToken = new(@"[\p{L}\p{N}_]+", RegexOptions.CultureInvariant);
    private readonly IModelInferenceAdapter _adapter = adapter ?? throw new ArgumentNullException(nameof(adapter));
    public int MaximumOutputTokens => maximumOutputTokens;
    private int _rejectedOutputCount;
    public int RejectedOutputCount => Volatile.Read(ref _rejectedOutputCount);

    public async ValueTask<CorrectionResult> CorrectAsync(CorrectionRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);
        if (maximumOutputTokens <= 0) throw new ArgumentOutOfRangeException(nameof(maximumOutputTokens));
        var inference = await _adapter.GenerateAsync(new InferenceRequest(CorrectionPrompt.Build(request), maximumOutputTokens), cancellationToken).ConfigureAwait(false);
        var analysis = AnalyzeInference(inference, request);
        if (analysis.Disposition is CorrectionDisposition.Rejected or CorrectionDisposition.NeedsReview)
            Interlocked.Increment(ref _rejectedOutputCount);
        return new CorrectionResult(analysis.Edits) { Disposition = analysis.Disposition, Analysis = analysis, Termination = inference.Termination };
    }

    public static ModelResponseAnalysis AnalyzeInference(InferenceResult inference, CorrectionRequest request)
    {
        ArgumentNullException.ThrowIfNull(inference);
        ArgumentNullException.ThrowIfNull(request);
        if (inference.Termination != GenerationTermination.EndOfGeneration)
            return ModelResponseAnalysis.Rejected(inference.Text, null, [], "termination", "Generation did not finish naturally at end-of-generation.");
        var analysis = AnalyzeModelResponse(inference.Text, request.Text);
        if (analysis.Disposition == CorrectionDisposition.Accepted && request.ProtectedSpans.Any(span => analysis.Edits.Any(edit =>
            edit.Length == 0 ? edit.Start > span.Start && edit.Start < span.Start + span.Length
                : edit.Start < span.Start + span.Length && edit.Start + edit.Length > span.Start)))
            return ModelResponseAnalysis.Rejected(inference.Text, analysis.ExtractedCorrection, analysis.RawCandidateEdits, "protected-span", "Candidate intersects explicitly protected source content.");
        return analysis;
    }

    public static ModelResponseAnalysis AnalyzeModelResponse(string output, string source)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(source);
        if (source.Length > 32768 || output.Length > Math.Max(8192, source.Length * 2 + 256))
            return ModelResponseAnalysis.Rejected(output, null, [], "limits", "Text exceeds correction validation limits.");
        try { SourceEdits.EnsureUnicode(source); SourceEdits.EnsureUnicode(output); }
        catch (InvalidDataException) { return ModelResponseAnalysis.Rejected(output, null, [], "unicode", "Malformed UTF-16 text."); }
        if (output == source) return ModelResponseAnalysis.Unchanged(output, source);
        string candidate;
        try { candidate = ExtractCandidate(output, source); }
        catch (InvalidDataException exception) { return ModelResponseAnalysis.Rejected(output, null, [], "extraction", exception.Message); }
        if (!source.Contains('\n') && candidate.Contains('\n'))
            return ModelResponseAnalysis.Rejected(output, candidate, [], "extraction", "Correction output adds line breaks absent from the source.");
        if (candidate == source) return ModelResponseAnalysis.Unchanged(output, candidate);
        if (OnlyWhitespaceChanged(source, candidate)
            && WordToken.Matches(candidate).Count == WordToken.Matches(source).Count
            && SameProtectedContent(source, candidate))
            return ModelResponseAnalysis.Suppressed(output, candidate, "Cosmetic whitespace-only change.");

        try
        {
            if (HasSentenceEnding(source) && !HasSentenceEnding(candidate))
                throw new InvalidDataException("Correction output appears incomplete at the end of the passage.");
            if (!SameSymbols(source, candidate))
                throw new InvalidDataException("Correction output changes a symbol or emoji from the source.");
            ValidateCandidatePassage(source, candidate);
            if (MeaningScopeNeedsReview(source, candidate))
                return ModelResponseAnalysis.NeedsReview(output, candidate, Diff(source, candidate), "Meaning-critical clause scope, quotation, or paragraph structure changed.");
            if (!SameProtectedNames(source, candidate))
                throw new InvalidDataException("Correction output changes a capitalized name or term.");
            if (!OnlyWhitespaceChanged(source, candidate)) EnsurePlausibleCorrection(candidate, source);
            var candidateEdits = Diff(source, candidate);
            var edits = ValidateEdits(candidateEdits, source);
            return ModelResponseAnalysis.Accepted(output, candidate, edits);
        }
        catch (InvalidDataException exception)
        {
            IReadOnlyList<TextEdit> rawEdits = [];
            if (!exception.Message.Contains("incomplete", StringComparison.OrdinalIgnoreCase))
                try { rawEdits = Diff(source, candidate); } catch (InvalidDataException) { }
            return ModelResponseAnalysis.Rejected(output, candidate, rawEdits, "validation", exception.Message);
        }
    }

    /// <summary>Extracts a conservative corrected passage and derives validated UTF-16 edits.</summary>
    public static IReadOnlyList<TextEdit> ParseModelResponse(string output, string source)
    {
        var result = AnalyzeModelResponse(output, source);
        if (result.Disposition is CorrectionDisposition.Rejected or CorrectionDisposition.NeedsReview)
            throw new InvalidDataException(result.RejectionReason);
        return result.Edits;
    }

    private static string ExtractCandidate(string output, string source)
    {
        var sourceLeadingWhitespace = output.StartsWith(LeadingWhitespace(source), StringComparison.Ordinal)
            ? LeadingWhitespace(source) : string.Empty;
        var sourceTrailingWhitespace = output.EndsWith(TrailingWhitespace(source), StringComparison.Ordinal)
            ? TrailingWhitespace(source) : string.Empty;
        var text = output.Trim();
        if (text.Length == 0) throw new InvalidDataException("Correction output is empty.");

        if (text.StartsWith("```", StringComparison.Ordinal))
        {
            var firstLineEnd = text.IndexOf('\n');
            var closingFence = text.LastIndexOf("```", StringComparison.Ordinal);
            if (firstLineEnd < 0 || closingFence <= firstLineEnd)
                throw new InvalidDataException("Correction output contains an incomplete Markdown code fence.");
            text = text[(firstLineEnd + 1)..closingFence].Trim();
        }

        if (!source.Contains('\n'))
        {
            var lines = text.Split('\n').Select(line => line.Trim()).Where(line => line.Length > 0).ToArray();
            if (lines.Length > 1 && HasSentenceEnding(lines[0])
                && lines.Skip(1).Count(line => string.Equals(line, lines[0], StringComparison.Ordinal)) >= 1)
                text = lines[0];
        }

        if (Regex.IsMatch(text, @"^The complete corrected passage(?: with its paragraph breaks preserved)? is:\s*\r?\n", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            text = Regex.Replace(text, @"^The complete corrected passage(?: with its paragraph breaks preserved)? is:\s*\r?\n", "", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).TrimStart();

        // Instruct models sometimes wrap the requested sentence in quotes and then continue
        // with unrelated analysis. Recover the first complete quoted line only when the source
        // itself was not quoted; never strip quotes that belong to the user's passage.
        var firstLine = text.Split('\n', 2)[0].Trim();
        if (source.Length == 0 || source[0] is not ('"' or '\u201c' or '\u2018'))
        {
            var wrapper = firstLine.Length >= 2 && firstLine[0] is '"' or '\u201c' or '\u2018'
                ? firstLine[^1] == (firstLine[0] == '\u201c' ? '\u201d' : firstLine[0] == '\u2018' ? '\u2019' : '"')
                : false;
            if (wrapper) text = firstLine[1..^1];
        }

        // Some chat models append explanations after the requested text. Recognize explicit note
        // markers while preserving blank lines that belong to a multi-paragraph input.
        var noteStart = Regex.Match(text,
            @"(?:^|\r?\n)\s*(?:\*+\s*\(|(?:The\s+)?(?:Explanation|Note|Reasoning|Output(?: text(?:\s*\([^)]*\))?)?|Final output|Input text(?:\s*\([^)]*\))?|Corrected (?:text|output)(?:\s*\([^)]*\))?)\s*:)",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        if (noteStart.Success && noteStart.Index > 0)
            text = text[..noteStart.Index].Trim();

        var responseLines = text.Split('\n').Select(line => line.Trim()).Where(line => line.Length > 0).ToArray();
        var repeatedCompleteLineCount = responseLines.Count(line => string.Equals(line, responseLines[0], StringComparison.Ordinal));
        var endsWithPartialRepeat = responseLines.Length > 1
            && responseLines[^1].Length < responseLines[0].Length
            && responseLines[0].StartsWith(responseLines[^1], StringComparison.Ordinal);
        if (!source.Contains('\n') && responseLines.Length > 1 && repeatedCompleteLineCount >= 2
            && responseLines.Take(responseLines.Length - (endsWithPartialRepeat ? 1 : 0))
                .All(line => string.Equals(line, responseLines[0], StringComparison.Ordinal))
            && (endsWithPartialRepeat || responseLines.All(line => string.Equals(line, responseLines[0], StringComparison.Ordinal))))
            text = responseLines[0];

        if (!source.Contains("**", StringComparison.Ordinal) && !source.Contains("__", StringComparison.Ordinal)
            && !source.Contains('`'))
            text = Regex.Replace(text, @"\*\*(.+?)\*\*|__(.+?)__|`([^`]+)`", match =>
                match.Groups[1].Success ? match.Groups[1].Value : match.Groups[2].Success ? match.Groups[2].Value : match.Groups[3].Value,
                RegexOptions.CultureInvariant);
        text = Regex.Replace(text, @"^(?:Corrected (?:text|passage)(?:\s*\([^)]*\))?|Output(?: text(?:\s*\([^)]*\))?)?|Correction)\s*:\s*", "", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        text = text.Trim();
        if (sourceLeadingWhitespace.Length > 0) text = sourceLeadingWhitespace + text;
        if (sourceTrailingWhitespace.Length > 0) text += sourceTrailingWhitespace;
        var firstOutputLine = text.Split('\n', 2)[0].Trim();
        if ((source.Length == 0 || source[0] is not ('"' or '\u201c' or '\u2018')) && firstOutputLine.Length >= 2
            && (firstOutputLine[0] == '"' && firstOutputLine[^1] == '"'
                || firstOutputLine[0] == '\u201c' && firstOutputLine[^1] == '\u201d'
                || firstOutputLine[0] == '\u2018' && firstOutputLine[^1] == '\u2019'))
            text = firstOutputLine[1..^1];
        if (text.Length == 0 || text.Contains("```", StringComparison.Ordinal)
            || Regex.IsMatch(text, @"(?:^|\n)\s*(?:Explanation|Note|Reasoning|Output(?: text)?|Final output)\s*:", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant)
            || text.StartsWith("Here is", StringComparison.OrdinalIgnoreCase)
            || text.StartsWith("The corrected", StringComparison.OrdinalIgnoreCase)
            || text.StartsWith("The input text", StringComparison.OrdinalIgnoreCase)
            || text.StartsWith("Please provide", StringComparison.OrdinalIgnoreCase)
            || text.StartsWith("Return the complete corrected", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Correction output does not contain a verifiable corrected passage.");
        if (text.Length > Math.Max(8192, source.Length * 2 + 256))
            throw new InvalidDataException("Correction output is too long to validate safely.");
        return text;
    }

    private static string LeadingWhitespace(string value) => new(value.TakeWhile(char.IsWhiteSpace).ToArray());
    private static string TrailingWhitespace(string value) => new(value.Reverse().TakeWhile(char.IsWhiteSpace).Reverse().ToArray());

    private static bool OnlyWhitespaceChanged(string first, string second) =>
        string.Equals(Regex.Replace(first, @"\s+", "", RegexOptions.CultureInvariant),
            Regex.Replace(second, @"\s+", "", RegexOptions.CultureInvariant), StringComparison.Ordinal);

    private static bool SameProtectedContent(string source, string candidate) =>
        SameMatches(NumberToken, source, candidate)
        && SameMatches(UrlToken, source, candidate)
        && SameMatches(EmailToken, source, candidate)
        && SameMatches(PathToken, source, candidate)
        && SameMatches(DomainToken, source, candidate)
        && SameMatches(IpToken, source, candidate)
        && SameMatches(WrittenDateToken, source, candidate)
        && SameMatches(ProtectedIdentifier, source, candidate);

    private static void ValidateCandidatePassage(string source, string candidate)
    {
        if (!SameSymbols(source, candidate))
            throw new InvalidDataException("Correction output changes a symbol or emoji from the source.");
        if (!SameProtectedContent(source, candidate))
            throw new InvalidDataException("Correction output changes protected content such as a number, URL, path, date, or identifier.");
        if (!SameProtectedNames(source, candidate))
            throw new InvalidDataException("Correction output changes a capitalized name or term.");
        if (CriticalMeaningMarkers(source).OrderBy(value => value).SequenceEqual(CriticalMeaningMarkers(candidate).OrderBy(value => value), StringComparer.Ordinal) is false)
            throw new InvalidDataException("Correction output changes negation or modality and requires review.");
        if (source.Count(character => character == '?') != candidate.Count(character => character == '?')
            || source.Count(character => character == '!') != candidate.Count(character => character == '!')
            || TerminalSpeechAct(source) != TerminalSpeechAct(candidate))
            throw new InvalidDataException("Correction output changes the statement/question speech act.");
        EnsureNoIndependentContentOmission(source, candidate);
    }

    private static IEnumerable<string> CriticalMeaningMarkers(string value)
    {
        foreach (Match match in Regex.Matches(value,
            @"(?<![\p{L}\p{N}])(?:can't|can’t|cannot|don't|don’t|doesn't|doesn’t|isn't|isn’t|aren't|aren’t|wasn't|wasn’t|weren't|weren’t|won't|won’t|wouldn't|wouldn’t|shouldn't|shouldn’t|mustn't|mustn’t|needn't|needn’t|never|not|must|may|can|need|should|niet|geen|nooit|moet|moeten|mag|mogen|kan|kunnen|mocht|mochten)(?![\p{L}\p{N}])",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
            yield return match.Value.ToLowerInvariant();
    }

    private static bool MeaningScopeNeedsReview(string source, string candidate)
    {
        if (source.Count(character => character == '\n') != candidate.Count(character => character == '\n')) return true;
        // Preserve marker association with clauses, not just an unordered bag of tokens.
        static string Scope(string text) => string.Join("|", Regex.Split(text,
            @"[.!?;\r\n]+|(?<![\p{L}\p{N}])(?:but|maar|and|en|if|als|unless|tenzij)(?![\p{L}\p{N}])",
            RegexOptions.IgnoreCase | RegexOptions.CultureInvariant).Select(clause => string.Join(",", CriticalMeaningMarkers(clause))));
        if ((CriticalMeaningMarkers(source).Any() || CriticalMeaningMarkers(candidate).Any()) && Scope(source) != Scope(candidate)) return true;
        static IEnumerable<string> Quotes(string text) => Regex.Matches(text, "\"[^\"]*\"|“[^”]*”", RegexOptions.CultureInvariant).Select(match => match.Value);
        return !Quotes(source).SequenceEqual(Quotes(candidate), StringComparer.Ordinal);
    }

    private static char TerminalSpeechAct(string value)
    {
        var trimmed = value.TrimEnd();
        while (trimmed.Length > 0 && trimmed[^1] is '"' or '\'' or '’' or '”' or ')' or ']' or '}')
            trimmed = trimmed[..^1].TrimEnd();
        return trimmed.Length == 0 ? '\0' : trimmed[^1] is '?' or '!' or '.' ? trimmed[^1] : '\0';
    }

    private static void EnsureNoIndependentContentOmission(string source, string candidate)
    {
        var sourceSentences = Regex.Split(source, @"(?<=[.!?…])\s+")
            .Where(sentence => WordToken.Matches(sentence).Count >= 2).ToArray();
        if (sourceSentences.Length < 2) return;
        var candidateWords = WordToken.Matches(candidate).Select(match => match.Value).ToArray();
        foreach (var sentence in sourceSentences)
        {
            var words = WordToken.Matches(sentence).Select(match => match.Value).ToArray();
            var shared = words.Count(word => candidateWords.Contains(word, StringComparer.OrdinalIgnoreCase));
            if (shared == 0 || shared * 2 < words.Length)
                throw new InvalidDataException("Correction output omits an independent source statement.");
        }
    }

    private static bool SameProtectedNames(string source, string candidate) =>
        ProtectedNames(source).SequenceEqual(ProtectedNames(candidate), StringComparer.Ordinal);

    private static IEnumerable<string> ProtectedNames(string value)
    {
        var sentenceInitialFunctionWords = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "I", "you", "he", "she", "it", "we", "they", "the", "a", "an", "this", "that", "these", "those",
            "there", "on", "in", "after", "please", "wait", "team", "tomorrow", "for", "when",
            "ik", "je", "jij", "u", "hij", "zij", "ze", "het", "de", "een", "dit", "dat", "er", "op", "om", "morgen", "als", "we", "wij"
        };
        foreach (Match match in Regex.Matches(value, @"\b\p{Lu}[\p{L}'’.-]*\b", RegexOptions.CultureInvariant))
        {
            var previous = value.AsSpan(0, match.Index).TrimEnd();
            var beginsSentence = previous.Length == 0 || previous[^1] is '.' or '!' or '?' or '…' or '\n' or '\r';
            if (!beginsSentence || !sentenceInitialFunctionWords.Contains(match.Value)) yield return match.Value;
        }
    }

    /// <summary>Diagnostic source alignment only; does not authorize applying the candidate.</summary>
    public static IReadOnlyList<TextEdit> DeriveDiagnosticEdits(string source, string candidate, bool canonicalEquivalence = false)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(candidate);
        return Diff(source, candidate, canonicalEquivalence);
    }

    private static IReadOnlyList<TextEdit> Diff(string source, string candidate, bool canonicalEquivalence = false)
    {
        var sourceStarts = StringInfo.ParseCombiningCharacters(source);
        var candidateStarts = StringInfo.ParseCombiningCharacters(candidate);
        var sourceParts = GetTextElements(source, sourceStarts);
        var candidateParts = GetTextElements(candidate, candidateStarts);
        var a = sourceParts.Select(part => canonicalEquivalence ? part.Value.Normalize(NormalizationForm.FormC) : part.Value).ToArray();
        var b = candidateParts.Select(part => canonicalEquivalence ? part.Value.Normalize(NormalizationForm.FormC) : part.Value).ToArray();
        var operations = MyersDiff(a, b);
        var edits = new List<TextEdit>();
        var ai = 0;
        var bi = 0;
        int? editStart = null;
        var original = new StringBuilder();
        var replacement = new StringBuilder();
        void Flush()
        {
            if (editStart is null) return;
            edits.Add(new TextEdit(editStart.Value, original.Length, original.ToString(), replacement.ToString()));
            editStart = null;
            original.Clear();
            replacement.Clear();
        }

        foreach (var operation in operations)
        {
            if (operation == DiffOperation.Equal)
            {
                Flush(); ai++; bi++;
            }
            else
            {
                editStart ??= ai == sourceParts.Length ? source.Length : sourceParts[ai].Start;
                if (operation == DiffOperation.Delete) { original.Append(sourceParts[ai].Value); ai++; }
                else { replacement.Append(candidateParts[bi].Value); bi++; }
            }
        }
        Flush();
        MergeWordFragments(edits, source, candidate);
        if (canonicalEquivalence
            ? ApplyDiffForVerification(source, edits).Normalize(NormalizationForm.FormC) != candidate.Normalize(NormalizationForm.FormC)
            : ApplyDiffForVerification(source, edits) != candidate)
            throw new InvalidDataException("Correction output could not be represented by a source-aligned diff.");
        return edits;
    }

    private static void MergeWordFragments(List<TextEdit> edits, string source, string candidate)
    {
        for (var index = 1; index < edits.Count;)
        {
            var previous = edits[index - 1];
            var current = edits[index];
            var sourceGapStart = previous.Start + previous.Length;
            var sourceGap = source.AsSpan(sourceGapStart, current.Start - sourceGapStart);
            var candidatePreviousStart = previous.Start + edits.Take(index - 1).Sum(edit => edit.ReplacementText.Length - edit.Length);
            var candidatePreviousEnd = candidatePreviousStart + previous.ReplacementText.Length;
            var candidateCurrentStart = current.Start + edits.Take(index).Sum(edit => edit.ReplacementText.Length - edit.Length);
            var candidateGap = candidate.AsSpan(candidatePreviousEnd, candidateCurrentStart - candidatePreviousEnd);
            if (IsWordGap(sourceGap) && IsWordGap(candidateGap))
            {
                var end = current.Start + current.Length;
                var candidateEnd = candidateCurrentStart + current.ReplacementText.Length;
                edits[index - 1] = new TextEdit(previous.Start, end - previous.Start,
                    source.Substring(previous.Start, end - previous.Start),
                    candidate.Substring(candidatePreviousStart, candidateEnd - candidatePreviousStart));
                edits.RemoveAt(index);
            }
            else index++;
        }
    }

    private static bool IsWordGap(ReadOnlySpan<char> value)
    {
        if (value.IsEmpty) return true;
        foreach (var rune in value.EnumerateRunes())
        {
            var category = Rune.GetUnicodeCategory(rune);
            if (category is not (UnicodeCategory.UppercaseLetter or UnicodeCategory.LowercaseLetter
                or UnicodeCategory.TitlecaseLetter or UnicodeCategory.ModifierLetter or UnicodeCategory.OtherLetter
                or UnicodeCategory.NonSpacingMark or UnicodeCategory.SpacingCombiningMark or UnicodeCategory.DecimalDigitNumber
                or UnicodeCategory.LetterNumber or UnicodeCategory.OtherNumber or UnicodeCategory.ConnectorPunctuation)) return false;
        }
        return true;
    }

    private static string ApplyDiffForVerification(string source, IReadOnlyList<TextEdit> edits)
    {
        var result = new StringBuilder(source);
        foreach (var edit in edits.OrderByDescending(edit => edit.Start))
            result.Remove(edit.Start, edit.Length).Insert(edit.Start, edit.ReplacementText);
        return result.ToString();
    }

    private static (int Start, string Value)[] GetTextElements(string value, int[] starts) => starts
        .Select((start, index) => (start, value.Substring(start, (index + 1 < starts.Length ? starts[index + 1] : value.Length) - start)))
        .ToArray();

    private static DiffOperation[] MyersDiff(string[] a, string[] b)
    {
        var maximum = a.Length + b.Length;
        if (maximum == 0) return [];
        if ((long)a.Length * b.Length <= 4_000_000)
        {
            var lcs = new ushort[a.Length + 1, b.Length + 1];
            for (var i = a.Length - 1; i >= 0; i--)
                for (var j = b.Length - 1; j >= 0; j--)
                    lcs[i, j] = a[i] == b[j] ? (ushort)(lcs[i + 1, j + 1] + 1)
                        : Math.Max(lcs[i + 1, j], lcs[i, j + 1]);
            var operations = new List<DiffOperation>(maximum);
            var ai = 0;
            var bi = 0;
            while (ai < a.Length && bi < b.Length)
            {
                if (a[ai] == b[bi]) { operations.Add(DiffOperation.Equal); ai++; bi++; }
                else if (lcs[ai + 1, bi] >= lcs[ai, bi + 1]) { operations.Add(DiffOperation.Delete); ai++; }
                else { operations.Add(DiffOperation.Insert); bi++; }
            }
            while (ai++ < a.Length) operations.Add(DiffOperation.Delete);
            while (bi++ < b.Length) operations.Add(DiffOperation.Insert);
            return operations.ToArray();
        }
        var offset = maximum;
        var v = new int[maximum * 2 + 3];
        v[offset + 1] = 0;
        var trace = new List<int[]>(Math.Min(maximum, 256));
        var finalDistance = 0;
        var done = false;
        for (var d = 0; d <= maximum && !done; d++)
        {
            for (var k = -d; k <= d; k += 2)
            {
                var index = offset + k;
                var x = k == -d || k != d && v[index - 1] < v[index + 1] ? v[index + 1] : v[index - 1] + 1;
                var y = x - k;
                while (x < a.Length && y < b.Length && a[x] == b[y]) { x++; y++; }
                v[index] = x;
                if (x >= a.Length && y >= b.Length) { finalDistance = d; done = true; break; }
            }
            if ((long)(trace.Count + 1) * v.Length > 4_000_000)
                throw new InvalidDataException("Correction output diff exceeds the safe work limit.");
            trace.Add((int[])v.Clone());
        }

        var result = new List<DiffOperation>(maximum);
        var currentX = a.Length;
        var currentY = b.Length;
        for (var d = finalDistance; d > 0; d--)
        {
            var prior = trace[d - 1];
            var k = currentX - currentY;
            var priorK = k == -d || k != d && prior[offset + k - 1] < prior[offset + k + 1] ? k + 1 : k - 1;
            var priorX = prior[offset + priorK];
            var priorY = priorX - priorK;
            while (currentX > priorX && currentY > priorY) { result.Add(DiffOperation.Equal); currentX--; currentY--; }
            if (currentX == priorX) { result.Add(DiffOperation.Insert); currentY--; }
            else { result.Add(DiffOperation.Delete); currentX--; }
        }
        while (currentX > 0 && currentY > 0) { result.Add(DiffOperation.Equal); currentX--; currentY--; }
        while (currentX-- > 0) result.Add(DiffOperation.Delete);
        while (currentY-- > 0) result.Add(DiffOperation.Insert);
        result.Reverse();
        return result.ToArray();
    }

    private enum DiffOperation { Equal, Delete, Insert }

    private static bool HasSentenceEnding(string value)
    {
        var index = value.TrimEnd().Length - 1;
        while (index >= 0 && value[index] is '\"' or '\'' or '’' or '”' or ')' or ']' or '}') index--;
        return index >= 0 && value[index] is '.' or '!' or '?' or '…';
    }

    private static void EnsurePlausibleCorrection(string candidate, string source)
    {
        var sourceWords = WordToken.Matches(source).Select(match => match.Value).ToArray();
        var candidateWords = WordToken.Matches(candidate).Select(match => match.Value).ToArray();
        if (sourceWords.Length <= 1 && candidateWords.Length == 1)
            return;
        if (sourceWords.Length == 0 || candidateWords.Length == 0
            || candidateWords.Length * 2 < sourceWords.Length
            || candidateWords.Length > sourceWords.Length * 2)
            throw new InvalidDataException("Correction output cannot be verified against the source text.");

        var sourceTerms = sourceWords.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var candidateTerms = candidateWords.ToHashSet(StringComparer.OrdinalIgnoreCase);
        var shared = sourceTerms.Intersect(candidateTerms, StringComparer.OrdinalIgnoreCase).Count();
        if (shared < Math.Min(sourceTerms.Count, candidateTerms.Count) * 0.5)
            throw new InvalidDataException("Correction output changes too much source wording to validate safely.");
    }

    public static IReadOnlyList<TextEdit> ParseAndValidate(string output, string source) => ParseEditData(output, source, validatePassage: true);

    // Untrusted source-aligned candidates for the experimental engine; never bypasses final passage validation.
    internal static IReadOnlyList<TextEdit> ParseDiagnosticEditCandidates(string output, string source) => ParseEditData(output, source, validatePassage: false);

    private static IReadOnlyList<TextEdit> ParseEditData(string output, string source, bool validatePassage)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(source);
        try
        {
            if (output.Length > 65536 || source.Length > 32768) throw new InvalidDataException("Structured edit data exceeds safety limits.");
            using var document = JsonDocument.Parse(output, new JsonDocumentOptions { MaxDepth = 16 });
            if (document.RootElement.ValueKind != JsonValueKind.Array)
                throw new InvalidDataException("Correction output must be a JSON array.");
            if (document.RootElement.GetArrayLength() > 256) throw new InvalidDataException("Too many structured edits.");
            var edits = new List<TextEdit>(document.RootElement.GetArrayLength());
            foreach (var item in document.RootElement.EnumerateArray())
            {
                if (item.ValueKind != JsonValueKind.Object) throw new InvalidDataException("Each edit must be a JSON object.");
                var names = item.EnumerateObject().Select(property => property.Name).ToArray();
                if (names.Length != 4 || names.Distinct(StringComparer.Ordinal).Count() != 4
                    || names.Any(name => name is not ("start" or "length" or "originalText" or "replacementText")))
                    throw new InvalidDataException("Edit contains missing, duplicate, or unknown properties.");
                if (!item.TryGetProperty("start", out var startValue) || startValue.ValueKind != JsonValueKind.Number || !startValue.TryGetInt32(out var start)
                    || !item.TryGetProperty("length", out var lengthValue) || lengthValue.ValueKind != JsonValueKind.Number || !lengthValue.TryGetInt32(out var length))
                    throw new InvalidDataException("Correction output is missing a valid source span.");
                if (start < 0 || length < 0 || start > source.Length - length)
                    throw new InvalidDataException("Correction output contains an out-of-range source span.");
                if (!item.TryGetProperty("originalText", out var originalValue) || originalValue.ValueKind != JsonValueKind.String
                    || !item.TryGetProperty("replacementText", out var replacementValue) || replacementValue.ValueKind != JsonValueKind.String)
                    throw new InvalidDataException("Correction output is missing edit text.");
                var original = originalValue.GetString()!;
                var replacement = replacementValue.GetString()!;
                if (original.Length != length)
                    throw new InvalidDataException("Correction output contains a span length that does not match its source text.");
                edits.Add(new TextEdit(start, length, original, replacement));
            }
            var validated = ValidateEdits(edits, source);
            var candidate = ApplyDiffForVerification(source, validated);
            if (!validatePassage || candidate == source) return validated;
            ValidateCandidatePassage(source, candidate);
            if (MeaningScopeNeedsReview(source, candidate)) throw new InvalidDataException("Meaning-critical clause scope, quotation, or paragraph structure requires review.");
            if (HasSentenceEnding(source) && !HasSentenceEnding(candidate)) throw new InvalidDataException("Correction output appears incomplete.");
            if (!OnlyWhitespaceChanged(source, candidate)) EnsurePlausibleCorrection(candidate, source);
            return validated;
        }
        catch (JsonException exception)
        {
            throw new InvalidDataException("Correction output is not valid JSON edit data.", exception);
        }
    }

    private static IReadOnlyList<TextEdit> ValidateEdits(IEnumerable<TextEdit> edits, string source) => SourceEdits.Validate(source, edits);

    private static bool SameMatches(Regex pattern, string original, string replacement) =>
        pattern.Matches(original).Select(match => match.Value).SequenceEqual(pattern.Matches(replacement).Select(match => match.Value), StringComparer.Ordinal);

    private static bool SameSymbols(string original, string replacement) =>
        ProtectedSymbols(original).SequenceEqual(ProtectedSymbols(replacement), StringComparer.Ordinal);

    private static IEnumerable<string> ProtectedSymbols(string value)
    {
        foreach (var rune in value.EnumerateRunes())
            if (Rune.GetUnicodeCategory(rune) is UnicodeCategory.MathSymbol or UnicodeCategory.CurrencySymbol
                or UnicodeCategory.ModifierSymbol or UnicodeCategory.OtherSymbol)
                yield return rune.ToString();
    }

}

public sealed record ModelResponseAnalysis(string RawOutput, string? ExtractedCorrection,
    IReadOnlyList<TextEdit> Edits, IReadOnlyList<TextEdit> RawCandidateEdits, CorrectionDisposition Disposition,
    string? RejectionStage, string? RejectionReason)
{
    public static ModelResponseAnalysis Accepted(string raw, string correction, IReadOnlyList<TextEdit> edits) =>
        new(raw, correction, edits, edits, CorrectionDisposition.Accepted, null, null);
    public static ModelResponseAnalysis Unchanged(string raw, string correction) =>
        new(raw, correction, [], [], CorrectionDisposition.Unchanged, null, null);
    public static ModelResponseAnalysis Suppressed(string raw, string correction, string reason) =>
        new(raw, correction, [], [], CorrectionDisposition.Suppressed, "normalization", reason);
    public static ModelResponseAnalysis NeedsReview(string raw, string correction, IReadOnlyList<TextEdit> rawEdits, string reason) =>
        new(raw, correction, [], rawEdits, CorrectionDisposition.NeedsReview, "meaning-review", reason);
    public static ModelResponseAnalysis Rejected(string raw, string? correction, IReadOnlyList<TextEdit> rawEdits, string stage, string reason) =>
        new(raw, correction, [], rawEdits, CorrectionDisposition.Rejected, stage, reason);
}
