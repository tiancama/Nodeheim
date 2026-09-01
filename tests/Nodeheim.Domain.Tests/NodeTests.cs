using Nodeheim.Domain;

namespace Nodeheim.Domain.Tests;

public class NodeTests
{
    [Fact]
    public void NodeId_WithNewNodes_IsDifferent()
    {
        Node a = new Node();
        Node b = new Node();
        Assert.NotEqual(a.Id, b.Id);
    }
    
    [Fact]
    public void NodeId_WithNewNode_IsNotEmpty()
    {
        Node a = new Node();
        Assert.NotEqual(a.Id, Guid.Empty);
    }
}