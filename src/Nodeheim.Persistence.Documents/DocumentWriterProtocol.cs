namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents the protocol of <see cref="IDocumentWriter"/>: it tracks the write calls, determines the name
/// of each value, and rejects calls in an invalid order.
/// </summary>
/// <remarks>
/// A writer forwards each of its calls to the method of the same name. The protocol checks only the order of
/// the calls; it does not check names, texts, or levels.
/// </remarks>
public sealed class DocumentWriterProtocol
{
    private readonly Stack<Group> _groups = new();
    private string? _pendingName;
    private bool _isComplete;

    /// <summary>
    /// Gets the number of open groups.
    /// </summary>
    public int OpenGroupCount => _groups.Count;

    /// <summary>
    /// Records a call to <see cref="IDocumentWriter.Name"/>.
    /// </summary>
    /// <param name="name">The name.</param>
    /// <exception cref="ArgumentNullException"><paramref name="name"/> is <see langword="null"/>.</exception>
    /// <exception cref="InvalidOperationException">
    /// The document is complete, a name is already pending, or a repeated field is open in the innermost group.
    /// </exception>
    public void Name(string name)
    {
        ArgumentNullException.ThrowIfNull(name);
        EnsureNotComplete();

        if (_pendingName is not null)
            throw new InvalidOperationException("A name is already pending.");

        if (_groups.TryPeek(out Group? group) && group.RepeatedName is not null)
            throw new InvalidOperationException("The values of a repeated field have no name.");

        _pendingName = name;
    }

    /// <summary>
    /// Records a call to <see cref="IDocumentWriter.Text"/>.
    /// </summary>
    /// <returns>The name of the text value.</returns>
    /// <exception cref="InvalidOperationException">
    /// The document is complete, no group is open, or no name applies to the value.
    /// </exception>
    public string Text()
    {
        EnsureNotComplete();

        if (_groups.Count == 0)
            throw new InvalidOperationException("The root must be a group.");

        return TakeName();
    }

    /// <summary>
    /// Records a call to <see cref="IDocumentWriter.BeginGroup"/>.
    /// </summary>
    /// <returns>The name of the group.</returns>
    /// <exception cref="InvalidOperationException">
    /// The document is complete, or no name applies to the value.
    /// </exception>
    public string BeginGroup()
    {
        EnsureNotComplete();

        string name = TakeName();
        _groups.Push(new Group());
        return name;
    }

    /// <summary>
    /// Records a call to <see cref="IDocumentWriter.EndGroup"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// No group is open, a name is pending, or a repeated field is open in the innermost group.
    /// </exception>
    /// <remarks>
    /// Closing the root group completes the document; every later call throws.
    /// </remarks>
    public void EndGroup()
    {
        Group group = PeekGroup();

        if (_pendingName is not null)
            throw new InvalidOperationException("A name is pending without a value.");

        if (group.RepeatedName is not null)
            throw new InvalidOperationException("A repeated field is still open.");

        _groups.Pop();

        if (_groups.Count == 0)
            _isComplete = true;
    }

    /// <summary>
    /// Records a call to <see cref="IDocumentWriter.BeginRepeated"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// No group is open, a repeated field is already open in the innermost group, or no name is pending.
    /// </exception>
    public void BeginRepeated()
    {
        Group group = PeekGroup();

        if (group.RepeatedName is not null)
            throw new InvalidOperationException("A repeated field must not directly contain a repeated field.");

        group.RepeatedName = _pendingName ?? throw new InvalidOperationException("No name is pending.");
        _pendingName = null;
    }

    /// <summary>
    /// Records a call to <see cref="IDocumentWriter.EndRepeated"/>.
    /// </summary>
    /// <exception cref="InvalidOperationException">
    /// No group is open, or no repeated field is open in the innermost group.
    /// </exception>
    public void EndRepeated()
    {
        Group group = PeekGroup();

        if (group.RepeatedName is null)
            throw new InvalidOperationException("No repeated field is open.");

        group.RepeatedName = null;
    }

    private void EnsureNotComplete()
    {
        if (_isComplete)
            throw new InvalidOperationException("The document is complete.");
    }

    private Group PeekGroup() =>
        _groups.TryPeek(out Group? group) ? group : throw new InvalidOperationException("No group is open.");

    private string TakeName()
    {
        if (_groups.TryPeek(out Group? group) && group.RepeatedName is not null)
            return group.RepeatedName;

        string name = _pendingName ?? throw new InvalidOperationException("No name is pending.");
        _pendingName = null;
        return name;
    }

    private sealed class Group
    {
        public string? RepeatedName { get; set; }
    }
}
