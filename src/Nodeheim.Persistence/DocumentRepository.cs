using System.Diagnostics;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

public sealed class DocumentRepository
{
    private readonly IDocumentStorage _storage;

    public DocumentRepository(IDocumentStorage storage)
    {
        _storage = storage;
    }

    public DocumentResult Load() => _storage.Load() switch
    {
        DocumentValueSuccess success => DocumentMapper.Read(success.Value),
        DocumentValueFailure failure => new DocumentFailure(failure.Rejection),
        _ => throw new UnreachableException()
    };
}
