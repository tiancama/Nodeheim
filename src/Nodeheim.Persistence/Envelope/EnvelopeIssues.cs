using Nodeheim.Issues;

namespace Nodeheim.Persistence;

/// <summary>
/// Provides the definitions of the issues that concern the envelope of a document.
/// </summary>
internal static class EnvelopeIssues
{
    /// <summary>
    /// Represents the discrepancy that a document contains a section of an unknown section type.
    /// </summary>
    /// <remarks>
    /// The section is skipped; the rest of the document is still loaded.
    /// </remarks>
    internal static readonly IssueDefinition UnknownSection =
        new("format.unknownSection", IssueKind.Discrepancy);

    /// <summary>
    /// Represents the rejection that the document has a format version that is not supported.
    /// </summary>
    internal static readonly IssueDefinition UnsupportedFormatVersion =
        new("format.unsupportedFormatVersion", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that the structure of the document does not match the format, such as a missing,
    /// repeated, or unexpected entry, or a branch where a text is expected.
    /// </summary>
    internal static readonly IssueDefinition InvalidStructure =
        new("format.invalidStructure", IssueKind.Rejection);
}
