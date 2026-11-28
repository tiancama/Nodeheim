using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

/// <summary>
/// Represents the registration of the owners and their section mappers.
/// </summary>
/// <remarks>
/// Each registration is checked at once, so that a conflict surfaces at startup. An owner name is a valid entry
/// name and is registered at most once. A section type belongs to at most one section mapper, a model type occurs
/// at most once, and each section type is a valid entry name that starts with the name of its owner and a dot.
/// </remarks>
internal sealed class SectionMapperRegistry
{
    private readonly List<ISectionMapper> _mappers = new();
    private readonly HashSet<string> _ownerNames = new(StringComparer.Ordinal);
    private readonly HashSet<string> _sectionTypes = new(StringComparer.Ordinal);
    private readonly HashSet<Type> _modelTypes = new();

    /// <summary>
    /// Gets the registered section mappers, in the order of registration, each as the wrapper created at
    /// registration rather than the registered instance itself.
    /// </summary>
    internal IReadOnlyList<ISectionMapper> Mappers => _mappers;

    /// <summary>
    /// Registers an owner.
    /// </summary>
    /// <param name="owner">The name of the owner; each of its section types starts with this name and a dot.</param>
    /// <returns>The registration of the owner, to which its sections are added.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="owner"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="owner"/> is not a valid entry name or is already registered.
    /// </exception>
    public OwnerRegistration Register(string owner)
    {
        ArgumentNullException.ThrowIfNull(owner);

        if (!DocumentEntry.IsValidName(owner) || !_ownerNames.Add(owner))
            throw new ArgumentException(
                $"Owner '{owner}': it is not a valid entry name or is already registered.", nameof(owner));

        return new OwnerRegistration(this, owner);
    }

    internal void Add<TModel>(string owner, ISectionMapper<TModel> mapper)
        where TModel : class
    {
        ArgumentNullException.ThrowIfNull(mapper);

        SectionMapperWrapper<TModel> wrapper = new(mapper);
        string sectionType = wrapper.SectionType.Name;

        if (_modelTypes.Contains(wrapper.ModelType))
            throw new ArgumentException(
                $"Owner '{owner}': the model type '{wrapper.ModelType}' is already registered.", nameof(mapper));

        if (!DocumentEntry.IsValidName(sectionType))
            throw Conflict(owner, sectionType, "it is not a valid entry name", nameof(mapper));

        if (!sectionType.StartsWith(owner + ".", StringComparison.Ordinal))
            throw Conflict(owner, sectionType, "it does not start with the owner name and a dot", nameof(mapper));

        if (!_sectionTypes.Add(sectionType))
            throw Conflict(owner, sectionType, "it is already registered", nameof(mapper));

        _modelTypes.Add(wrapper.ModelType);
        _mappers.Add(wrapper);
    }

    private static ArgumentException Conflict(string owner, string sectionType, string reason, string paramName) =>
        new($"Owner '{owner}', section type '{sectionType}': {reason}.", paramName);
}
