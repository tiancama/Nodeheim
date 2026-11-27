namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents the registration of one owner, to which its sections are added.
/// </summary>
/// <remarks>
/// Instances are created by <see cref="SectionMapperRegistry.Register(OwnerDescriptor)"/>.
/// </remarks>
public sealed class OwnerRegistration
{
    private readonly SectionMapperRegistry _registry;
    private readonly OwnerDescriptor _owner;

    internal OwnerRegistration(SectionMapperRegistry registry, OwnerDescriptor owner)
    {
        _registry = registry;
        _owner = owner;
    }

    /// <summary>
    /// Adds a section of the owner, represented by its section mapper.
    /// </summary>
    /// <typeparam name="TModel">The type of the model.</typeparam>
    /// <param name="mapper">The section mapper of the section.</param>
    /// <returns>This registration, to add further sections.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="mapper"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// The section type of <paramref name="mapper"/> is not a valid name, does not start with the name of the owner
    /// and a dot, or is already registered; or <typeparamref name="TModel"/> is already registered.
    /// </exception>
    public OwnerRegistration AddSection<TModel>(ISectionMapper<TModel> mapper)
        where TModel : class
    {
        _registry.Add(_owner, mapper);
        return this;
    }
}
