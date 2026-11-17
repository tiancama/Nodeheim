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
    /// Represents the rejection that a text contains a character that is not permitted, such as a character that
    /// XML 1.0 does not permit.
    /// </summary>
    public static readonly IssueDefinition InvalidCharacter = new("format.invalidCharacter", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that the document is nested too deeply.
    /// </summary>
    public static readonly IssueDefinition MaxLevelsExceeded =
        new("format.maxLevelsExceeded", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that a JSON object contains the same name more than once.
    /// </summary>
    public static readonly IssueDefinition DuplicateKey = new("format.duplicateKey", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that an element of an XML document contains both child elements and text that is
    /// not only whitespace.
    /// </summary>
    public static readonly IssueDefinition MixedContent = new("format.mixedContent", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that a JSON array cannot be mapped to the document tree, because it is not the value
    /// of a property.
    /// </summary>
    public static readonly IssueDefinition UnmappableArray = new("format.unmappableArray", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that the input is not a Nodeheim document, because the root entry is not named
    /// <c>nodeheim</c> or has no single child named <c>nodeheimFormat</c>, or because the root element of an XML
    /// document is not in the Nodeheim namespace.
    /// </summary>
    public static readonly IssueDefinition NotNodeheimDocument = new("format.notNodeheimDocument", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that an element below the root element of an XML document is in a foreign namespace
    /// or in no namespace.
    /// </summary>
    public static readonly IssueDefinition ForeignElement = new("format.foreignElement", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that the bytes of a document are not valid UTF-8, or that the XML declaration
    /// names an encoding other than UTF-8.
    /// </summary>
    public static readonly IssueDefinition InvalidEncoding = new("format.invalidEncoding", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that the document has a format version that is not supported.
    /// </summary>
    public static readonly IssueDefinition UnsupportedFormatVersion =
        new("format.unsupportedFormatVersion", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that the structure of the document does not match the format, such as a missing,
    /// repeated, or unexpected entry, or a branch where a text is expected.
    /// </summary>
    public static readonly IssueDefinition InvalidStructure =
        new("format.invalidStructure", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that the name of an entry is not permitted.
    /// </summary>
    public static readonly IssueDefinition InvalidEntryName =
        new("format.invalidEntryName", IssueKind.Rejection);
}
