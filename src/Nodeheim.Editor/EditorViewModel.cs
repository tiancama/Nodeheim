using System.Collections.ObjectModel;
using Nodeheim.Domain;

namespace Nodeheim.Editor;

public class EditorViewModel
{
    private readonly Graph _graph = new();
    private readonly ObservableCollection<NodeViewModel> _selectedNodes = new();
    private readonly Dictionary<NodeViewModel, SurfacePosition> _dragOrigins = new();
    private SurfacePosition _pointerPressedPosition;

    public EditorViewModel()
    {
        SelectedNodes = new ReadOnlyObservableCollection<NodeViewModel>(_selectedNodes);

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

    public ReadOnlyObservableCollection<NodeViewModel> SelectedNodes { get; }

    public ObservableCollection<NodeViewModel> Nodes { get; } = new();

    /// <summary>
    /// Core method for both selecting and deselecting a node.
    /// </summary>
    /// <param name="node">The node to be selected or deselected.</param>
    /// <param name="selected"><c>true</c> selects the node; <c>false</c> deselects the node.</param>
    private void SetSelected(NodeViewModel node, bool selected)
    {
        if (selected)
        {
            if (!_selectedNodes.Contains(node))
            {
                _selectedNodes.Add(node);
                node.IsSelected = true;
            }
        }
        else
        {
            _selectedNodes.Remove(node);
            node.IsSelected = false;
        }
    }

    /// <summary>
    /// Clears the selection, resetting the selected flag on every previously selected node.
    /// </summary>
    public void DeselectAll()
    {
        foreach (NodeViewModel node in _selectedNodes.ToList())
            SetSelected(node, false);
    }

    /// <summary>
    /// Sets the selection to exactly this node, clearing any previous selection.
    /// </summary>
    /// <param name="node">The node to become the sole selected node.</param>
    public void SelectOnly(NodeViewModel node)
    {
        DeselectAll();
        SetSelected(node, true);
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
