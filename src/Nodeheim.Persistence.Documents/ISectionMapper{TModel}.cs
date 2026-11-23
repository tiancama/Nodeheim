using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Defines a section mapper, which maps the sections of one owner to a model and the model back to sections.
/// </summary>
/// <typeparam name="TModel">The type of the model.</typeparam>
/// <remarks>
/// A section mapper holds no state between calls; one registered instance serves every document.
/// </remarks>
public interface ISectionMapper<TModel>
    where TModel : class
{
    /// <summary>
    /// Gets the name of the owner; each of its section types starts with this name and a dot.
    /// </summary>
    string OwnerName { get; }

    /// <summary>
    /// Gets the name of the owner shown to the user.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    /// Gets the section types of the owner.
    /// </summary>
    IReadOnlyList<SectionTypeDescriptor> SectionTypes { get; }

    /// <summary>
    /// Reads the model from the sections of the owner.
    /// </summary>
    /// <param name="sections">The sections of the owner found in the document, by section type; possibly none.</param>
    /// <param name="issues">The collector that receives the issues of reading.</param>
    /// <returns>
    /// The model, an empty model if <paramref name="sections"/> is empty; or <see langword="null"/> if the sections
    /// cannot be read.
    /// </returns>
    /// <remarks>
    /// <see langword="null"/> means only that reading failed; the reason is reported to <paramref name="issues"/>
    /// with a code of the owner.
    /// </remarks>
    TModel? Read(IReadOnlyDictionary<string, DocumentSection> sections, IIssueCollector issues);

    /// <summary>
    /// Returns the contributions of the model to a document that is being written, one per section.
    /// </summary>
    /// <param name="model">The model.</param>
    /// <returns>The contributions; each with a section type from <see cref="SectionTypes"/>.</returns>
    IReadOnlyList<DocumentSectionContribution> Contribute(TModel model);
}
