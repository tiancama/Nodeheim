namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Defines a port that writes a document as a sequence of names and values.
/// </summary>
/// <remarks>
/// A value is a text, a group, or a repeated field. In a group, each value is preceded by
/// <see cref="Name"/>; the values of a repeated field have no name and are texts or groups.
/// A caller writes exactly one root: <see cref="Name"/> followed by a group.
/// </remarks>
public interface IDocumentWriter
{
    /// <summary>
    /// Writes the name of the next value.
    /// </summary>
    /// <param name="name">The name.</param>
    void Name(string name);

    /// <summary>
    /// Writes a text value.
    /// </summary>
    /// <param name="text">The text.</param>
    void Text(string text);

    /// <summary>
    /// Writes the beginning of a group.
    /// </summary>
    void BeginGroup();

    /// <summary>
    /// Writes the end of the innermost open group.
    /// </summary>
    void EndGroup();

    /// <summary>
    /// Writes the beginning of a repeated field.
    /// </summary>
    void BeginRepeated();

    /// <summary>
    /// Writes the end of the innermost open repeated field.
    /// </summary>
    void EndRepeated();
}
