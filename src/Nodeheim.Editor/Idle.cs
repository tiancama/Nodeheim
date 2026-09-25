namespace Nodeheim.Editor;

/// <summary>
/// Represents the resting state of the interaction.
/// </summary>
/// <remarks>
/// Handles selection gestures and commands directly, and turns a grab into either a
/// pending click or an immediate drag.
/// </remarks>
public sealed class Idle : InteractionState
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Idle"/> class.
    /// </summary>
    /// <param name="operations">The editor operations the state acts on.</param>
    /// <param name="controller">The controller that holds the state and performs transitions.</param>
    public Idle(IEditorOperations operations, InteractionController controller)
        : base(operations, controller)
    { }

    /// <summary>
    /// Clears the selection if empty space was hit; other targets are ignored.
    /// </summary>
    /// <param name="target">The target that was hit.</param>
    public override void OnExclusive(HitTarget target)
    {
        switch (target)
        {
            case EmptyHit:
                Operations.DeselectAll();
                break;
        }
    }

    /// <summary>
    /// Toggles the selection of the hit node, or of both endpoints of the hit connection.
    /// </summary>
    /// <param name="target">The target that was hit.</param>
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
    /// Grabs the target. An already selected target enters <see cref="Pending"/> to await a click
    /// or drag; an unselected target becomes the sole selection and enters <see cref="Dragging"/>
    /// at once.
    /// </summary>
    /// <param name="target">The target that was hit.</param>
    /// <param name="position">The press position, used as the drag anchor.</param>
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

    /// <inheritdoc />
    public override void OnCreateNode(SurfacePosition position) => Operations.CreateNode(position);

    /// <inheritdoc />
    public override void OnDeleteNodes() => Operations.DeleteSelectedNodes();

    /// <inheritdoc />
    public override void OnConnectNodes() => Operations.ConnectSelectedNodes();

    /// <inheritdoc />
    public override void OnDisconnectNodes() => Operations.DisconnectSelectedNodes();
}
