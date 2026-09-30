using Nodeheim.Domain;

namespace Nodeheim.Editor;

/// <summary>
/// Represents an open world in the editor, with its surface and pointer interaction.
/// </summary>
public class DocumentViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentViewModel"/> class.
    /// </summary>
    /// <param name="graph">The graph of the world the document represents.</param>
    public DocumentViewModel(Graph graph)
    {
        Surface = new(graph);
        InteractionController = new(Surface);
    }

    /// <summary>
    /// Gets the surface that projects the graph for display and editing.
    /// </summary>
    public SurfaceViewModel Surface { get; }

    /// <summary>
    /// Gets the controller that interprets pointer input on the surface.
    /// </summary>
    public InteractionController InteractionController { get; }
}
