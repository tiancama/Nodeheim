using Nodeheim.Persistence.Documents;
using Nodeheim.Persistence.Documents.Adapters;

namespace Nodeheim.Persistence;

/// <summary>
/// Represents a builder that composes the document repository from document formats, a storage, and section
/// mappers.
/// </summary>
public sealed class DocumentRepositoryBuilder
{
    private readonly List<DocumentFormat> _formats = new();
    private IDocumentStorage? _storage;
    private readonly SectionMapperRegistry _registry = new();
    private readonly Dictionary<string, OwnerRegistration> _owners = new(StringComparer.Ordinal);

    /// <summary>
    /// Adds a document format.
    /// </summary>
    /// <param name="format">The document format to add.</param>
    /// <returns>This builder, to add further registrations.</returns>
    public DocumentRepositoryBuilder AddFormat(DocumentFormat format)
    {
        _formats.Add(format);
        return this;
    }

    /// <summary>
    /// Sets the storage that reads and writes the bytes of documents.
    /// </summary>
    /// <param name="storage">The storage to use; it replaces a storage set before.</param>
    /// <returns>This builder, to add further registrations.</returns>
    public DocumentRepositoryBuilder UseStorage(IDocumentStorage storage)
    {
        _storage = storage;
        return this;
    }

    /// <summary>
    /// Adds the owner <c>engine</c> with the section mapper of <c>engine.graph</c>.
    /// </summary>
    /// <returns>This builder, to add further registrations.</returns>
    public DocumentRepositoryBuilder AddEngine() => AddSectionMapper(EngineSections.Owner, EngineSections.Graph);

    /// <summary>
    /// Adds a section mapper of an owner.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="owner">The name of the owner; each section type of the mapper starts with it and a dot.</param>
    /// <param name="mapper">The section mapper to add.</param>
    /// <returns>This builder, to add further registrations.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="owner"/> or <paramref name="mapper"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="owner"/> is not a valid entry name; the section type of <paramref name="mapper"/> is not a
    /// valid name, does not start with <paramref name="owner"/> and a dot, or is already registered; or
    /// <typeparamref name="TModel"/> is already registered.
    /// </exception>
    public DocumentRepositoryBuilder AddSectionMapper<TModel>(string owner, ISectionMapper<TModel> mapper)
        where TModel : class
    {
        if (!_owners.TryGetValue(owner, out OwnerRegistration? registration))
        {
            registration = _registry.Register(owner);
            _owners.Add(owner, registration);
        }

        registration.AddSection(mapper);
        return this;
    }

    /// <summary>
    /// Builds the document repository.
    /// </summary>
    /// <returns>The document repository with the registrations made so far.</returns>
    public IDocumentRepository Build() => new DocumentRepository(_formats, _storage!, _registry);
}
