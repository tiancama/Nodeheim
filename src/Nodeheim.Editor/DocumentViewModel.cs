using Nodeheim.Domain;
using Nodeheim.Editor.Domain;
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
    /// The document to edit; if it holds no graph or no surface, an empty one is added to it. // geändert
    /// </param>
    public DocumentViewModel(LoadedDocument document)
    {
        LoadedDocument = document;

        Graph = document.Models.Get<Graph>() ?? new Graph();
        document.Models.Set(Graph);

        Surface = document.Models.Get<Surface>() ?? new Surface();
        document.Models.Set(Surface);
        ReconcileSurface();

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

    /// <summary>
    /// Gets the editor's model of the surface, with an entry for each node of the graph.
    /// </summary>
    internal Surface Surface { get; }

    /// <summary>
    /// Brings the surface in line with the graph: adds an entry at the origin for each node without one and
    /// removes entries whose node is not in the graph.
    /// </summary>
    private void ReconcileSurface()
    {
        HashSet<Guid> nodeIds = new();
        foreach (Node node in Graph.Nodes)
        {
            nodeIds.Add(node.Id);
            if (!Surface.TryGetNode(node.Id, out _))
            {
                Surface.SetNode(node.Id, new SurfaceNode(new SurfacePosition(0, 0)));
            }
        }

        foreach (Guid nodeId in Surface.Nodes.Keys.ToList())
        {
            if (!nodeIds.Contains(nodeId))
            {
                Surface.RemoveNode(nodeId);
            }
        }
    }
}
