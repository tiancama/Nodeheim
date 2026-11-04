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

        if (_root is not null)
            throw new InvalidOperationException("The root entry is already complete.");

        Frame frame = new(name);
        _frames.Push(frame);
    }

    /// <inheritdoc />
    public void Text(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (!_frames.TryPeek(out Frame? frame))
            throw new InvalidOperationException("No entry is open.");

        if (frame.Text is not null)
            throw new InvalidOperationException("The open entry already has text.");

        frame.Text = text;
    }

    /// <inheritdoc />
    public void EndEntry()
    {
        if (!_frames.TryPop(out Frame? frame))
            throw new InvalidOperationException("No entry is open.");

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
    /// <returns>The root entry.</returns>
    /// <exception cref="InvalidOperationException">The root entry is not complete.</exception>
    public DocumentEntry GetResult()
    {
        if (_root is null)
            throw new InvalidOperationException("The root entry is not complete.");

        return _root;
    }

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
