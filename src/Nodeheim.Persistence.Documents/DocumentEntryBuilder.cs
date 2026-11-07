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
    private readonly HashSet<string> _names = new(StringComparer.Ordinal);
    private bool _hasFailed;

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

        if (_hasFailed) return;

        if (_root is not null)
            throw new InvalidOperationException("The root entry is already complete.");

        if (_frames.TryPeek(out Frame? parent) && parent.Text is not null)
            throw new InvalidOperationException("The open entry already has text.");

        if (!DocumentEntry.IsValidName(name))
        {
            Fail(FormatIssues.InvalidEntryName, "An entry name is not valid.");
            return;
        }

        int depth = _frames.Count;

        if (depth >= DocumentEntry.MaxLevels)
        {
            Fail(FormatIssues.MaxLevelsExceeded,
                $"The document tree has more than {DocumentEntry.MaxLevels} levels.");
            return;
        }

        if (!_names.TryGetValue(name, out string? shared))
        {
            _names.Add(name);
            shared = name;
        }

        _frames.Push(new Frame(shared));
    }

    /// <inheritdoc />
    public void Text(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (_hasFailed) return;

        if (!_frames.TryPeek(out Frame? frame))
            throw new InvalidOperationException("No entry is open.");

        if (frame.Text is not null)
            throw new InvalidOperationException("The open entry already has text.");

        if (frame.Children.Count > 0)
            throw new InvalidOperationException("The open entry already has children.");

        if (!DocumentEntry.IsValidText(text))
        {
            Fail(FormatIssues.InvalidCharacter,
                "A text contains a character that XML 1.0 does not permit.");
            return;
        }

        frame.Text = text;
    }

    /// <inheritdoc />
    public void EndEntry()
    {
        if (_hasFailed) return;

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
    /// <returns>
    /// The root entry, or <see langword="null"/> if the document tree is rejected.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The document tree is neither complete nor rejected.
    /// </exception>
    public DocumentEntry? GetResult()
    {
        if (_hasFailed) return null;

        if (_root is null)
            throw new InvalidOperationException("The root entry is not complete.");

        return _root;
    }

    private void Fail(IssueDefinition definition, string message)
    {
        _issues.Add(new Issue(definition, message, null));
        _hasFailed = true;
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
