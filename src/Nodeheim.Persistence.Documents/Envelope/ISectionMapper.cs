using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Defines a registered section mapper whose model type is known only at run time.
/// </summary>
/// <remarks>
/// Instances are created by <see cref="SectionMapperRegistry"/> around an <see cref="ISectionMapper{TModel}"/>.
/// </remarks>
public interface ISectionMapper
{
    /// <inheritdoc cref="ISectionMapper{TModel}.SectionType" />
    SectionTypeDescriptor SectionType { get; }

    /// <summary>
    /// Gets the type of the model.
    /// </summary>
    Type ModelType { get; }

    /// <inheritdoc cref="ISectionMapper{TModel}.Read" />
    object? Read(DocumentSection? section, IIssueCollector issues);

    /// <inheritdoc cref="ISectionMapper{TModel}.Contribute" />
    /// <exception cref="ArgumentException"><paramref name="model"/> is not of <see cref="ModelType"/>.</exception>
    DocumentSectionContribution Contribute(object model);
}
