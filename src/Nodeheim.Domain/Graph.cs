namespace Nodeheim.Domain;

public class Graph
{
    private readonly HashSet<Node> _nodes = new();

    public IReadOnlySet<Node> Nodes => _nodes;

    public void AddNode(Node node) => _nodes.Add(node);

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
    /// <c>true</c> if the nodes were newly connected; <c>false</c> if they were already
    /// connected or if <paramref name="a"/> and <paramref name="b"/> are the same node.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if either node is not registered in the graph.
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
    /// <c>true</c> if the nodes were connected and are now disconnected; <c>false</c> if
    /// they were not connected.
    /// </returns>
    /// <exception cref="ArgumentException">
    /// Thrown if either node is not registered in the graph.
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
