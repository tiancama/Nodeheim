using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Defines a port that converts the bytes of a document into a document tree.
/// </summary>
public interface IDocumentParser
{
    /// <summary>
    /// Parses the bytes of a document.
    /// </summary>
    /// <param name="bytes">The bytes of the document.</param>
    /// <param name="issues">The collector that receives the issues found while parsing.</param>
    /// <returns>
    /// The root entry, or <see langword="null"/> if the document is rejected; the rejection is added to
    /// <paramref name="issues"/>.
    /// </returns>
    DocumentEntry? Parse(ReadOnlyMemory<byte> bytes, IIssueCollector issues);
}
