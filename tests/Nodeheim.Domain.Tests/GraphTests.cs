namespace Nodeheim.Domain.Tests;

public class GraphTests
{
    [Fact]
    public void ConnectMakesNodesNeighbors()
    {
        var graph = new Graph();
        var a = new Node();
        var b = new Node();
        
        graph.Connect(a, b);
        
        Assert.Contains(b, a.Neighbors);
        Assert.Contains(a, b.Neighbors);
    }
}