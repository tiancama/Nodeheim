using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Defines a port that reads and writes a document at its storage location.
/// </summary>
/// <remarks>
/// The storage location is part of the configuration of an implementation, not a parameter.
/// </remarks>
public interface IDocumentStorage
{
    /// <summary>
    /// Loads the root entry of the document.
    /// </summary>
    /// <param name="issues">The collector that receives the issues found while loading.</param>
    /// <returns>
    /// The root entry, or <see langword="null"/> if the document is rejected; the rejection is added to
    /// <paramref name="issues"/>.
    /// </returns>
    /// <remarks>
    /// An implementation checks the syntax of its format and the rules of the document tree, not the structure of
    /// the document. Format contract: An implementation whose syntax carries no name at the root uses
    /// <see cref="DocumentEntry.RootName"/>.
    /// </remarks>
    DocumentEntry? Load(IIssueCollector issues);

    /// <summary>
    /// Saves a document.
    /// </summary>
    /// <param name="write">The function that writes the document to the given writer.</param>
    /// <param name="issues">The collector that receives the issues found while saving.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="write"/> or <paramref name="issues"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// An implementation creates the writer for its format and invokes <paramref name="write"/> once. It changes
    /// the stored document only after <paramref name="write"/> has completed.
    /// </remarks>
    void Save(Action<IDocumentWriter> write, IIssueCollector issues);
}
