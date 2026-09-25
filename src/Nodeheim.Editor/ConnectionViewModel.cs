namespace Nodeheim.Editor;

/// <summary>
/// Represents a connection between two nodes on the editor surface.
/// </summary>
/// <remarks>
/// The endpoints are ordered by node identifier, so two instances with swapped endpoints
/// are equal. <see cref="NodeA"/> and <see cref="NodeB"/> carry no direction.
/// </remarks>
public sealed record class ConnectionViewModel
{
    internal NodeViewModel NodeA { get; }
    internal NodeViewModel NodeB { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ConnectionViewModel"/> class.
    /// </summary>
    /// <param name="nodeA">One endpoint of the connection.</param>
    /// <param name="nodeB">The other endpoint of the connection.</param>
    public ConnectionViewModel(NodeViewModel nodeA, NodeViewModel nodeB)
    {
        if (nodeA.Id.CompareTo(nodeB.Id) < 0)
        {
            NodeA = nodeA;
            NodeB = nodeB;
        }
        else
        {
            NodeA = nodeB;
            NodeB = nodeA;
        }
    }
}
