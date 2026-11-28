using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents.Adapters;

/// <summary>
/// Defines a port that reads and writes the bytes of documents at their storage locations.
/// </summary>
/// <remarks>
/// The storage knows nothing of document formats; it only moves bytes.
/// </remarks>
public interface IDocumentStorage
{
    /// <summary>
    /// Reads the bytes of the document at the given location.
    /// </summary>
    /// <param name="location">The storage location of the document, such as a file path.</param>
    /// <param name="issues">The collector intended for failures of the environment; not used yet.</param>
    /// <returns>
    /// The bytes of the document, or <see langword="null"/> if they cannot be read; for now, never
    /// <see langword="null"/>.
    /// </returns>
    byte[]? Read(string location, IIssueCollector issues);

    /// <summary>
    /// Writes the bytes of a document to the given location, replacing the document stored there.
    /// </summary>
    /// <param name="location">The storage location of the document, such as a file path.</param>
    /// <param name="bytes">The bytes of the document.</param>
    /// <param name="issues">The collector intended for failures of the environment; not used yet.</param>
    void Write(string location, byte[] bytes, IIssueCollector issues);
}
