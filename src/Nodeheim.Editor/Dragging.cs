namespace Nodeheim.Editor;

/// <summary>
/// Active drag. Captures the positions of the selected nodes at construction, then moves them
/// together by the pointer's offset from the press anchor, returning to <see cref="Idle"/> on
/// release.
/// </summary>
public sealed class Dragging : InteractionState
{
    private readonly SurfacePosition _anchor;
    private readonly IReadOnlyDictionary<NodeViewModel, SurfacePosition> _origins;

    /// <param name="anchor">The pointer position at press time; the origin of the drag offset.</param>
    /// <remarks>
    /// Node origins are captured here, so they reflect the positions held before the first move.
    /// </remarks>
    public Dragging(IEditorOperations operations, InteractionController controller, SurfacePosition anchor) : base(
        operations, controller)
    {
        _anchor = anchor;
        _origins = operations.SnapshotSelectionPositions();
    }

    public override void OnPointerMoved(SurfacePosition position)
    {
        foreach ((NodeViewModel node, SurfacePosition origin) in _origins)
        {
            double x = origin.X + (position.X - _anchor.X);
            double y = origin.Y + (position.Y - _anchor.Y);
            Operations.MoveNode(node, new SurfacePosition(x, y));
        }
    }

    public override void OnPointerReleased() => Controller.TransitionTo(new Idle(Operations, Controller));
}
