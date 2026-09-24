namespace Nodeheim.Editor;

/// <summary>
/// The resting state. Handles selection gestures and selection commands directly, and
/// arbitrates a grab into either a pending click (grab on an already-selected target) or
/// an immediate drag (grab on an unselected target, which becomes the sole selection first).
/// </summary>
public sealed class Idle : InteractionState
{
    public Idle(IEditorOperations operations, InteractionController controller)
        : base(operations, controller)
    { }

    public override void OnExclusive(HitTarget target)
    {
        switch (target)
        {
            case EmptyHit:
                Operations.DeselectAll();
                break;
        }
    }

    public override void OnToggle(HitTarget target)
    {
        switch (target)
        {
            case NodeHit(var node):
                Operations.ToggleSelected(node);
                break;
            case ConnectionHit(var connection):
                Operations.ToggleAsGroup(connection.NodeA, connection.NodeB);
                break;
            case EmptyHit:
                break;
        }
    }

    /// <summary>
    /// Grabs the target. An already-selected target enters <see cref="Pending"/> to await a click
    /// or drag; an unselected target becomes the sole selection and enters <see cref="Dragging"/>
    /// at once.
    /// </summary>
    public override void OnGrab(HitTarget target, SurfacePosition position)
    {
        switch (target)
        {
            case NodeHit(var node):
                if (!Operations.IsInSelection(node))
                {
                    Operations.SelectOnly(node);
                    Controller.TransitionTo(new Dragging(Operations, Controller, position));
                }
                else
                {
                    Controller.TransitionTo(new Pending(Operations, Controller, target, position));
                }

                break;

            case ConnectionHit(var connection):
                if (!Operations.IsInSelection(connection.NodeA) || !Operations.IsInSelection(connection.NodeB))
                {
                    Operations.SelectOnly(connection.NodeA, connection.NodeB);
                    Controller.TransitionTo(new Dragging(Operations, Controller, position));
                }
                else
                {
                    Controller.TransitionTo(new Pending(Operations, Controller, target, position));
                }

                break;

            case EmptyHit:
                break;
        }
    }

    public override void OnCreateNode(SurfacePosition position) => Operations.CreateNode(position);
    public override void OnDeleteNodes() => Operations.DeleteSelectedNodes();
    public override void OnConnectNodes() => Operations.ConnectSelectedNodes();
    public override void OnDisconnectNodes() => Operations.DisconnectSelectedNodes();
}
