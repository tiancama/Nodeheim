namespace Nodeheim.Domain;

public class Node
{
    private readonly HashSet<Node> _neighbors = new HashSet<Node>();
    public IReadOnlySet<Node> Neighbors => _neighbors;
    internal void AddNeighbor(Node other) => _neighbors.Add(other);
    internal void RemoveNeighbor(Node other) => _neighbors.Remove(other);
    
    public Guid Id {get; init;} = Guid.NewGuid();
}