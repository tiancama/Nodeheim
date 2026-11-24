using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Defines a section mapper, which maps one section to a model and the model back to that section.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <remarks>
/// A section mapper holds no state between calls; one registered instance serves every document. A new section
/// requires a new section mapper.
/// </remarks>
public interface ISectionMapper<TModel>
    where TModel : class
{
    /// <summary>
    /// Gets the section type that this mapper maps.
    /// </summary>
    SectionTypeDescriptor SectionType { get; }

    /// <summary>
    /// Reads the model from the section.
    /// </summary>
    /// <param name="section">
    /// The section of <see cref="SectionType"/> found in the document, or <see langword="null"/> if the document
    /// does not contain it.
    /// </param>
    /// <param name="issues">The collector that receives the issues of reading.</param>
    /// <returns>
    /// The model; an empty model if <paramref name="section"/> is <see langword="null"/>; or
    /// <see langword="null"/> if the section cannot be read.
    /// </returns>
    /// <remarks>
    /// <see langword="null"/> means only that reading failed; the reason is reported to <paramref name="issues"/>
    /// with a code of the owner.
    /// Provisional: The empty model for a missing section is under trial. It may be replaced by not calling the
    /// mapper at all, so that the application decides whether to start empty; this method then remains unchanged.
    /// </remarks>
    TModel? Read(DocumentSection? section, IIssueCollector issues);

    /// <summary>
    /// Returns the contribution of the model to a document that is being written.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>
    /// The contribution; its section type is the <see cref="SectionTypeDescriptor.Name"/> of <see cref="SectionType"/>.
    /// </returns>
    DocumentSectionContribution Contribute(TModel model);
}
