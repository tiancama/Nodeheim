namespace Nodeheim.Persistence.Documents;


/// <summary>
/// Represents a rule violation found in a document.
/// </summary>
/// <param name="Kind">The kind of rule that is violated.</param>
/// <param name="Message">A description of the violation.</param>
/// <remarks>
/// The message is intended for developers and logs.
/// Invalid characters appear as <c>U+XXXX</c> together with their UTF-16 index, never as text from the document.
/// </remarks>
public sealed record DocumentError(DocumentErrorKind Kind, string Message);
