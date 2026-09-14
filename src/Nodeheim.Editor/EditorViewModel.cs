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

        CreateNode(new SurfacePosition(666, 256));
        CreateNode(new SurfacePosition(708, 256));
        CreateNode(new SurfacePosition(750, 256));
        CreateConnection(Nodes.ElementAt(0), Nodes.ElementAt(1));
        DeselectAll();
    }

    public ReadOnlyObservableCollection<NodeViewModel> SelectedNodes { get; }

    public ObservableCollection<NodeViewModel> Nodes { get; } = new();
    public ObservableCollection<ConnectionViewModel> Connections { get; } = new();

    /// <summary>
    /// Finds every connection that contains the given node and removes it from the view projection.
    /// </summary>
    /// <param name="node">The node whose connections are removed.</param>
    private void RemoveConnectionsOf(NodeViewModel node)
    {
        foreach (ConnectionViewModel connection in Connections.ToList())
        {
            if (connection.NodeA == node || connection.NodeB == node)
            {
                Connections.Remove(connection);
            }
        }
    }

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
    /// Connects two nodes in the graph and adds the corresponding connection to the view
    /// projection. The connection is only projected if the nodes were not already connected.
    /// </summary>
    /// <param name="nodeA">One endpoint of the connection.</param>
    /// <param name="nodeB">The other endpoint of the connection.</param>
    private void CreateConnection(NodeViewModel nodeA, NodeViewModel nodeB)
    {
        if (_graph.Connect(nodeA.Model, nodeB.Model))
        {
            Connections.Add(new ConnectionViewModel(nodeA, nodeB));
        }
    }

    /// <summary>
    /// Connects every pair among the currently selected nodes, so that the selection becomes
    /// fully interconnected. Pairs that are already connected are left unchanged. Does nothing
    /// if fewer than two nodes are selected.
    /// </summary>
    public void ConnectSelectedNodes()
    {
        if (_selectedNodes.Count <= 1) return;

        for (int i = 0; i < _selectedNodes.Count - 1; i++)
            for (int j = i + 1; j < _selectedNodes.Count; j++)
            {
                CreateConnection(_selectedNodes[i], _selectedNodes[j]);
            }
    }

    /// <summary>
    /// Disconnects two nodes in the graph and removes the corresponding connection from the
    /// view projection. The connection is only removed if the nodes were actually connected.
    /// </summary>
    /// <param name="nodeA">One endpoint of the connection.</param>
    /// <param name="nodeB">The other endpoint of the connection.</param>
    private void DeleteConnection(NodeViewModel nodeA, NodeViewModel nodeB)
    {
        if (_graph.Disconnect(nodeA.Model, nodeB.Model))
        {
            Connections.Remove(new ConnectionViewModel(nodeA, nodeB));
        }
    }

    /// <summary>
    /// Disconnects every connected pair among the currently selected nodes, removing only
    /// connections that run between two selected nodes. A connection from a selected node to
    /// an unselected one is left in place. Does nothing if fewer than two nodes are selected.
    /// </summary>
    public void DisconnectSelectedNodes()
    {
        if (_selectedNodes.Count <= 1) return;

        for (int i = 0; i < _selectedNodes.Count - 1; i++)
            for (int j = i + 1; j < _selectedNodes.Count; j++)
            {
                DeleteConnection(_selectedNodes[i], _selectedNodes[j]);
            }
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
            RemoveConnectionsOf(node);
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
