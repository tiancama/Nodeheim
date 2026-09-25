using System.Collections.ObjectModel;
using Nodeheim.Domain;

namespace Nodeheim.Editor;

/// <summary>
/// Represents the state of the editor: the nodes and connections on the surface
/// and the current selection.
/// </summary>
public class EditorViewModel : IEditorOperations
{
    private readonly Graph _graph = new();
    private readonly ObservableCollection<NodeViewModel> _selectedNodes = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="EditorViewModel"/> class with an empty graph.
    /// </summary>
    public EditorViewModel()
    {
        SelectedNodes = new ReadOnlyObservableCollection<NodeViewModel>(_selectedNodes);
    }

    /// <summary>
    /// Gets the currently selected nodes.
    /// </summary>
    public ReadOnlyObservableCollection<NodeViewModel> SelectedNodes { get; }

    /// <summary>
    /// Gets the nodes on the editor surface.
    /// </summary>
    public ObservableCollection<NodeViewModel> Nodes { get; } = new();

    /// <summary>
    /// Gets the connections on the editor surface.
    /// </summary>
    public ObservableCollection<ConnectionViewModel> Connections { get; } = new();

    /// <inheritdoc />
    public bool IsInSelection(NodeViewModel node) => _selectedNodes.Contains(node);

    /// <inheritdoc />
    public void SelectOnly(NodeViewModel node)
    {
        DeselectAll();
        SetSelected(node, true);
    }

    /// <inheritdoc />
    public void SelectOnly(NodeViewModel nodeA, NodeViewModel nodeB)
    {
        DeselectAll();
        SetSelected(nodeA, true);
        SetSelected(nodeB, true);
    }

    /// <inheritdoc />
    public void ToggleSelected(NodeViewModel node) => SetSelected(node, !_selectedNodes.Contains(node));

    /// <inheritdoc />
    public void ToggleAsGroup(NodeViewModel nodeA, NodeViewModel nodeB)
    {
        if (_selectedNodes.Contains(nodeA) && _selectedNodes.Contains(nodeB))
        {
            SetSelected(nodeA, false);
            SetSelected(nodeB, false);
        }
        else
        {
            SetSelected(nodeA, true);
            SetSelected(nodeB, true);
        }
    }

    /// <inheritdoc />
    public void DeselectAll()
    {
        foreach (NodeViewModel node in _selectedNodes.ToList())
        {
            SetSelected(node, false);
        }
    }

    /// <inheritdoc />
    public void MoveNode(NodeViewModel node, SurfacePosition position)
    {
        node.X = position.X;
        node.Y = position.Y;
    }

    /// <inheritdoc />
    public void CreateNode(SurfacePosition position)
    {
        var node = new Node();
        _graph.AddNode(node);
        NodeViewModel nodeViewModel = new(node) { X = position.X, Y = position.Y };
        Nodes.Add(nodeViewModel);
        SelectOnly(nodeViewModel);
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public void ConnectSelectedNodes()
    {
        if (_selectedNodes.Count <= 1) return;

        for (int i = 0; i < _selectedNodes.Count - 1; i++)
        {
            for (int j = i + 1; j < _selectedNodes.Count; j++)
            {
                CreateConnection(_selectedNodes[i], _selectedNodes[j]);
            }
        }
    }

    /// <inheritdoc />
    public void DisconnectSelectedNodes()
    {
        if (_selectedNodes.Count <= 1) return;

        for (int i = 0; i < _selectedNodes.Count - 1; i++)
            for (int j = i + 1; j < _selectedNodes.Count; j++)
            {
                DeleteConnection(_selectedNodes[i], _selectedNodes[j]);
            }
    }

    /// <inheritdoc />
    public IReadOnlyDictionary<NodeViewModel, SurfacePosition> SnapshotSelectionPositions()
    {
        Dictionary<NodeViewModel, SurfacePosition> origins = new();
        foreach (NodeViewModel node in _selectedNodes)
        {
            origins.Add(node, new SurfacePosition(node.X, node.Y));
        }

        return origins;
    }

    /// <summary>
    /// Selects or deselects a node, keeping the selection and the node's
    /// <see cref="NodeViewModel.IsSelected"/> flag in sync.
    /// </summary>
    /// <param name="node">The node to select or deselect.</param>
    /// <param name="selected">
    /// <see langword="true"/> to select the node; <see langword="false"/> to deselect it.
    /// </param>
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
    /// Removes every connection that involves the given node from the view projection.
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
}
