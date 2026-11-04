using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a receiver that builds a document tree from the reported entries.
/// </summary>
public sealed class DocumentEntryBuilder : IDocumentEntryWriter
{
    private DocumentEntry? _root;
    private readonly IIssueCollector _issues;
    private readonly Stack<Frame> _frames = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentEntryBuilder"/> class.
    /// </summary>
    /// <param name="issues">The collector that receives the issues found while building.</param>
    /// <exception cref="ArgumentNullException"><paramref name="issues"/> is <see langword="null"/>.</exception>
    public DocumentEntryBuilder(IIssueCollector issues)
    {
        ArgumentNullException.ThrowIfNull(issues);

        _issues = issues;
    }

    /// <inheritdoc />
    public void BeginEntry(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        Frame frame = new(name);
        _frames.Push(frame);
    }

    /// <inheritdoc />
    public void Text(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _frames.Peek().Text = text;
    }

    /// <inheritdoc />
    public void EndEntry()
    {
        Frame frame = _frames.Pop();

        DocumentEntry entry = frame.Children.Count > 0
            ? DocumentEntry.CreateBranch(frame.Name, frame.Children)
            : DocumentEntry.CreateLeaf(frame.Name, frame.Text ?? string.Empty);

        if (_frames.TryPeek(out Frame? parent))
        {
            parent.Children.Add(entry);
        }
        else
        {
            _root = entry;
        }
    }

    /// <summary>
    /// Returns the root entry.
    /// </summary>
    /// <returns>The root entry, or <see langword="null"/> if no root entry has been completed.</returns>
    public DocumentEntry? GetResult() => _root;

    private sealed class Frame
    {
        public Frame(string name)
        {
            Name = name;
        }

        public List<DocumentEntry> Children { get; } = new();

        public string Name { get; }
        public string? Text { get; set; }
    }
}
