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

    /// <summary>
    /// Represents the rejection that the input does not conform to the syntax of its format, such as JSON or XML.
    /// </summary>
    public static readonly IssueDefinition InvalidSyntax = new("format.invalidSyntax", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that a text value, a reference value, or a key contains a character that is not
    /// permitted, or that a key is empty.
    /// </summary>
    public static readonly IssueDefinition InvalidCharacter = new("format.invalidCharacter", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that a reference value is empty.
    /// </summary>
    public static readonly IssueDefinition EmptyReference = new("format.emptyReference", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that a number cannot be represented in the value model, such as an integer outside
    /// the permitted range, not a number (NaN), or infinity.
    /// </summary>
    public static readonly IssueDefinition NumberNotRepresentable =
        new("format.numberNotRepresentable", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that a value exceeds the maximum nesting depth,
    /// <see cref="DocumentValue.MaxNestingDepth"/>.
    /// </summary>
    public static readonly IssueDefinition MaxNestingDepthExceeded =
        new("format.maxNestingDepthExceeded", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that a compound contains the same key more than once.
    /// </summary>
    public static readonly IssueDefinition DuplicateKey = new("format.duplicateKey", IssueKind.Rejection);
}
