namespace Nodeheim.Domain;

public class Node
{
    private readonly HashSet<Node> _neighbors = new();
    public IReadOnlySet<Node> Neighbors => _neighbors;

    public Guid Id { get; init; } = Guid.NewGuid();
    internal void AddNeighbor(Node other) => _neighbors.Add(other);
    internal void RemoveNeighbor(Node other) => _neighbors.Remove(other);
}
