using Nodeheim.Persistence.Documents.Adapters;

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
    private readonly DocumentWriterProtocol _protocol = new();
    private readonly Stack<Frame> _frames = new();
    private DocumentEntry? _root;

    /// <inheritdoc />
    public void Name(string name) => _protocol.Name(name);

    /// <inheritdoc />
    public void Text(string text)
    {
        string name = _protocol.Text();
        _frames.Peek().Children.Add(DocumentEntry.CreateLeaf(name, text));
    }

    /// <inheritdoc />
    public void BeginGroup() => _frames.Push(new Frame(_protocol.BeginGroup()));

    /// <inheritdoc />
    public void EndGroup()
    {
        _protocol.EndGroup();

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
    public void BeginRepeated() => _protocol.BeginRepeated();

    /// <inheritdoc />
    public void EndRepeated() => _protocol.EndRepeated();

    /// <summary>
    /// Returns the root entry.
    /// </summary>
    /// <returns>The root entry.</returns>
    /// <exception cref="InvalidOperationException">The document tree is not complete.</exception>
    public DocumentEntry GetResult() =>
        _root ?? throw new InvalidOperationException("The document tree is not complete.");

    private sealed class Frame
    {
        public Frame(string name)
        {
            Name = name;
        }

        public List<DocumentEntry> Children { get; } = new();

        public string Name { get; }
    }
}
