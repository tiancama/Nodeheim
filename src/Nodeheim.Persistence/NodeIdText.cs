namespace Nodeheim.Persistence;

/// <summary>
/// Represents the text form of node IDs in a document.
/// </summary>
/// <remarks>
/// Format contract: Node IDs are written as GUIDs in format D with lowercase letters
/// and compared ordinally by that text.
/// </remarks>
internal static class NodeIdText
{
    /// <summary>
    /// Gets the comparer for node ID texts.
    /// </summary>
    /// <remarks>
    /// Format contract: Node IDs are sorted ordinally by their written text, not by
    /// <see cref="Guid.CompareTo(Guid)"/>.
    /// </remarks>
    internal static StringComparer Comparer => StringComparer.Ordinal;

    /// <summary>
    /// Formats a node ID as it is written to a document.
    /// </summary>
    /// <param name="id">The node ID.</param>
    /// <returns>The node ID in format D with lowercase letters.</returns>
    internal static string Format(Guid id) => id.ToString("D").ToLowerInvariant();
}
