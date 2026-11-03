namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Defines a receiver for the entries of a document tree, reported in document order.
/// </summary>
/// <remarks>
/// An entry is reported as <see cref="BeginEntry"/>, followed by its text or its children, and the matching
/// <see cref="EndEntry"/>. A caller reports exactly one root entry.
/// </remarks>
public interface IDocumentEntryWriter
{
    /// <summary>
    /// Reports the beginning of an entry.
    /// </summary>
    /// <param name="name">The name.</param>
    void BeginEntry(string name);

    /// <summary>
    /// Reports the text of the innermost open entry.
    /// </summary>
    /// <param name="text">The text.</param>
    void Text(string text);

    /// <summary>
    /// Reports the end of the innermost open entry.
    /// </summary>s
    void EndEntry();
}
