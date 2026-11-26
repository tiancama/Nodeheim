using Nodeheim.Editor.Domain;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Editor.Persistence;

/// <summary>
/// Provides the owner <c>editor</c> and the section mappers of its sections.
/// </summary>
public static class EditorSections
{
    /// <summary>
    /// Gets the description of the owner <c>editor</c>.
    /// </summary>
    public static OwnerDescriptor Owner { get; } = new("editor", "Editor");

    /// <summary>
    /// Gets the section mapper of <c>editor.surface</c>.
    /// </summary>
    public static ISectionMapper<Surface> Surface { get; } = new SurfaceSectionMapper();
}
