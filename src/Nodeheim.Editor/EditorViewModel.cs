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

    /// <summary>
    /// Toggles the selection state of a node: selects it if currently unselected, deselects it otherwise.
    /// </summary>
    /// <param name="node">The node whose selection state is flipped.</param>
    public void ToggleSelected(NodeViewModel node) => SetSelected(node, !_selectedNodes.Contains(node));

    /// <summary>
    /// Creates a node at the given position and selects it as the sole selection.
    /// </summary>
    /// <param name="position">The center position of the new node.</param>
    public void CreateNode(SurfacePosition position)
    {
        var node = new Node();
        _graph.AddNode(node);
        NodeViewModel nodeViewModel = new(node) { X = position.X, Y = position.Y };
        Nodes.Add(nodeViewModel);
        SelectOnly(nodeViewModel);
    }

    /// <summary>
    /// Removes every selected node from the graph and the view, clearing the selection.
    /// </summary>
    public void DeleteSelectedNodes()
    {
        var selectedNodes = _selectedNodes.ToList();
        DeselectAll();
        foreach (NodeViewModel node in selectedNodes)
        {
            _graph.RemoveNode(node.Model);
            Nodes.Remove(node);
        }
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
