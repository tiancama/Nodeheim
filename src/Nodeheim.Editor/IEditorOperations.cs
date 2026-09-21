namespace Nodeheim.Editor;

/// <summary>
/// The editor operations the interaction states compose. The seam between the framework-free
/// interaction layer and the view model.
/// </summary>
public interface IEditorOperations
{
    bool IsInSelection(NodeViewModel node);

    void SelectOnly(NodeViewModel node);
    void SelectOnly(NodeViewModel nodeA, NodeViewModel nodeB);
    void ToggleSelected(NodeViewModel node);
    void ToggleAsGroup(NodeViewModel nodeA, NodeViewModel nodeB);
    void DeselectAll();
    void MoveNode(NodeViewModel node, SurfacePosition position);
    void CreateNode(SurfacePosition position);
    void DeleteSelectedNodes();
    void ConnectSelectedNodes();
    void DisconnectSelectedNodes();
    IReadOnlyDictionary<NodeViewModel, SurfacePosition> SnapshotSelectionPositions();
}
