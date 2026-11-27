namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a writer that checks each write call and forwards it to another writer.
/// </summary>
/// <remarks>
/// The order of the calls is checked through <see cref="DocumentWriterProtocol"/>; names, texts, and levels
/// by the same rules as on load. Each call is checked before it is forwarded, so the inner writer never
/// receives an invalid call. An invalid call throws; nothing is reported to an issue collector.
/// </remarks>
internal sealed class ValidatingDocumentWriter : IDocumentWriter
{
    private readonly IDocumentWriter _inner;
    private readonly DocumentWriterProtocol _protocol = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidatingDocumentWriter"/> class.
    /// </summary>
    /// <param name="inner">The writer that receives the checked calls.</param>
    /// <exception cref="ArgumentNullException"><paramref name="inner"/> is <see langword="null"/>.</exception>
    public ValidatingDocumentWriter(IDocumentWriter inner)
    {
        ArgumentNullException.ThrowIfNull(inner);

        _inner = inner;
    }

    /// <inheritdoc />
    public void Name(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        if (!DocumentEntry.IsValidName(name))
            throw new ArgumentException($"The name '{name}' is not valid.", nameof(name));

        _protocol.Name(name);
        _inner.Name(name);
    }

    /// <inheritdoc />
    public void Text(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        if (!DocumentEntry.IsValidText(text))
            throw new ArgumentException("The text contains a character that XML 1.0 does not permit.", nameof(text));

        EnsureWithinMaxLevels();
        _ = _protocol.Text();
        _inner.Text(text);
    }

    /// <inheritdoc />
    public void BeginGroup()
    {
        EnsureWithinMaxLevels();
        _ = _protocol.BeginGroup();
        _inner.BeginGroup();
    }

    /// <inheritdoc />
    public void EndGroup()
    {
        _protocol.EndGroup();
        _inner.EndGroup();
    }

    /// <inheritdoc />
    public void BeginRepeated()
    {
        _protocol.BeginRepeated();
        _inner.BeginRepeated();
    }

    /// <inheritdoc />
    public void EndRepeated()
    {
        _protocol.EndRepeated();
        _inner.EndRepeated();
    }

    private void EnsureWithinMaxLevels()
    {
        // A new value lies one level below the innermost open group; the root lies on level 1.
        if (_protocol.OpenGroupCount + 1 > DocumentEntry.MaxLevels)
            throw new InvalidOperationException($"The value would lie below level {DocumentEntry.MaxLevels}.");
    }
}
