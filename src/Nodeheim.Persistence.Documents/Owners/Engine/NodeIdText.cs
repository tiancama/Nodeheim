namespace Nodeheim.Persistence.Documents.Owners.Engine;

/// <summary>
/// Represents the text form of node IDs in a document.
/// </summary>
/// <remarks>
/// This type belongs to the vocabulary of the owner <c>engine</c>. It is part of the persistence contract only so
/// that every owner that refers to nodes reads, writes and sorts node IDs in the same way.
/// Format contract: Node IDs are written as GUIDs in format D with lowercase letters and compared ordinally by
/// that text. When read, any format that <see cref="Guid.TryParse(string?, out Guid)"/> accepts is valid.
/// </remarks>
public static class NodeIdText
{
    /// <summary>
    /// Gets the comparer for node ID texts.
    /// </summary>
    /// <remarks>
    /// Format contract: Node IDs are sorted ordinally by their written text, not by
    /// <see cref="Guid.CompareTo(Guid)"/>.
    /// </remarks>
    public static StringComparer Comparer => StringComparer.Ordinal;

    /// <summary>
    /// Formats a node ID as it is written to a document.
    /// </summary>
    /// <param name="id">The node ID.</param>
    /// <returns>The node ID in format D with lowercase letters.</returns>
    public static string Format(Guid id) => id.ToString("D").ToLowerInvariant();

    /// <summary>
    /// Tries to parse a node ID from its text in a document.
    /// </summary>
    /// <param name="text">The text to parse.</param>
    /// <param name="id">
    /// The node ID if <paramref name="text"/> is valid; otherwise, <see cref="Guid.Empty"/>.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="text"/> is a valid node ID; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryParse(string? text, out Guid id) => Guid.TryParse(text, out id);
}
