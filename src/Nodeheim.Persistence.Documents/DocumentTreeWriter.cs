namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a writer that builds a document tree from the write calls.
/// </summary>
/// <remarks>
/// Format contract: A group becomes an entry with its values as children, each value of a repeated field
/// becomes a child with the name of the field, and a text becomes a leaf. A group without children becomes
/// a leaf with the text <see cref="string.Empty"/>, as on load.
/// </remarks>
public sealed class DocumentTreeWriter : IDocumentWriter
{
    private readonly Stack<Frame> _frames = new();
    private string? _pendingName;
    private DocumentEntry? _root;

    /// <inheritdoc />
    public void Name(string name) => _pendingName = name;

    /// <inheritdoc />
    public void Text(string text) =>
        _frames.Peek().Children.Add(DocumentEntry.CreateLeaf(TakeName(), text));

    /// <inheritdoc />
    public void BeginGroup() => _frames.Push(new Frame(TakeName()));

    /// <inheritdoc />
    public void EndGroup()
    {
        Frame frame = _frames.Pop();
        DocumentEntry entry = DocumentEntry.CreateBranchOrEmpty(frame.Name, frame.Children);

        if (_frames.TryPeek(out Frame? parent))
        {
            parent.Children.Add(entry);
        }
        else
        {
            _root = entry;
        }
    }

    /// <inheritdoc />
    public void BeginRepeated()
    {
        _frames.Peek().RepeatedName = _pendingName;
        _pendingName = null;
    }

    /// <inheritdoc />
    public void EndRepeated() => _frames.Peek().RepeatedName = null;

    /// <summary>
    /// Returns the root entry.
    /// </summary>
    /// <returns>The root entry.</returns>
    /// <exception cref="InvalidOperationException">The document tree is not complete.</exception>
    public DocumentEntry GetResult() =>
        _root ?? throw new InvalidOperationException("The document tree is not complete.");

    private string TakeName()
    {
        if (_frames.TryPeek(out Frame? frame) && frame.RepeatedName is not null)
            return frame.RepeatedName;

        string name = _pendingName ?? throw new InvalidOperationException("No name is pending.");
        _pendingName = null;
        return name;
    }

    private sealed class Frame
    {
        public Frame(string name)
        {
            Name = name;
        }

        public List<DocumentEntry> Children { get; } = new();

        public string Name { get; }

        public string? RepeatedName { get; set; }
    }
}
