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
    /// A text contains a character that XML 1.0 does not permit.
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
}
