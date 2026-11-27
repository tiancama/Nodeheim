using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

/// <summary>
/// Represents the registered form of an <see cref="ISectionMapper{TModel}"/>.
/// </summary>
/// <remarks>
/// The descriptive properties are read once at registration, so that the checks of the registration remain valid.
/// </remarks>
internal sealed class SectionMapperWrapper<TModel> : ISectionMapper
    where TModel : class
{
    private readonly ISectionMapper<TModel> _mapper;

    public SectionMapperWrapper(ISectionMapper<TModel> mapper)
    {
        _mapper = mapper;
        SectionType = mapper.SectionType;
    }

    public SectionTypeDescriptor SectionType { get; }

    public Type ModelType => typeof(TModel);

    public object? Read(DocumentSection? section, IIssueCollector issues) =>
        _mapper.Read(section, issues);

    public DocumentSectionContribution Contribute(object model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (model is not TModel typedModel)
            throw new ArgumentException(
                $"Section '{SectionType.Name}': the model is of type '{model.GetType()}', not '{typeof(TModel)}'.",
                nameof(model));

        DocumentSectionContribution contribution = _mapper.Contribute(typedModel);
        if (contribution.SectionType != SectionType.Name)
            throw new InvalidOperationException(
                $"Section '{SectionType.Name}': the mapper contributed the section type '{contribution.SectionType}'.");

        return contribution;
    }
}
