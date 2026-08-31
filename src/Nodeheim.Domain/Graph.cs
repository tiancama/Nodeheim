namespace Nodeheim.Domain;

public class Graph
{
    private readonly HashSet<Node> _graph = new HashSet<Node>();
    
    public void AddNode(Node node)
    {
        _graph.Add(node);
    }
    
    public void Connect(Node a, Node b)
    {
        if (a.Equals(b)) return;
        if (!_graph.Contains(a) || !_graph.Contains(b))
            throw new ArgumentException("Only registered nodes can be connected");
        
        a.AddNeighbor(b);
        b.AddNeighbor(a);
    }
}
