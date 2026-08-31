namespace Nodeheim.Domain.Tests;

public class GraphTests
{
    [Fact]
    public void ConnectMakesNodesNeighbors()
    {
        var graph = new Graph();
        var a = new Node();
        var b = new Node();
        
        graph.AddNode(a);
        graph.AddNode(b);
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
        
        graph.AddNode(a);
        graph.AddNode(b);
        graph.Connect(a, b);
        graph.Connect(a, b);
        
        Assert.Single(a.Neighbors);
        Assert.Single(b.Neighbors);
    }

    [Fact]
    public void ConnectNodeNotInGraphThrowsException()
    {
        var graph = new Graph();
        var a = new Node();
        var b = new Node();

        graph.AddNode(a);
        
        Assert.Throws<ArgumentException>(() => graph.Connect(a, b));
    }

    [Fact]
    public void DisconnectRemovesNeighbors()
    {
        var graph = new Graph();
        var a = new Node();
        var b = new Node();
        
        graph.AddNode(a);
        graph.AddNode(b);
        graph.Connect(a, b);
        graph.Disconnect(a, b);
        
        Assert.DoesNotContain(b, a.Neighbors);
        Assert.DoesNotContain(a, b.Neighbors);
    }
    
    [Fact]
    public void DisconnectWhenNotConnectedDoesNothing()
    {
        var graph = new Graph();
        var a = new Node();
        var b = new Node();
        
        graph.AddNode(a);
        graph.AddNode(b);
        //graph.Connect(a, b);
        graph.Disconnect(a, b);
        
        Assert.Empty(a.Neighbors);
        Assert.Empty(b.Neighbors);
    }
    
    [Fact]
    public void DisconnectReceivingNodeNotInGraphThrowsException()
    {
        var graph = new Graph();
        var a = new Node();
        var b = new Node();
        graph.AddNode(a);
        //graph.AddNode(b);
        Assert.Throws<ArgumentException>(() => graph.Disconnect(a, b));
    }
    
    [Fact]
    public void RemoveNodeClearsAllNeighbors()
    {
        var graph = new Graph();
        var a = new Node();
        var b = new Node();
        var c = new Node();
        
        graph.AddNode(a);
        graph.AddNode(b);
        graph.AddNode(c);
        graph.Connect(a, b);
        graph.Connect(a, c);
        graph.Connect(b, c);
        
        graph.RemoveNode(a);
        
        Assert.Empty(a.Neighbors);
        Assert.DoesNotContain(a, b.Neighbors);
        Assert.DoesNotContain(a, c.Neighbors);
    }
}
