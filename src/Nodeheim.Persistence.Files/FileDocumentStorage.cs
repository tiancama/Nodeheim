using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence.Files;

public sealed class FileDocumentStorage
{
    private readonly string _path;
    private readonly IDocumentParser _parser;

    public FileDocumentStorage(string path, IDocumentParser parser)
    {
        ArgumentNullException.ThrowIfNull(path);
        ArgumentNullException.ThrowIfNull(parser);

        _path = path;
        _parser = parser;
    }

    public DocumentEntry? Load(IIssueCollector issues)
    {
        byte[] bytes = File.ReadAllBytes(_path);

        return _parser.Parse(bytes, issues);
    }
}
