namespace Nodeheim.Editor;

/// <summary>
/// Represents the state after a grab on an already selected target, before it is
/// clear whether the gesture is a click or a drag.
/// </summary>
/// <remarks>
/// A pointer movement begins a <see cref="Dragging"/>. A release without movement
/// collapses the selection onto the grabbed target and returns to <see cref="Idle"/>.
/// </remarks>
public sealed class Pending : InteractionState
{
    private readonly HitTarget _target;
    private readonly SurfacePosition _anchor;

    /// <summary>
    /// Initializes a new instance of the <see cref="Pending"/> class.
    /// </summary>
    /// <param name="operations">The editor operations the state acts on.</param>
    /// <param name="controller">The controller that holds the state and performs transitions.</param>
    /// <param name="target">The target that was grabbed.</param>
    /// <param name="anchor">The pointer position at the time of the grab.</param>
    public Pending(IEditorOperations operations, InteractionController controller, HitTarget target,
        SurfacePosition anchor)
        : base(operations, controller)
    {
        _target = target;
        _anchor = anchor;
    }

    /// <summary>
    /// Begins a drag from the grab position.
    /// </summary>
    /// <param name="position">The current pointer position.</param>
    public override void OnPointerMoved(SurfacePosition position) =>
        Controller.TransitionTo(new Dragging(Operations, Controller, _anchor));

    /// <summary>
    /// Treats the grab as a click: selects only the grabbed target and returns to <see cref="Idle"/>.
    /// </summary>
    public override void OnPointerReleased()
    {
        switch (_target)
        {
            case NodeHit(var node):
                Operations.SelectOnly(node);
                break;
            case ConnectionHit(var connection):
                Operations.SelectOnly(connection.NodeA, connection.NodeB);
                break;
        }

        Controller.TransitionTo(new Idle(Operations, Controller));
    }
}
