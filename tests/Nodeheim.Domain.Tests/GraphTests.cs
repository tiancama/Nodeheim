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
    
    [Fact]
    public void ConnectNodeWithItselfMakesNoNeighbor()
    {
        var graph = new Graph();
        var a = new Node();
        
        graph.Connect(a, a);
        
        Assert.Empty(a.Neighbors);
    }

    [Fact]
    public void ConnectAgainWithSameNodeDoesNotDuplicate()
    {
        var graph = new Graph();
        var a = new Node();
        var b = new Node();
        
        graph.Connect(a, b);
        graph.Connect(a, b);
        
        Assert.Single(a.Neighbors);
        Assert.Single(b.Neighbors);
    }
}