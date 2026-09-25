namespace Nodeheim.Editor;

/// <summary>
/// Represents an active drag of the selected nodes.
/// </summary>
/// <remarks>
/// The nodes move together by the pointer's offset from the anchor. Releasing the
/// pointer returns to <see cref="Idle"/>.
/// </remarks>
public sealed class Dragging : InteractionState
{
    private readonly SurfacePosition _anchor;
    private readonly IReadOnlyDictionary<NodeViewModel, SurfacePosition> _origins;

    /// <summary>
    /// Initializes a new instance of the <see cref="Dragging"/> class and captures the
    /// positions of the selected nodes.
    /// </summary>
    /// <param name="operations">The editor operations the state acts on.</param>
    /// <param name="controller">The controller that holds the state and performs transitions.</param>
    /// <param name="anchor">The pointer position at press time; the origin of the drag offset.</param>
    public Dragging(IEditorOperations operations, InteractionController controller, SurfacePosition anchor)
        : base(operations, controller)
    {
        _anchor = anchor;
        _origins = operations.SnapshotSelectionPositions();
    }

    /// <summary>
    /// Moves every captured node by the pointer's offset from the anchor.
    /// </summary>
    /// <param name="position">The current pointer position.</param>
    public override void OnPointerMoved(SurfacePosition position)
    {
        foreach ((NodeViewModel node, SurfacePosition origin) in _origins)
        {
            double x = origin.X + (position.X - _anchor.X);
            double y = origin.Y + (position.Y - _anchor.Y);
            Operations.MoveNode(node, new SurfacePosition(x, y));
        }
    }

    /// <summary>
    /// Ends the drag and returns to <see cref="Idle"/>.
    /// </summary>
    public override void OnPointerReleased() => Controller.TransitionTo(new Idle(Operations, Controller));
}
