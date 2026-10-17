using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Defines a port that reads a document from its storage location.
/// </summary>
/// <remarks>
/// The storage location is part of the configuration of an implementation, not a parameter.
/// </remarks>
public interface IDocumentStorage
{
    /// <summary>
    /// Loads the root value of the document.
    /// </summary>
    /// <param name="issues">The collector that receives the issues found while loading.</param>
    /// <returns>
    /// The root value, or <see langword="null"/> if the document is rejected; the rejection is added to
    /// <paramref name="issues"/>.
    /// </returns>
    /// <remarks>
    /// An implementation checks the syntax of its format and the rules of the value model, not the structure of
    /// the document.
    /// </remarks>
    DocumentValue? Load(IIssueCollector issues);
}
