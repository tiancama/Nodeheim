namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Specifies the kind of rule a document violates.
/// </summary>
/// <remarks>
/// New kinds are added as the value model grows. Code that switches over this enumeration
/// should handle unrecognized values in its default case.
/// </remarks>
public enum DocumentRejectionKind
{
    /// <summary>
    /// A text value, a reference value, or a key contains a character that is not permitted.
    /// </summary>
    InvalidCharacter,

    /// <summary>
    /// A reference is empty.
    /// </summary>
    EmptyReference,

    /// <summary>
    /// A number cannot be represented in the value model, such as not a number (NaN) or infinity.
    /// </summary>
    NumberNotRepresentable,

    /// <summary>
    /// A value exceeds the maximum nesting depth, <see cref="DocumentValue.MaxNestingDepth"/>.
    /// </summary>
    MaxNestingDepthExceeded,

    /// <summary>
    /// A compound contains the same key more than once.
    /// </summary>
    /// <remarks>
    /// Format contract: Keys are compared ordinally, so keys that look alike but are encoded differently are not
    /// duplicates.
    /// </remarks>
    DuplicateKey,

    /// <summary>
    /// The document does not conform to the syntax of its format, such as JSON or XML.
    /// </summary>
    InvalidSyntax,

    /// <summary>
    /// The input is not marked as a Nodeheim document.
    /// </summary>
    NotNodeheimDocument,

    /// <summary>
    /// The document has a format version that is not supported.
    /// </summary>
    UnsupportedFormatVersion,

    /// <summary>
    /// The structure of the document does not match its format, such as a missing or unexpected part.
    /// </summary>
    InvalidStructure,
}
