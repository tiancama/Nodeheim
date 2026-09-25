namespace Nodeheim.Editor;

/// <summary>
/// Defines the editor operations that the interaction states use.
/// </summary>
/// <remarks>
/// Separates the framework-free interaction layer from the view model.
/// </remarks>
public interface IEditorOperations
{
    /// <summary>
    /// Determines whether the given node is part of the current selection.
    /// </summary>
    /// <param name="node">The node to test.</param>
    /// <returns>
    /// <see langword="true"/> if the node is selected; otherwise, <see langword="false"/>.
    /// </returns>
    bool IsInSelection(NodeViewModel node);

    /// <summary>
    /// Selects only the given node, clearing any previous selection.
    /// </summary>
    /// <param name="node">The node to select.</param>
    void SelectOnly(NodeViewModel node);

    /// <summary>
    /// Selects only the two given nodes, clearing any previous selection.
    /// </summary>
    /// <param name="nodeA">The first node to select.</param>
    /// <param name="nodeB">The second node to select.</param>
    void SelectOnly(NodeViewModel nodeA, NodeViewModel nodeB);

    /// <summary>
    /// Toggles the selection state of a node.
    /// </summary>
    /// <param name="node">The node whose selection state is toggled.</param>
    void ToggleSelected(NodeViewModel node);

    /// <summary>
    /// Toggles two nodes as a group: if both are selected, both are deselected;
    /// otherwise, both are selected.
    /// </summary>
    /// <param name="nodeA">The first node of the group.</param>
    /// <param name="nodeB">The second node of the group.</param>
    void ToggleAsGroup(NodeViewModel nodeA, NodeViewModel nodeB);

    /// <summary>
    /// Clears the selection.
    /// </summary>
    void DeselectAll();

    /// <summary>
    /// Moves a node to the given position.
    /// </summary>
    /// <param name="node">The node to move.</param>
    /// <param name="position">The new center position of the node.</param>
    void MoveNode(NodeViewModel node, SurfacePosition position);

    /// <summary>
    /// Creates a node at the given position and makes it the only selected node.
    /// </summary>
    /// <param name="position">The center position of the new node.</param>
    void CreateNode(SurfacePosition position);

    /// <summary>
    /// Deletes all selected nodes together with their connections and clears the selection.
    /// </summary>
    void DeleteSelectedNodes();

    /// <summary>
    /// Connects every pair of selected nodes that is not yet connected.
    /// </summary>
    /// <remarks>
    /// Does nothing if fewer than two nodes are selected.
    /// </remarks>
    void ConnectSelectedNodes();

    /// <summary>
    /// Disconnects every connected pair of selected nodes.
    /// </summary>
    /// <remarks>
    /// Connections to unselected nodes are kept. Does nothing if fewer than two nodes are selected.
    /// </remarks>
    void DisconnectSelectedNodes();

    /// <summary>
    /// Captures the current position of every selected node.
    /// </summary>
    /// <returns>
    /// A dictionary that maps each selected node to its position at the time of the call.
    /// </returns>
    IReadOnlyDictionary<NodeViewModel, SurfacePosition> SnapshotSelectionPositions();
}
