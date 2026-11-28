using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents.Adapters;

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
    /// <remarks>
    /// An implementation checks the syntax of its document format and the rules of the document tree, not the
    /// structure of the document. Format contract: An implementation whose syntax carries no name at the root uses
    /// <see cref="DocumentEntry.RootName"/>.
    /// </remarks>
    DocumentEntry? Parse(ReadOnlyMemory<byte> bytes, IIssueCollector issues);
}
