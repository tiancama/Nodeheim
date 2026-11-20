using Nodeheim.Domain;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

/// <summary>
/// Represents the coordinator that loads and saves a graph as a document through a document storage.
/// </summary>
public sealed class DocumentRepository : IDocumentRepository
{
    private readonly Func<string, IDocumentStorage> _storageFactory;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentRepository"/> class.
    /// </summary>
    /// <param name="descriptors">The descriptions of the supported kinds of document storage.</param>
    /// <param name="storageFactory">The function that creates the document storage for a location.</param>
    public DocumentRepository(IReadOnlyList<DocumentStorageDescriptor> descriptors,
        Func<string, IDocumentStorage> storageFactory)
    {
        _storageFactory = storageFactory;
        StorageDescriptors = [.. descriptors];
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
        DocumentEntry? root = storage.Load(issues);
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

    /// <inheritdoc />
    /// <remarks>
    /// <inheritdoc path="/remarks/node()" />
    /// Errors of the environment, such as I/O errors, are not yet reported as issues and cause exceptions.
    /// </remarks>
    public void Save(string location, Graph graph)
    {
        ArgumentNullException.ThrowIfNull(location);
        ArgumentNullException.ThrowIfNull(graph);

        IDocumentStorage storage = _storageFactory(location);
        DocumentSectionContribution[] contributions = [GraphMapper.CreateContribution(graph)];

        storage.Save(writer => DocumentMapper.Write(writer, contributions));
    }
}
