using Nodeheim.Domain;

namespace Nodeheim.Domain.Tests;

public class NodeTests
{
    [Fact]
    public void TwoNodesHaveDifferentIds()
    {
        Node a = new Node();
        Node b = new Node();
        Assert.NotEqual(a.Id, b.Id);
    }
    
    [Fact]
    public void NodeIdIsNotEmpty()
    {
        Node a = new Node();
        Assert.NotEqual(a.Id, Guid.Empty);
    }
}