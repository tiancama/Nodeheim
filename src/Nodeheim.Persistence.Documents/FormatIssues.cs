using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Provides the definitions of the issues that concern the format itself rather than the content of a section.
/// </summary>
public static class FormatIssues
{
    /// <summary>
    /// Represents the discrepancy that a document contains a section of an unknown section type.
    /// </summary>
    /// <remarks>
    /// The section is skipped; the rest of the document is still loaded.
    /// </remarks>
    public static readonly IssueDefinition UnknownSection = new("format.unknownSection", IssueKind.Discrepancy);
}
