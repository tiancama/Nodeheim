using System.Buffers;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Provides methods for reading the entries of a document tree.
/// </summary>
/// <remarks>
/// The methods report a mismatch only through their return value; the caller decides whether and how to report it.
/// </remarks>
public static class DocumentEntryReader
{
    private static readonly SearchValues<char> Whitespace = SearchValues.Create(" \t\n\r");

    /// <summary>
    /// Gets the only child with the specified name.
    /// </summary>
    /// <param name="entry">The entry whose children are searched.</param>
    /// <param name="name">The name of the child.</param>
    /// <returns>
    /// The child, or <see langword="null"/> if <paramref name="entry"/> has no child or more than one child with
    /// the name <paramref name="name"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="entry"/> or <paramref name="name"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// Names are compared ordinally.
    /// </remarks>
    public static DocumentEntry? GetChild(DocumentEntry entry, string name)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(name);

        DocumentEntry? found = null;

        foreach (DocumentEntry child in entry.Children)
        {
            if (child.Name != name)
                continue;

            if (found is not null)
                return null;

            found = child;
        }

        return found;
    }

    /// <summary>
    /// Gets the children with the specified name, in their order.
    /// </summary>
    /// <param name="entry">The entry whose children are read.</param>
    /// <param name="name">The name of the children.</param>
    /// <returns>
    /// The children with the name <paramref name="name"/>, possibly none; or <see langword="null"/> if
    /// <paramref name="entry"/> is a leaf whose text is not only whitespace.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="entry"/> or <paramref name="name"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// Names are compared ordinally. Children with other names are skipped.
    /// </para>
    /// <para>
    /// Format contract: Where children are expected, a leaf whose text is empty or consists only of whitespace
    /// (space, tab, line feed, carriage return) has no children, so that indentation never makes a document
    /// invalid.
    /// </para>
    /// </remarks>
    public static IReadOnlyList<DocumentEntry>? GetChildren(DocumentEntry entry, string name)
    {
        ArgumentNullException.ThrowIfNull(entry);
        ArgumentNullException.ThrowIfNull(name);

        if (entry.Children.Count > 0)
            return entry.Children.Where(child => child.Name == name).ToArray();

        if (entry.Text.AsSpan().ContainsAnyExcept(Whitespace))
            return null;

        return [];
    }

    /// <summary>
    /// Gets the text of a leaf.
    /// </summary>
    /// <param name="entry">The entry to read.</param>
    /// <returns>
    /// The text, unchanged; or <see langword="null"/> if <paramref name="entry"/> is a branch.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="entry"/> is <see langword="null"/>.</exception>
    public static string? GetText(DocumentEntry entry)
    {
        ArgumentNullException.ThrowIfNull(entry);

        return entry.Children.Count == 0 ? entry.Text : null;
    }
}
