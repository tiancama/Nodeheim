using Nodeheim.Domain;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

/// <summary>
/// Represents the coordinator that loads a document through a document storage and maps it to a graph.
/// </summary>
/// <remarks>
/// The repository supports exactly one kind of document storage. Every location is passed to it, regardless of
/// the file extensions in its descriptor.
/// </remarks>
public sealed class DocumentRepository : IDocumentRepository
{
    private readonly Func<string, IDocumentStorage> _storageFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentRepository"/> class.
    /// </summary>
    /// <param name="descriptor">The description of the kind of document storage that the factory creates.</param>
    /// <param name="storageFactory">The function that creates a document storage for a location.</param>
    public DocumentRepository(DocumentStorageDescriptor descriptor, Func<string, IDocumentStorage> storageFactory)
    {
        _storageFactory = storageFactory;
        StorageDescriptors = [descriptor];
    }

    /// <inheritdoc />
    public IReadOnlyList<DocumentStorageDescriptor> StorageDescriptors { get; }

    /// <inheritdoc />
    /// <remarks>
    /// <inheritdoc path="/remarks/node()" />
    /// Errors of the environment, such as I/O errors, are not yet reported as issues and cause exceptions.
    /// </remarks>
    public Graph? Load(string location, IIssueCollector issues)
    {
        IDocumentStorage storage = _storageFactory(location);
        DocumentValue? root = storage.Load(issues);
        if (root is null) return null;

        Document? document = DocumentMapper.Read(root, issues);
        if (document is null) return null;

        foreach (string key in document.Sections.Keys)
        {
            if (key != GraphMapper.SectionType)
                issues.Add(new Issue(FormatIssues.UnknownSection,
                    $"Unknown section '{key}' is skipped.", $"/sections/{key}"));
        }

        if (!document.Sections.TryGetValue(GraphMapper.SectionType, out DocumentSection? section))
            return null;

        return GraphMapper.Read(section, issues);
    }
}
