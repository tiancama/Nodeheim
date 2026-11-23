using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents;

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
        OwnerName = mapper.OwnerName;
        DisplayName = mapper.DisplayName;
        SectionTypes = Array.AsReadOnly(mapper.SectionTypes.ToArray());
    }

    public string OwnerName { get; }

    public string DisplayName { get; }

    public IReadOnlyList<SectionTypeDescriptor> SectionTypes { get; }

    public Type ModelType => typeof(TModel);

    public object? Read(IReadOnlyDictionary<string, DocumentSection> sections, IIssueCollector issues) =>
        _mapper.Read(sections, issues);

    public IReadOnlyList<DocumentSectionContribution> Contribute(object model)
    {
        ArgumentNullException.ThrowIfNull(model);

        if (model is not TModel typedModel)
            throw new ArgumentException(
                $"Owner '{OwnerName}': the model is of type '{model.GetType()}', not '{typeof(TModel)}'.",
                nameof(model));

        return _mapper.Contribute(typedModel);
    }
}
