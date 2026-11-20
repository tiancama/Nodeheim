using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence.Files;

/// <inheritdoc />
public sealed class FileDocumentStorage : IDocumentStorage
{
    private readonly string _path;
    private readonly IDocumentParser _parser;
    private readonly Func<Stream, IDocumentWriter> _writerFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDocumentStorage"/> class.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="parser">The parser for the format of the file.</param>
    /// <param name="writerFactory">The function that creates the writer for the format of the file.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="path"/>, <paramref name="parser"/>, or <paramref name="writerFactory"/> is
    /// <see langword="null"/>.
    /// </exception>
    public FileDocumentStorage(string path, IDocumentParser parser, Func<Stream, IDocumentWriter> writerFactory)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(parser);
        ArgumentNullException.ThrowIfNull(writerFactory);

        _path = path;
        _parser = parser;
        _writerFactory = writerFactory;
    }

    /// <summary>
    /// Loads the root entry of the document.
    /// </summary>
    /// <param name="issues">The collector that receives the issues found while loading.</param>
    /// <returns>
    /// The root entry, or <see langword="null"/> if the document is rejected; the rejection is added to
    /// <paramref name="issues"/>.
    /// </returns>
    public DocumentEntry? Load(IIssueCollector issues)
    {
        byte[] bytes = File.ReadAllBytes(_path);

        return _parser.Parse(bytes, issues);
    }

    /// <inheritdoc />
    /// <remarks>
    /// <inheritdoc path="/remarks/node()" />
    /// Errors of the environment, such as I/O errors, are not yet reported as issues and cause exceptions.
    /// </remarks>
    public void Save(Action<IDocumentWriter> write, IIssueCollector issues)
    {
        ArgumentNullException.ThrowIfNull(write);
        ArgumentNullException.ThrowIfNull(issues);

        using MemoryStream buffer = new();
        write(_writerFactory(buffer));

        using FileStream file = File.Create(_path);
        buffer.WriteTo(file);
    }
}
