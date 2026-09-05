using Nodeheim.Domain;

namespace Nodeheim.Editor.Tests;

public class EditorViewModelTests
{
    [Fact]
    public void BeginDrag_InvokedTwice_DoesNotThrow()
    {
        var editor = new EditorViewModel();
        var point = new SurfacePosition(10, 10);
        editor.SelectSingleNode(new NodeViewModel(new Node()));
        editor.BeginDrag(point);
        Exception? exception = Record.Exception(() => editor.BeginDrag(point));
        Assert.Null(exception);
    }
}
