namespace Nodeheim.Persistence.Documents.Adapters;

/// <summary>
/// Represents a document format, such as JSON, with its parser and its writer.
/// </summary>
public sealed class DocumentFormat
{
    private readonly Func<Stream, IDocumentWriter> _createWriter;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentFormat"/> class.
    /// </summary>
    /// <param name="name">The name of the document format, such as JSON.</param>
    /// <param name="fileExtensions">The file extensions of the document format, each with a leading period.</param>
    /// <param name="parser">The parser that reads documents of the document format.</param>
    /// <param name="createWriter">The function that creates a writer of the document format for a stream.</param>
    public DocumentFormat(string name, IReadOnlyList<string> fileExtensions, IDocumentParser parser,
        Func<Stream, IDocumentWriter> createWriter)
    {
        Name = name;
        FileExtensions = fileExtensions;
        Parser = parser;
        _createWriter = createWriter;
    }

    /// <summary>
    /// Gets the name of the document format, such as JSON.
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Gets the file extensions of the document format, each with a leading period, such as <c>.json</c>.
    /// </summary>
    public IReadOnlyList<string> FileExtensions { get; }

    /// <summary>
    /// Gets the parser of the document format.
    /// </summary>
    public IDocumentParser Parser { get; }

    /// <summary>
    /// Creates a writer of the document format that writes to the given stream.
    /// </summary>
    /// <param name="stream">The stream that receives the document; it stays open.</param>
    /// <returns>A new writer of the document format.</returns>
    public IDocumentWriter CreateWriter(Stream stream) => _createWriter(stream);
}
