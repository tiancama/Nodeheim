using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

public sealed class DocumentRepository
{
    private readonly IDocumentStorage _storage;

    public DocumentRepository(IDocumentStorage storage)
    {
        _storage = storage;
    }

    public DocumentValueResult Load()
    {
        return _storage.Load();
    }
}
