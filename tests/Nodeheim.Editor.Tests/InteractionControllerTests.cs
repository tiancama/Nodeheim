using Nodeheim.Domain;

namespace Nodeheim.Editor.Tests;

public class InteractionControllerTests
{
    [Fact]
    public void CreateNode_WhenIdle_DelegatesToCreateNode()
    {
        var fake = new FakeEditorOperations();
        var controller = new InteractionController(fake);

        controller.CreateNode(new SurfacePosition(0, 0));

        Assert.Contains(nameof(IEditorOperations.CreateNode), fake.Calls);
    }

    [Fact]
    public void DeleteNodes_WhenIdle_DelegatesToDeleteSelectedNodes()
    {
        var fake = new FakeEditorOperations();
        var controller = new InteractionController(fake);

        controller.DeleteNodes();

        Assert.Contains(nameof(IEditorOperations.DeleteSelectedNodes), fake.Calls);
    }

    [Fact]
    public void ConnectNodes_WhenIdle_DelegatesToConnectSelectedNodes()
    {
        var fake = new FakeEditorOperations();
        var controller = new InteractionController(fake);

        controller.ConnectNodes();

        Assert.Contains(nameof(IEditorOperations.ConnectSelectedNodes), fake.Calls);
    }

    [Fact]
    public void DisconnectNodes_WhenIdle_DelegatesToDisconnectSelectedNodes()
    {
        var fake = new FakeEditorOperations();
        var controller = new InteractionController(fake);

        controller.DisconnectNodes();

        Assert.Contains(nameof(IEditorOperations.DisconnectSelectedNodes), fake.Calls);
    }

    [Fact]
    public void Exclusive_WithEmptyHit_DelegatesToDeselectAll()
    {
        var fake = new FakeEditorOperations();
        var controller = new InteractionController(fake);

        controller.Exclusive(new EmptyHit());

        Assert.Contains(nameof(IEditorOperations.DeselectAll), fake.Calls);
    }

    [Fact]
    public void Toggle_WithNodeHit_DelegatesToToggleSelected()
    {
        var fake = new FakeEditorOperations();
        var controller = new InteractionController(fake);
        var node = new NodeViewModel(new Node());

        controller.Toggle(new NodeHit(node));

        Assert.Contains(nameof(IEditorOperations.ToggleSelected), fake.Calls);
    }

    [Fact]
    public void Toggle_WithConnectionHit_DelegatesToToggleAsGroup()
    {
        var fake = new FakeEditorOperations();
        var controller = new InteractionController(fake);
        var connection = new ConnectionViewModel(
            new NodeViewModel(new Node()),
            new NodeViewModel(new Node()));

        controller.Toggle(new ConnectionHit(connection));

        Assert.Contains(nameof(IEditorOperations.ToggleAsGroup), fake.Calls);
    }

    [Fact]
    public void PointerMoved_WhileDraggingSingleNode_ShiftsNodeByPointerOffset()
    {
        var startNode = new SurfacePosition(100, 100);
        var grabPosition = new SurfacePosition(107, 113);
        var movedPosition = new SurfacePosition(160, 199);
        var offset = new SurfacePosition(movedPosition.X - grabPosition.X, // 160 - 107 = 53
            movedPosition.Y - grabPosition.Y); // 199 - 113 = 86
        var expected = new SurfacePosition(startNode.X + offset.X, //100 + 53 = 153
            startNode.Y + offset.Y); // 100 + 86 = 186

        var fake = new FakeEditorOperations();
        var controller = new InteractionController(fake);
        var node = new NodeViewModel(new Node()) { X = startNode.X, Y = startNode.Y };

        controller.Grab(new NodeHit(node), grabPosition);
        controller.PointerMoved(movedPosition);

        Assert.Equal(expected.X, node.X);
        Assert.Equal(expected.Y, node.Y);
    }

    [Fact]
    public void PointerMoved_WhileDraggingConnection_ShiftsConnectedNodesByPointerOffset()
    {
        var startNodeA = new SurfacePosition(100, 100);
        var startNodeB = new SurfacePosition(142, 166);
        var grabPosition = new SurfacePosition(107, 113);
        var movedPosition = new SurfacePosition(160, 199);
        var offset = new SurfacePosition(movedPosition.X - grabPosition.X, // 160 - 107 = 53
            movedPosition.Y - grabPosition.Y); // 199 - 113 = 86
        var expectedA = new SurfacePosition(startNodeA.X + offset.X, // 100 + 53 = 153
            startNodeA.Y + offset.Y); // 100 + 86 = 186
        var expectedB = new SurfacePosition(startNodeB.X + offset.X, // 142 + 53 = 195
            startNodeB.Y + offset.Y); // 166 + 86 = 252

        var fake = new FakeEditorOperations();
        var controller = new InteractionController(fake);

        var nodeA = new NodeViewModel(new Node()) { X = startNodeA.X, Y = startNodeA.Y };
        var nodeB = new NodeViewModel(new Node()) { X = startNodeB.X, Y = startNodeB.Y };
        var connection = new ConnectionViewModel(nodeA, nodeB);

        controller.Grab(new ConnectionHit(connection), grabPosition);
        controller.PointerMoved(movedPosition);

        Assert.Equal(expectedA.X, nodeA.X);
        Assert.Equal(expectedA.Y, nodeA.Y);
        Assert.Equal(expectedB.X, nodeB.X);
        Assert.Equal(expectedB.Y, nodeB.Y);
    }

    [Fact]
    public void PointerReleased_AfterGrabbingSelectedNodeWithoutMoving_CollapsesSelectionToThatNode()
    {
        var fake = new FakeEditorOperations();
        var controller = new InteractionController(fake);
        var nodeA = new NodeViewModel(new Node());
        var nodeB = new NodeViewModel(new Node());

        controller.Toggle(new NodeHit(nodeA));
        controller.Toggle(new NodeHit(nodeB));

        controller.Grab(new NodeHit(nodeA), new SurfacePosition(0, 0));
        controller.PointerReleased();

        Assert.True(fake.IsInSelection(nodeA));
        Assert.False(fake.IsInSelection(nodeB));
    }

    [Fact]
    public void PointerMoved_AfterGrabbingSelectedNode_BeginsDragInsteadOfCollapsing()
    {
        var fake = new FakeEditorOperations();
        var controller = new InteractionController(fake);
        var node = new NodeViewModel(new Node()) { X = 100, Y = 100 };

        controller.Toggle(new NodeHit(node));

        controller.Grab(new NodeHit(node), new SurfacePosition(100, 100));
        controller.PointerMoved(new SurfacePosition(120, 140)); // First PointerMoved triggers transition to Dragging
        controller.PointerMoved(new SurfacePosition(140, 170)); // Now in Dragging, so this move is applied

        Assert.Equal(140, node.X);
        Assert.Equal(170, node.Y);
    }
}

file sealed class FakeEditorOperations : IEditorOperations
{
    private readonly HashSet<NodeViewModel> _selection = new();

    public List<string> Calls { get; } = new();

    public bool IsInSelection(NodeViewModel node) => _selection.Contains(node);

    public void SelectOnly(NodeViewModel node)
    {
        Calls.Add(nameof(SelectOnly));
        _selection.Clear();
        _selection.Add(node);
    }

    public void SelectOnly(NodeViewModel nodeA, NodeViewModel nodeB)
    {
        Calls.Add(nameof(SelectOnly));
        _selection.Clear();
        _selection.Add(nodeA);
        _selection.Add(nodeB);
    }

    public void ToggleSelected(NodeViewModel node)
    {
        Calls.Add(nameof(ToggleSelected));
        if (!_selection.Add(node))
            _selection.Remove(node);
    }

    public void ToggleAsGroup(NodeViewModel nodeA, NodeViewModel nodeB)
    {
        Calls.Add(nameof(ToggleAsGroup));
        if (_selection.Contains(nodeA) && _selection.Contains(nodeB))
        {
            _selection.Remove(nodeA);
            _selection.Remove(nodeB);
        }
        else
        {
            _selection.Add(nodeA);
            _selection.Add(nodeB);
        }
    }

    public void DeselectAll()
    {
        Calls.Add(nameof(DeselectAll));
        _selection.Clear();
    }

    public void MoveNode(NodeViewModel node, SurfacePosition position)
    {
        Calls.Add(nameof(MoveNode));
        node.X = position.X;
        node.Y = position.Y;
    }

    public void CreateNode(SurfacePosition position) => Calls.Add(nameof(CreateNode));
    public void DeleteSelectedNodes() => Calls.Add(nameof(DeleteSelectedNodes));
    public void ConnectSelectedNodes() => Calls.Add(nameof(ConnectSelectedNodes));
    public void DisconnectSelectedNodes() => Calls.Add(nameof(DisconnectSelectedNodes));

    public IReadOnlyDictionary<NodeViewModel, SurfacePosition> SnapshotSelectionPositions() =>
        _selection.ToDictionary(n => n, n => new SurfacePosition(n.X, n.Y));
}
