using Nodeheim.Domain;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;
using Nodeheim.Persistence.Documents.Adapters;

namespace Nodeheim.Persistence;

/// <summary>
/// Represents the coordinator that loads and saves documents through a document storage and the registered
/// section mappers.
/// </summary>
internal sealed class DocumentRepository : IDocumentRepository
{
    private readonly IReadOnlyList<DocumentFormat> _formats;
    private readonly IDocumentStorage _storage;
    private readonly IReadOnlyList<ISectionMapper> _mappers;
    private readonly HashSet<string> _knownSectionTypes;

    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentRepository"/> class.
    /// </summary>
    /// <param name="formats">The document formats, in the order of registration.</param>
    /// <param name="storage">The storage that reads and writes the bytes of documents.</param>
    /// <param name="sectionMappers">
    /// The registered section mappers; later registrations are not taken into account.
    /// </param>
    /// <exception cref="ArgumentNullException"><paramref name="sectionMappers"/> is <see langword="null"/>.</exception>
    public DocumentRepository(IReadOnlyList<DocumentFormat> formats, IDocumentStorage storage,
        SectionMapperRegistry sectionMappers)
    {
        ArgumentNullException.ThrowIfNull(sectionMappers);

        _formats = [.. formats];
        _storage = storage;
        _mappers = [.. sectionMappers.Mappers];
        _knownSectionTypes = new HashSet<string>(
            _mappers.Select(mapper => mapper.SectionType.Name),
            StringComparer.Ordinal);
        Formats = [.. _formats.Select(format => new DocumentFormatDescriptor(format.Name, format.FileExtensions))];
    }

    /// <inheritdoc />
    public IReadOnlyList<DocumentFormatDescriptor> Formats { get; }

    /// <inheritdoc />
    public LoadedDocument Create()
    {
        LoadedDocument document = new();
        IssueCollector issues = new();

        foreach (ISectionMapper mapper in _mappers)
        {
            object model = mapper.Read(null, issues)
                ?? throw new InvalidOperationException(
                    $"Section '{mapper.SectionType.Name}': the mapper returned no model for a missing section.");

            document.Set(mapper.ModelType, model);
        }

        return document;
    }

    /// <inheritdoc />
    /// <remarks>
    /// <inheritdoc path="/remarks/node()" />
    /// Errors of the environment, such as I/O errors, are not yet reported as issues and cause exceptions.
    /// </remarks>
    public LoadedDocument? Load(string location, IIssueCollector issues)
    {
        DocumentFormat format = GetFormat(location);

        byte[]? bytes = _storage.Read(location, issues);
        if (bytes is null) return null;

        DocumentEntry? root = format.Parser.Parse(bytes, issues);
        if (root is null) return null;

        Document? document = DocumentMapper.Read(root, issues);
        if (document is null) return null;

        foreach (string sectionType in document.Sections.Keys)
        {
            if (!_knownSectionTypes.Contains(sectionType))
                issues.Add(new Issue(EnvelopeIssues.UnknownSection,
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

            loaded.Set(mapper.ModelType, model);
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

        DocumentFormat format = GetFormat(location);

        List<DocumentSectionContribution> contributions = [];
        foreach (ISectionMapper mapper in _mappers)
        {
            contributions.Add(mapper.Contribute(document.Get(mapper.ModelType)));
        }

        using MemoryStream buffer = new();
        DocumentMapper.Write(format.CreateWriter(buffer), contributions);
        _storage.Write(location, buffer.ToArray(), issues);
    }

    private DocumentFormat GetFormat(string location)
    {
        string extension = Path.GetExtension(location);

        return _formats.FirstOrDefault(format =>
                format.FileExtensions.Contains(extension, StringComparer.OrdinalIgnoreCase))
            ?? throw new ArgumentException(
                $"No document format is registered for the file extension '{extension}'.", nameof(location));
    }
}
