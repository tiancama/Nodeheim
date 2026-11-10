using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence.Files;

/// <summary>
/// Represents a document storage that reads a document from a file and parses it with an
/// <see cref="IDocumentParser"/>.
/// </summary>
public sealed class FileDocumentStorage
{
    private readonly string _path;
    private readonly IDocumentParser _parser;

    /// <summary>
    /// Initializes a new instance of the <see cref="FileDocumentStorage"/> class.
    /// </summary>
    /// <param name="path">The path of the file.</param>
    /// <param name="parser">The parser for the format of the file.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="path"/> or <paramref name="parser"/> is <see langword="null"/>.
    /// </exception>
    public FileDocumentStorage(string path, IDocumentParser parser)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(parser);

        _path = path;
        _parser = parser;
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
}
