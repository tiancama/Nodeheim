namespace Nodeheim.Domain;

/// <summary>
/// Represents a set of nodes and manages the neighbor relations between them.
/// </summary>
/// <remarks>
/// The graph is the only place where topology changes, which keeps every
/// neighbor relation mutual.
/// </remarks>
public class Graph
{
    private readonly HashSet<Node> _nodes = new();

    /// <summary>
    /// Gets the nodes registered in this graph.
    /// </summary>
    public IReadOnlySet<Node> Nodes => _nodes;

    /// <summary>
    /// Registers a node in this graph.
    /// </summary>
    /// <param name="node">The node to register.</param>
    /// <remarks>
    /// Registering a node that is already registered has no effect.
    /// </remarks>
    public void AddNode(Node node) => _nodes.Add(node);

    /// <summary>
    /// Removes a node from this graph and disconnects it from all of its neighbors.
    /// </summary>
    /// <param name="node">The node to remove.</param>
    /// <remarks>
    /// Removing a node that is not registered has no effect.
    /// </remarks>
    public void RemoveNode(Node node)
    {
        foreach (Node n in node.Neighbors.ToList())
        {
            Disconnect(node, n);
        }

        _nodes.Remove(node);
    }

    /// <summary>
    /// Connects two registered nodes as mutual neighbors.
    /// </summary>
    /// <param name="a">One endpoint of the connection.</param>
    /// <param name="b">The other endpoint of the connection.</param>
    /// <returns>
    /// <see langword="true"/> if the nodes were newly connected; otherwise, <see langword="false"/>,
    /// for example when they were already connected or when <paramref name="a"/> and
    /// <paramref name="b"/> are the same node.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="a"/> or <paramref name="b"/> is not registered in this graph.
    /// </exception>
    public bool Connect(Node a, Node b)
    {
        if (a.Equals(b)) return false;
        if (!_nodes.Contains(a) || !_nodes.Contains(b))
            throw new ArgumentException("Only registered nodes can be connected");

        bool connected = a.AddNeighbor(b);
        b.AddNeighbor(a);
        return connected;
    }

    /// <summary>
    /// Disconnects two registered nodes, removing the mutual neighbor relation.
    /// </summary>
    /// <param name="a">One endpoint of the connection.</param>
    /// <param name="b">The other endpoint of the connection.</param>
    /// <returns>
    /// <see langword="true"/> if the nodes were connected and are now disconnected;
    /// otherwise, <see langword="false"/>, for example when they were already
    /// disconnected or when <paramref name="a"/> and  <paramref name="b"/> are the same node.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// <paramref name="a"/> or <paramref name="b"/> is not registered in this graph.
    /// </exception>
    public bool Disconnect(Node a, Node b)
    {
        if (!_nodes.Contains(a) || !_nodes.Contains(b))
            throw new ArgumentException("Only registered nodes can be disconnected");

        bool disconnected = a.RemoveNeighbor(b);
        b.RemoveNeighbor(a);
        return disconnected;
    }
}
