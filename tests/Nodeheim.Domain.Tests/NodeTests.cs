namespace Nodeheim.Domain.Tests;

public class NodeTests
{
    [Fact]
    public void NodeId_WithNewNodes_IsDifferent()
    {
        var a = new Node();
        var b = new Node();
        Assert.NotEqual(a.Id, b.Id);
    }

    [Fact]
    public void NodeId_WithNewNode_IsNotEmpty()
    {
        var a = new Node();
        Assert.NotEqual(a.Id, Guid.Empty);
    }
}
