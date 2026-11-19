using System.Buffers;
using System.Collections.ObjectModel;
using System.Xml;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents an entry of the document tree, either a leaf or a branch.
/// </summary>
public sealed class DocumentEntry
{
    /// <summary>
    /// Represents the maximum number of levels of a document tree.
    /// </summary>
    public const int MaxLevels = 32;

    private static readonly SearchValues<char> NameCharacters =
        SearchValues.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.-_");

    private readonly int _height;

    private DocumentEntry(string name, string text, IReadOnlyList<DocumentEntry> children)
    {
        ArgumentNullException.ThrowIfNull(name);
        if (!IsValidName(name))
            throw new ArgumentException("The name is not valid.", nameof(name));

        ArgumentNullException.ThrowIfNull(text);

        if (!IsValidText(text))
            throw new ArgumentException("The text contains a character that XML 1.0 does not permit.", nameof(text));

        int height = 1;
        foreach (DocumentEntry child in children)
        {
            height = Math.Max(height, child._height + 1);
        }

        if (height > MaxLevels)
            throw new ArgumentException($"The entry has more than {MaxLevels} levels.", nameof(children));

        Name = name;
        Text = text;
        Children = children;
        _height = height;
    }

    /// <summary>
    /// Gets the name.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the text of a leaf.
    /// </summary>
    public string Text { get; }

    /// <summary>
    /// Gets the children of a branch.
    /// </summary>
    public IReadOnlyList<DocumentEntry> Children { get; }

    /// <summary>
    /// Creates a leaf.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="text">The text.</param>
    /// <returns>The leaf.</returns>
    /// <exception cref="ArgumentException"><paramref name="name"/> is <see langword="null"/> or empty.</exception>

    public static DocumentEntry CreateLeaf(string name, string text)
    {
        return new DocumentEntry(name, text, ReadOnlyCollection<DocumentEntry>.Empty);
    }

    /// <summary>
    /// Creates a branch.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <param name="children">The children; at least one.</param>
    /// <returns>The branch.</returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="name"/> is <see langword="null"/> or empty, or <paramref name="children"/> is
    /// <see langword="null"/>, empty, or contains <see langword="null"/>, or the branch would have more than
    /// <see cref="MaxLevels"/> levels.
    /// </exception>
    public static DocumentEntry CreateBranch(string name, IEnumerable<DocumentEntry> children)
    {
        ArgumentNullException.ThrowIfNull(children);

        DocumentEntry[] copy = children.ToArray();
        if (copy.Length == 0)
            throw new ArgumentException("A branch requires at least one child.", nameof(children));

        if (copy.Contains(null))
            throw new ArgumentException("The children must not contain null.", nameof(children));

        return new DocumentEntry(name, string.Empty, Array.AsReadOnly(copy));
    }

    internal static bool IsValidName(string? name)
    {
        if (string.IsNullOrEmpty(name)) return false;
        if (name.AsSpan().ContainsAnyExcept(NameCharacters)) return false;
        if (!(char.IsAsciiLetter(name[0]) || name[0] == '_')) return false;
        return !name.StartsWith("xml", StringComparison.OrdinalIgnoreCase);
    }

    internal static bool IsValidText(string text)
    {
        for (int i = 0; i < text.Length; i++)
        {
            if (XmlConvert.IsXmlChar(text[i]))
                continue;

            if (i + 1 < text.Length && XmlConvert.IsXmlSurrogatePair(text[i + 1], text[i]))
            {
                i++;
                continue;
            }

            return false;
        }

        return true;
    }
}
