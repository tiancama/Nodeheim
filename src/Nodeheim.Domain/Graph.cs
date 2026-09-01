namespace Nodeheim.Domain;

public class Graph
{
    private readonly HashSet<Node> _nodes = new();

    public IReadOnlySet<Node> Nodes => _nodes;

    public void AddNode(Node node) => _nodes.Add(node);

    public void RemoveNode(Node node)
    {
        foreach (Node n in node.Neighbors.ToList())
            Disconnect(node, n);

        _nodes.Remove(node);
    }

    public void Connect(Node a, Node b)
    {
        if (a.Equals(b)) return;
        if (!_nodes.Contains(a) || !_nodes.Contains(b))
            throw new ArgumentException("Only registered nodes can be connected");

        a.AddNeighbor(b);
        b.AddNeighbor(a);
    }

    public void Disconnect(Node a, Node b)
    {
        if (!_nodes.Contains(a) || !_nodes.Contains(b))
            throw new ArgumentException("Only registered nodes can be disconnected");

        a.RemoveNeighbor(b);
        b.RemoveNeighbor(a);
    }
}
