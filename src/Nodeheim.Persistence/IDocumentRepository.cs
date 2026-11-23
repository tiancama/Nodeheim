using Nodeheim.Domain;
using Nodeheim.Issues;

namespace Nodeheim.Persistence;

/// <summary>
/// Defines the port through which the application loads and saves documents.
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
    /// The loaded graph, or <see langword="null"/> if the document or one of its sections is rejected.
    /// </returns>
    /// <remarks>
    /// Sections of an unknown section type are skipped and reported as discrepancies. A document without a
    /// <c>core.graph</c> section yields an empty graph. <see langword="null"/> comes with a rejection in
    /// <paramref name="issues"/>; a graph never does.
    /// </remarks>
    Graph? Load(string location, IIssueCollector issues);

    /// <summary>
    /// Saves the graph to the document at the specified location.
    /// </summary>
    /// <param name="location">The storage location of the document, such as a file path.</param>
    /// <param name="graph">The graph to save.</param>
    /// <param name="issues">The collector that receives the issues found while saving.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="location"/>, <paramref name="graph"/>, or <paramref name="issues"/> is
    /// <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// The document is written in the format of the storage at <paramref name="location"/> and replaces the
    /// document stored there.
    /// </remarks>
    void Save(string location, Graph graph, IIssueCollector issues);
}
