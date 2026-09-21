namespace Nodeheim.Editor;

/// <summary>
/// Entered from <see cref="Idle"/> when a grab lands on an already-selected target.
/// Disambiguates a click from a drag: a pointer move begins a <see cref="Dragging"/>, while
/// a release without movement collapses the selection onto the grabbed target and returns
/// to <see cref="Idle"/>.
/// </summary>
public sealed class Pending : InteractionState
{
    private readonly HitTarget _target;
    private readonly SurfacePosition _anchor;

    public Pending(IEditorOperations operations, InteractionController controller, HitTarget target,
        SurfacePosition anchor)
        : base(operations, controller)
    {
        _target = target;
        _anchor = anchor;
    }

    public override void OnPointerMoved(SurfacePosition position) =>
        Controller.TransitionTo(new Dragging(Operations, Controller, _anchor));

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
