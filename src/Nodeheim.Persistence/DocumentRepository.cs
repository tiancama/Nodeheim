using Nodeheim.Domain;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

/// <summary>
/// Represents the coordinator that loads and saves documents through a document storage and the registered
/// section mappers.
/// </summary>
public sealed class DocumentRepository : IDocumentRepository
{
    private readonly Func<string, IDocumentStorage> _storageFactory;
    private readonly IReadOnlyList<ISectionMapper> _mappers;
    private readonly HashSet<string> _knownSectionTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentRepository"/> class.
    /// </summary>
    /// <param name="descriptors">The descriptions of the supported kinds of document storage.</param>
    /// <param name="storageFactory">The function that creates the document storage for a location.</param>
    /// <param name="sectionMappers">
    /// The registered section mappers; later registrations are not taken into account.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="sectionMappers"/> is <see langword="null"/>.</exception>
    public DocumentRepository(IReadOnlyList<DocumentStorageDescriptor> descriptors,
        Func<string, IDocumentStorage> storageFactory, SectionMapperRegistry sectionMappers)
    {
        ArgumentNullException.ThrowIfNull(sectionMappers);

        _storageFactory = storageFactory;
        _mappers = [.. sectionMappers.Mappers];
        _knownSectionTypes = new HashSet<string>(
            _mappers.Select(mapper => mapper.SectionType.Name),
            StringComparer.Ordinal);
        StorageDescriptors = [.. descriptors];
    }

    /// <inheritdoc />
    public IReadOnlyList<DocumentStorageDescriptor> StorageDescriptors { get; }

    /// <inheritdoc />
    /// <remarks>
    /// <inheritdoc path="/remarks/node()" />
    /// Errors of the environment, such as I/O errors, are not yet reported as issues and cause exceptions.
    /// </remarks>
    public LoadedDocument? Load(string location, IIssueCollector issues)
    {
        IDocumentStorage storage = _storageFactory(location);
        DocumentEntry? root = storage.Load(issues);
        if (root is null) return null;

        Document? document = DocumentMapper.Read(root, issues);
        if (document is null) return null;

        foreach (string sectionType in document.Sections.Keys)
        {
            if (!_knownSectionTypes.Contains(sectionType))
                issues.Add(new Issue(FormatIssues.UnknownSection,
                    $"Unknown section '{sectionType}' is skipped.", $"/sections/{sectionType}"));
        }

        LoadedDocument loaded = new();

        foreach (ISectionMapper mapper in _mappers)
        {
            DocumentSection? section = document.Sections.GetValueOrDefault(mapper.SectionType.Name);
            object? model = mapper.Read(section, issues);

            // Provisional until the memento keeps the sections of a mapper that fails.
            if (model is null)
                return null;

            loaded.Models.Set(mapper.ModelType, model);
        }

        return loaded;
    }

    /// <inheritdoc />
    /// <remarks>
    /// <inheritdoc path="/remarks/node()" />
    /// Errors of the environment, such as I/O errors, are not yet reported as issues and cause exceptions.
    /// </remarks>
    public void Save(string location, LoadedDocument document, IIssueCollector issues)
    {
        ArgumentNullException.ThrowIfNull(location);
        ArgumentNullException.ThrowIfNull(document);
        ArgumentNullException.ThrowIfNull(issues);

        List<DocumentSectionContribution> contributions = [];
        foreach (ISectionMapper mapper in _mappers)
        {
            object? model = document.Models.Get(mapper.ModelType);
            if (model is not null)
                contributions.Add(mapper.Contribute(model));
        }

        IDocumentStorage storage = _storageFactory(location);
        storage.Save(writer => DocumentMapper.Write(writer, contributions), issues);
    }
}
