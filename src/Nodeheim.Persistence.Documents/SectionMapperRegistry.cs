namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents the registration of the section mappers.
/// </summary>
/// <remarks>
/// Each registration is checked at once, so that a conflict surfaces at startup. A section type belongs to at most
/// one section mapper, a model type occurs at most once, and each section type is a valid entry name that starts
/// with the name of its owner and a dot.
/// </remarks>
public sealed class SectionMapperRegistry
{
    private readonly List<ISectionMapper> _mappers = new();
    private readonly HashSet<string> _sectionTypes = new(StringComparer.Ordinal);
    private readonly HashSet<Type> _modelTypes = new();

    /// <summary>
    /// Gets the registered section mappers, in the order of registration, each as the wrapper created at
    /// registration rather than the registered instance itself.
    /// </summary>
    public IReadOnlyList<ISectionMapper> Mappers => _mappers;

    /// <summary>
    /// Registers a section mapper.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="mapper">The section mapper.</param>
    /// <exception cref="ArgumentNullException"><paramref name="mapper"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// The model type is already registered, or a section type is not a valid entry name, does not start with the
    /// name of the owner and a dot, or is already registered.
    /// </exception>
    public void Register<TModel>(ISectionMapper<TModel> mapper)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(mapper);

        SectionMapperWrapper<TModel> wrapper = new(mapper);
        string owner = wrapper.OwnerName;

        if (_modelTypes.Contains(wrapper.ModelType))
            throw new ArgumentException(
                $"Owner '{owner}': the model type '{wrapper.ModelType}' is already registered.", nameof(mapper));

        HashSet<string> ownSectionTypes = new(StringComparer.Ordinal);
        foreach (SectionTypeDescriptor descriptor in wrapper.SectionTypes)
        {
            string sectionType = descriptor.SectionType;

            if (!DocumentEntry.IsValidName(sectionType))
                throw Conflict(owner, sectionType, "it is not a valid entry name", nameof(mapper));

            if (!sectionType.StartsWith(owner + ".", StringComparison.Ordinal))
                throw Conflict(owner, sectionType, "it does not start with the owner name and a dot", nameof(mapper));

            if (_sectionTypes.Contains(sectionType) || !ownSectionTypes.Add(sectionType))
                throw Conflict(owner, sectionType, "it is already registered", nameof(mapper));
        }

        _modelTypes.Add(wrapper.ModelType);
        _sectionTypes.UnionWith(ownSectionTypes);
        _mappers.Add(wrapper);
    }

    private static ArgumentException Conflict(string owner, string sectionType, string reason, string paramName) =>
        new($"Owner '{owner}', section type '{sectionType}': {reason}.", paramName);
}
