using Nodeheim.Issues;
using Nodeheim.Persistence.Documents.Adapters;

namespace Nodeheim.Persistence.Files;

/// <summary>
/// Represents a storage that reads and writes documents as files.
/// </summary>
internal sealed class FileDocumentStorage : IDocumentStorage
{
    /// <inheritdoc />
    public byte[]? Read(string location, IIssueCollector issues) => File.ReadAllBytes(location);

    /// <inheritdoc />
    public void Write(string location, byte[] bytes, IIssueCollector issues) => File.WriteAllBytes(location, bytes);
}
