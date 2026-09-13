namespace Nodeheim.Editor;

public sealed record class ConnectionViewModel
{
    internal NodeViewModel NodeA { get; }
    internal NodeViewModel NodeB { get; }

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
