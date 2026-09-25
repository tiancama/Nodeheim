using Nodeheim.Domain;

namespace Nodeheim.Editor.Tests;

public class NodeViewModelTests
{
    [Fact]
    public void X_SetToDifferentValue_RaisesPropertyChanged()
    {
        var node = new NodeViewModel(new Node()) { X = 10 };
        var raised = new List<string?>();
        node.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        node.X = 20;

        string? name = Assert.Single(raised);
        Assert.Equal(nameof(NodeViewModel.X), name);
    }

    [Fact]
    public void X_SetToSameValue_DoesNotRaisePropertyChanged()
    {
        var node = new NodeViewModel(new Node()) { X = 10 };
        var raised = new List<string?>();
        node.PropertyChanged += (_, e) => raised.Add(e.PropertyName);

        node.X = 10;

        Assert.Empty(raised);
    }
}
