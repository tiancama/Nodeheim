using Nodeheim.Domain;
using Nodeheim.Persistence;

namespace Nodeheim.Editor;

/// <summary>
/// Represents an open world in the editor, with its surface and pointer interaction.
/// </summary>
public class DocumentViewModel
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DocumentViewModel"/> class.
    /// </summary>
    /// <param name="document">
    /// The document to edit; if it holds no graph, an empty graph is added to it.
    /// </param>
    public DocumentViewModel(LoadedDocument document)
    {
        LoadedDocument = document;

        Graph = document.Models.Get<Graph>() ?? new Graph();
        document.Models.Set(Graph);

        SurfaceViewModel = new(Graph);
        InteractionController = new(SurfaceViewModel);
    }

    /// <summary>
    /// Gets the surface that projects the graph for display and editing.
    /// </summary>
    public SurfaceViewModel SurfaceViewModel { get; }

    /// <summary>
    /// Gets the controller that interprets pointer input on the surface.
    /// </summary>
    public InteractionController InteractionController { get; }

    /// <summary>
    /// Gets the document that is edited and passed back when saving.
    /// </summary>
    internal LoadedDocument LoadedDocument { get; }

    /// <summary>
    /// Gets the graph that the document represents and the surface edits.
    /// </summary>
    internal Graph Graph { get; }
}
