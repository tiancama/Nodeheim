namespace Nodeheim.Editor.Domain;

/// <summary>
/// Represents the editor's model of the surface: an entry for each node, keyed by node ID, and the defaults
/// for nodes without values of their own.
/// </summary>
/// <remarks>
/// The surface refers to nodes only by their ID; it does not know the graph.
/// </remarks>
public class Surface
{
    private const double ProgramDefaultNodeRadius = 13;

    private readonly Dictionary<Guid, SurfaceNode> _nodes = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="Surface"/> class with the program's default node radius.
    /// </summary>
    public Surface()
        : this(ProgramDefaultNodeRadius)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="Surface"/> class.
    /// </summary>
    /// <param name="defaultNodeRadius">The radius of nodes without a radius of their own.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="defaultNodeRadius"/> is zero or negative.
    /// </exception>
    public Surface(double defaultNodeRadius)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(defaultNodeRadius);

        DefaultNodeRadius = defaultNodeRadius;
    }

    /// <summary>
    /// Gets the radius of nodes without a radius of their own.
    /// </summary>
    public double DefaultNodeRadius { get; }

    /// <summary>
    /// Gets the entries of all nodes on the surface, keyed by node ID.
    /// </summary>
    public IReadOnlyDictionary<Guid, SurfaceNode> Nodes => _nodes;

    /// <summary>
    /// Gets the entry of the node with the specified ID.
    /// </summary>
    /// <param name="nodeId">The ID of the node.</param>
    /// <param name="node">
    /// When this method returns, the entry of the node if it exists; otherwise, the default value.
    /// </param>
    /// <returns>
    /// <see langword="true"/> if the surface has an entry for the node; otherwise, <see langword="false"/>.
    /// </returns>
    public bool TryGetNode(Guid nodeId, out SurfaceNode node) =>
        _nodes.TryGetValue(nodeId, out node);

    /// <summary>
    /// Sets the entry of the node with the specified ID, adding it if it does not exist and replacing it
    /// otherwise.
    /// </summary>
    /// <param name="nodeId">The ID of the node.</param>
    /// <param name="node">The new entry of the node.</param>
    public void SetNode(Guid nodeId, SurfaceNode node) => _nodes[nodeId] = node;

    /// <summary>
    /// Removes the entry of the node with the specified ID.
    /// </summary>
    /// <param name="nodeId">The ID of the node.</param>
    /// <remarks>
    /// Does nothing if the surface has no entry for the node.
    /// </remarks>
    public void RemoveNode(Guid nodeId) => _nodes.Remove(nodeId);
}
