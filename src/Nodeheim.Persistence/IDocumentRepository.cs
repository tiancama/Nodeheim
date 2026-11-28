using Nodeheim.Issues;

namespace Nodeheim.Persistence;

/// <summary>
/// Defines the port through which the application loads and saves documents.
/// </summary>
public interface IDocumentRepository
{
    /// <summary>
    /// Gets the descriptions of the registered document formats, in the order of registration.
    /// </summary>
    IReadOnlyList<DocumentFormatDescriptor> Formats { get; }

    /// <summary>
    /// Creates a new document with an empty model for each registered section mapper.
    /// </summary>
    /// <returns>The new document.</returns>
    /// <exception cref="InvalidOperationException">
    /// A section mapper returns no model for a missing section.
    /// </exception>
    LoadedDocument Create();

    /// <summary>
    /// Loads the document at the specified location.
    /// </summary>
    /// <param name="location">The storage location of the document, such as a file path.</param>
    /// <param name="issues">The collector that receives the issues found while loading.</param>
    /// <returns>
    /// The loaded document, or <see langword="null"/> if the document or one of its sections is rejected.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// No document format is registered for the file extension of <paramref name="location"/>.
    /// </exception>
    /// <remarks>
    /// The document format is chosen by the file extension of <paramref name="location"/>, ignoring case. Sections
    /// of an unknown section type are skipped and reported as discrepancies. Each registered owner contributes a
    /// model, an empty one if the document has none of its sections. <see langword="null"/> comes with a rejection
    /// in <paramref name="issues"/>; a document never does.
    /// </remarks>
    LoadedDocument? Load(string location, IIssueCollector issues);

    /// <summary>
    /// Saves the document to the specified location.
    /// </summary>
    /// <param name="location">The storage location of the document, such as a file path.</param>
    /// <param name="document">The document to save.</param>
    /// <param name="issues">The collector that receives the issues found while saving.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="location"/>, <paramref name="document"/>, or <paramref name="issues"/> is
    /// <see langword="null"/>.
    /// </exception>
    /// <exception cref="ArgumentException">
    /// No document format is registered for the file extension of <paramref name="location"/>.
    /// </exception>
    /// <remarks>
    /// Each registered section mapper contributes the section of its model. The document format is chosen by the
    /// file extension of <paramref name="location"/>, ignoring case. The document replaces the document stored
    /// there.
    /// </remarks>
    void Save(string location, LoadedDocument document, IIssueCollector issues);
}
