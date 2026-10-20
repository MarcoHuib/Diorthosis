using System.Buffers;
using System.Globalization;
using System.Text;

namespace Diorthosis.Core.Corrections;

/// <summary>Literal UTF-16 source alignment. No normalization or linguistic validation.</summary>
public static class SourceEdits
{
    public static IReadOnlyList<TextEdit> Validate(string source, IEnumerable<TextEdit> edits, bool allowNoOps = false)
    {
        ArgumentNullException.ThrowIfNull(source);
        ArgumentNullException.ThrowIfNull(edits);
        EnsureUnicode(source);
        var boundaries = StringInfo.ParseCombiningCharacters(source).Append(source.Length).ToHashSet();
        var ordered = edits.OrderBy(edit => edit.Start).ThenBy(edit => edit.Length).ToArray();
        foreach (var edit in ordered)
        {
            if (edit.Start < 0 || edit.Length < 0 || edit.Start > source.Length - edit.Length
                || !source.AsSpan(edit.Start, edit.Length).SequenceEqual(edit.OriginalText))
                throw new InvalidDataException("Edit does not align with the UTF-16 source span.");
            if (!boundaries.Contains(edit.Start) || !boundaries.Contains(edit.Start + edit.Length))
                throw new InvalidDataException("Edit splits a Unicode grapheme boundary.");
            EnsureUnicode(edit.ReplacementText);
            if (!allowNoOps && edit.OriginalText == edit.ReplacementText) throw new InvalidDataException("Edit is unchanged.");
        }
        for (var i = 1; i < ordered.Length; i++)
            if (ordered[i].Start < ordered[i - 1].Start + ordered[i - 1].Length
                || ordered[i].Start == ordered[i - 1].Start && (ordered[i].Length == 0 || ordered[i - 1].Length == 0))
                throw new InvalidDataException("Edits overlap or contain ambiguous insertions.");
        return ordered;
    }

    public static string Apply(string source, IEnumerable<TextEdit> edits)
    {
        var ordered = Validate(source, edits, allowNoOps: true);
        var result = new StringBuilder(source);
        foreach (var edit in ordered.Reverse()) result.Remove(edit.Start, edit.Length).Insert(edit.Start, edit.ReplacementText);
        return result.ToString();
    }

    public static void EnsureUnicode(string value)
    {
        var remaining = value.AsSpan();
        while (!remaining.IsEmpty)
        {
            if (Rune.DecodeFromUtf16(remaining, out _, out var consumed) != OperationStatus.Done)
                throw new InvalidDataException("Text contains malformed UTF-16.");
            remaining = remaining[consumed..];
        }
    }
}
