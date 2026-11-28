using Nodeheim.Persistence.Documents.Adapters;

namespace Nodeheim.Persistence.Files;

/// <summary>
/// Provides the storage of documents as files.
/// </summary>
public static class FileStorage
{
    /// <summary>
    /// Gets the storage of documents as files.
    /// </summary>
    public static IDocumentStorage Value { get; } = new FileDocumentStorage();
}
