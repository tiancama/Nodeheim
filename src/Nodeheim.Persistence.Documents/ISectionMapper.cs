using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Defines a registered section mapper, independent of the type of its model.
/// </summary>
/// <remarks>
/// Instances are created by <see cref="SectionMapperRegistry"/> around an <see cref="ISectionMapper{TModel}"/>.
/// </remarks>
public interface ISectionMapper
{
    /// <summary>
    /// Gets the name of the owner.
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
    /// Gets the type of the model.
    /// </summary>
    Type ModelType { get; }

    /// <summary>
    /// Reads the model from the sections of the owner.
    /// </summary>
    /// <param name="sections">The sections of the owner found in the document, by section type; possibly none.</param>
    /// <param name="issues">The collector that receives the issues of reading.</param>
    /// <returns>The model, or <see langword="null"/> if the sections cannot be read.</returns>
    object? Read(IReadOnlyDictionary<string, DocumentSection> sections, IIssueCollector issues);

    /// <summary>
    /// Returns the contributions of the model to a document that is being written, one per section.
    /// </summary>
    /// <param name="model">The model; of type <see cref="ModelType"/>.</param>
    /// <returns>The contributions.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="model"/> is not of type <see cref="ModelType"/>.</exception>
    IReadOnlyList<DocumentSectionContribution> Contribute(object model);
}
