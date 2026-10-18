using Nodeheim.Domain;
using Nodeheim.Issues;

namespace Nodeheim.Persistence;

/// <summary>
/// Defines the port through which the application loads documents.
/// </summary>
public interface IDocumentRepository
{
    /// <summary>
    /// Gets the descriptions of the supported kinds of document storage.
    /// </summary>
    IReadOnlyList<DocumentStorageDescriptor> StorageDescriptors { get; }

    /// <summary>
    /// Loads the graph from the document at the specified location.
    /// </summary>
    /// <param name="location">The storage location of the document, such as a file path.</param>
    /// <param name="issues">The collector that receives the issues found while loading.</param>
    /// <returns>
    /// The loaded graph, or <see langword="null"/> if the document or the content version of the graph is rejected,
    /// or the document has no <c>core.graph</c> section.
    /// </returns>
    /// <remarks>
    /// Sections of an unknown section type are skipped and reported as discrepancies.
    /// A return value of <see langword="null"/> alone does not indicate a failure, because no section is required.
    /// Only a rejection in <paramref name="issues"/> does. A graph is never returned together with a rejection.
    /// </remarks>
    Graph? Load(string location, IIssueCollector issues);
}
