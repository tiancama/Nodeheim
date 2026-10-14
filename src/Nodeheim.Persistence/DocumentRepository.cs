using System.Diagnostics;
using Nodeheim.Domain;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

public sealed class DocumentRepository
{
    private readonly IDocumentStorage _storage;

    public DocumentRepository(IDocumentStorage storage)
    {
        _storage = storage;
    }

    public Graph Load()
    {
        DocumentValue root = _storage.Load() switch
        {
            DocumentValueSuccess success => success.Value,
            DocumentValueFailure failure => throw new InvalidOperationException(failure.Rejection.Message),
            _ => throw new UnreachableException()
        };

        Document document = DocumentMapper.Read(root) switch
        {
            DocumentSuccess success => success.Document,
            DocumentFailure failure => throw new InvalidOperationException(failure.Rejection.Message),
            _ => throw new UnreachableException()
        };

        if (!document.Sections.TryGetValue(GraphMapper.SectionType, out DocumentSection? section))
            throw new InvalidOperationException($"The section '{GraphMapper.SectionType}' is missing.");

        return GraphMapper.Read(section);
    }
}
