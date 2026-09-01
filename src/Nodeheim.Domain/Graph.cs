namespace Nodeheim.Domain;

public class Graph
{
    private readonly HashSet<Node> _graph = new();
    
    public IReadOnlySet<Node> Nodes => _graph;
    
    public void AddNode(Node node)
    {
        _graph.Add(node);
    }

    public void RemoveNode(Node node)
    {
        foreach (var n in node.Neighbors.ToList())
            Disconnect(node, n);
        
        _graph.Remove(node);
    }
    
    public void Connect(Node a, Node b)
    {
        if (a.Equals(b)) return;
        if (!_graph.Contains(a) || !_graph.Contains(b))
            throw new ArgumentException("Only registered nodes can be connected");
        
        a.AddNeighbor(b);
        b.AddNeighbor(a);
    }
    
    public void Disconnect(Node a, Node b)
    {
        if (!_graph.Contains(a) || !_graph.Contains(b))
            throw new ArgumentException("Only registered nodes can be disconnected");
        
        a.RemoveNeighbor(b);
        b.RemoveNeighbor(a);
    }
}
