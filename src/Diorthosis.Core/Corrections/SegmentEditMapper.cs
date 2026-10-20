namespace Diorthosis.Core.Corrections;

/// <summary>Source mapping only; callers must separately authorize correction proposals.</summary>
public static class SegmentEditMapper
{
    public static IReadOnlyList<TextEdit> Map(string documentText, int segmentStart, string exactSegmentText, IReadOnlyList<TextEdit> localEdits)
    {
        ArgumentNullException.ThrowIfNull(exactSegmentText);
        // Validate both target edges against literal document text and Unicode graphemes.
        SourceEdits.Validate(documentText, [new TextEdit(segmentStart, exactSegmentText.Length, exactSegmentText, exactSegmentText)], allowNoOps: true);
        var edits = SourceEdits.Validate(exactSegmentText, localEdits);
        var mapped = edits.Select(edit => new TextEdit(checked(segmentStart + edit.Start), edit.Length, edit.OriginalText, edit.ReplacementText)).ToArray();
        return SourceEdits.Validate(documentText, mapped);
    }
}
