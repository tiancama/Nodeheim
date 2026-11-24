using Nodeheim.Domain;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

/// <summary>
/// Provides the owner <c>engine</c> and the section mappers of its sections.
/// </summary>
public static class EngineSections
{
    /// <summary>
    /// Gets the description of the owner <c>engine</c>.
    /// </summary>
    public static OwnerDescriptor Owner { get; } = new("engine", "Engine");

    /// <summary>
    /// Gets the section mapper of <c>engine.graph</c>.
    /// </summary>
    public static ISectionMapper<Graph> Graph { get; } = new GraphSectionMapper();
}
