namespace Nodeheim.Domain;

public class Graph
{
    public void Connect(Node a, Node b)
    {
        a.AddNeighbor(b);
        b.AddNeighbor(a);
    }
}