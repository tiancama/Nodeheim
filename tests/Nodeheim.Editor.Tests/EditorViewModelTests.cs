using Nodeheim.Domain;

namespace Nodeheim.Editor.Tests;

public class EditorViewModelTests
{
    [Fact]
    public void BeginDrag_InvokedTwice_DoesNotThrow()
    {
        var editor = new EditorViewModel();
        var point = new SurfacePosition(10, 10);
        editor.SelectOnly(new NodeViewModel(new Node()));
        editor.BeginDrag(point);
        Exception? exception = Record.Exception(() => editor.BeginDrag(point));
        Assert.Null(exception);
    }

    [Fact]
    public void SelectOnly_WithSingleNode_SelectsOnlyThatNode()
    {
        var editor = new EditorViewModel();
        var node = new NodeViewModel(new Node());

        editor.SelectOnly(node);

        Assert.Single(editor.SelectedNodes);
        Assert.Contains(node, editor.SelectedNodes);
        Assert.True(node.IsSelected);
    }

    [Fact]
    public void SelectOnly_CalledTwiceWithSameNode_KeepsSingleSelection()
    {
        var editor = new EditorViewModel();
        var node = new NodeViewModel(new Node());

        editor.SelectOnly(node);
        editor.SelectOnly(node);

        Assert.Single(editor.SelectedNodes);
    }

    [Fact]
    public void DeselectAll_WithSelectedNode_ClearsSelectionAndResetsFlag()
    {
        var editor = new EditorViewModel();
        var node = new NodeViewModel(new Node());
        editor.SelectOnly(node);

        editor.DeselectAll();

        Assert.Empty(editor.SelectedNodes);
        Assert.False(node.IsSelected);
    }

    [Fact]
    public void ToggleSelected_OnUnselectedNode_AddsToSelection()
    {
        var editor = new EditorViewModel();
        var node = new NodeViewModel(new Node());

        editor.ToggleSelected(node);

        Assert.Contains(node, editor.SelectedNodes);
        Assert.True(node.IsSelected);
    }

    [Fact]
    public void ToggleSelected_OnSelectedNode_RemovesFromSelection()
    {
        var editor = new EditorViewModel();
        var node = new NodeViewModel(new Node());
        editor.ToggleSelected(node);

        editor.ToggleSelected(node);

        Assert.DoesNotContain(node, editor.SelectedNodes);
        Assert.False(node.IsSelected);
    }

    [Fact]
    public void ToggleSelected_WithExistingSelection_KeepsBothWithoutDuplication()
    {
        var editor = new EditorViewModel();
        var first = new NodeViewModel(new Node());
        var second = new NodeViewModel(new Node());

        editor.ToggleSelected(first);
        editor.ToggleSelected(second);

        Assert.Equal(2, editor.SelectedNodes.Count);
        Assert.Contains(first, editor.SelectedNodes);
        Assert.Contains(second, editor.SelectedNodes);
    }
}
