using Nodeheim.Domain;

namespace Nodeheim.Editor.Tests;

public class ConnectionViewModelTests
{
    [Fact]
    public void TwoConnections_WithSwappedEndpoints_AreEqual()
    {
        var a = new NodeViewModel(new Node());
        var b = new NodeViewModel(new Node());

        var connection = new ConnectionViewModel(a, b);
        var swapped = new ConnectionViewModel(b, a);

        Assert.Equal(connection, swapped);
    }
}
