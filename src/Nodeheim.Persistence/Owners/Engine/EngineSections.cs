using Nodeheim.Domain;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

/// <summary>
/// Provides the owner <c>engine</c> and the section mappers of its sections.
/// </summary>
internal static class EngineSections
{
    /// <summary>
    /// Gets the name of the owner <c>engine</c>.
    /// </summary>
    public static string Owner { get; } = "engine";

    /// <summary>
    /// Gets the section mapper of <c>engine.graph</c>.
    /// </summary>
    public static ISectionMapper<Graph> Graph { get; } = new GraphSectionMapper();
}
