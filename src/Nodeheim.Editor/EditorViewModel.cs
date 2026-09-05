using System.Collections.ObjectModel;
using Nodeheim.Domain;

namespace Nodeheim.Editor;

public class EditorViewModel
{
    private readonly Graph _graph = new();
    private readonly HashSet<NodeViewModel> _selectedNodes = new();
    private readonly Dictionary<NodeViewModel, SurfacePosition> _dragOrigins = new();
    private SurfacePosition _pointerPressedPosition;

    public EditorViewModel()
    {
        _graph.AddNode(new Node());
        _graph.AddNode(new Node());
        _graph.AddNode(new Node());
        int startX = 666;
        foreach (var node in _graph.Nodes)
        {
            NodeViewModel nodeViewModel = new(node) { X = startX, Y = 256 };
            startX += 42;
            Nodes.Add(nodeViewModel);
        }
    }

    public ObservableCollection<NodeViewModel> Nodes { get; } = new();

    public void ClearSelection()
    {
        foreach (var node in _selectedNodes)
            node.IsSelected = false;

        _selectedNodes.Clear();
    }

    public void SelectSingleNode(NodeViewModel selectedNode)
    {
        ClearSelection();

        selectedNode.IsSelected = true;
        _selectedNodes.Add(selectedNode);
    }

    public void BeginDrag(SurfacePosition position)
    {
        _pointerPressedPosition = position;
        _dragOrigins.Clear();
        foreach (NodeViewModel node in _selectedNodes)
        {
            SurfacePosition surfacePosition = new(node.X, node.Y);
            _dragOrigins.Add(node, surfacePosition);
        }
    }

    public void UpdateDrag(SurfacePosition position)
    {
        foreach (KeyValuePair<NodeViewModel, SurfacePosition> nodePair in _dragOrigins)
        {
            nodePair.Key.X = nodePair.Value.X + (position.X - _pointerPressedPosition.X);
            nodePair.Key.Y = nodePair.Value.Y + (position.Y - _pointerPressedPosition.Y);
        }
    }

    public void EndDrag() => _dragOrigins.Clear();
}
