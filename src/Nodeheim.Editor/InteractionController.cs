namespace Nodeheim.Editor;

/// <summary>
/// Represents the context of the interaction state machine: holds the current state
/// and forwards input intents to it.
/// </summary>
/// <remarks>
/// States request transitions through <see cref="TransitionTo"/>, which runs the
/// exit and enter lifecycle.
/// </remarks>
public class InteractionController
{
    private InteractionState _current;

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionController"/> class in the
    /// <see cref="Idle"/> state.
    /// </summary>
    /// <param name="operations">The editor operations the states act on.</param>
    public InteractionController(IEditorOperations operations)
    {
        _current = new Idle(operations, this);
    }

    /// <summary>
    /// Forwards an exclusive selection gesture on a target to the current state.
    /// </summary>
    /// <param name="target">The target that was hit.</param>
    public void Exclusive(HitTarget target) => _current.OnExclusive(target);

    /// <summary>
    /// Forwards a toggling selection gesture on a target to the current state.
    /// </summary>
    /// <param name="target">The target that was hit.</param>
    public void Toggle(HitTarget target) => _current.OnToggle(target);

    /// <summary>
    /// Forwards a grab on a target to the current state.
    /// </summary>
    /// <param name="target">The target that was hit.</param>
    /// <param name="position">The press position.</param>
    public void Grab(HitTarget target, SurfacePosition position) => _current.OnGrab(target, position);

    /// <summary>
    /// Forwards a pointer movement to the current state.
    /// </summary>
    /// <param name="position">The current pointer position.</param>
    public void PointerMoved(SurfacePosition position) => _current.OnPointerMoved(position);

    /// <summary>
    /// Forwards a pointer release to the current state.
    /// </summary>
    public void PointerReleased() => _current.OnPointerReleased();

    /// <summary>
    /// Forwards a request to create a node to the current state.
    /// </summary>
    /// <param name="position">The center position of the new node.</param>
    public void CreateNode(SurfacePosition position) => _current.OnCreateNode(position);

    /// <summary>
    /// Forwards a command to delete the selected nodes to the current state.
    /// </summary>
    public void DeleteNodes() => _current.OnDeleteNodes();

    /// <summary>
    /// Forwards a command to connect the selected nodes to the current state.
    /// </summary>
    public void ConnectNodes() => _current.OnConnectNodes();

    /// <summary>
    /// Forwards a command to disconnect the selected nodes to the current state.
    /// </summary>
    public void DisconnectNodes() => _current.OnDisconnectNodes();

    /// <summary>
    /// Replaces the current state, calling
    /// <see cref="InteractionState.OnExit"/> on the outgoing state and
    /// <see cref="InteractionState.OnEnter"/> on the incoming one.
    /// </summary>
    /// <param name="next">The state to transition to.</param>
    public void TransitionTo(InteractionState next)
    {
        _current.OnExit();
        _current = next;
        _current.OnEnter();
    }
}
