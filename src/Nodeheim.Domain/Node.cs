namespace Nodeheim.Domain;

/// <summary>
/// Represents a point in the graph that holds references to its neighbors.
/// </summary>
/// <remarks>
/// Neighbor relations are mutual and are changed only by <see cref="Graph"/>.
/// </remarks>
public class Node
{
    private readonly HashSet<Node> _neighbors = new();

    /// <summary>
    /// Gets the nodes directly connected to this node.
    /// </summary>
    public IReadOnlySet<Node> Neighbors => _neighbors;

    /// <summary>
    /// Gets the unique identifier of this node.
    /// </summary>
    /// <remarks>
    /// A new identifier is assigned on creation. It can be set on initialization,
    /// for example when a node is restored from storage.
    /// </remarks>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Adds a one-sided neighbor reference. Intended for use by <see cref="Graph"/> only.
    /// </summary>
    /// <param name="other">The node to add as a neighbor.</param>
    /// <returns>
    /// <see langword="true"/> if the reference was added; otherwise, <see langword="false"/>.
    /// </returns>
    internal bool AddNeighbor(Node other) => _neighbors.Add(other);

    /// <summary>
    /// Removes a one-sided neighbor reference. Intended for use by <see cref="Graph"/> only.
    /// </summary>
    /// <param name="other">The node to remove as a neighbor.</param>
    /// <returns>
    /// <see langword="true"/> if the reference was removed; otherwise, <see langword="false"/>.
    /// </returns>
    internal bool RemoveNeighbor(Node other) => _neighbors.Remove(other);
}
