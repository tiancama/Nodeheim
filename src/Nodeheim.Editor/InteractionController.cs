namespace Nodeheim.Editor;

/// <summary>
/// Holds the current interaction state and forwards input intents to it. States request
/// transitions through <see cref="TransitionTo"/>, which runs the exit/enter lifecycle.
/// </summary>
public class InteractionController
{
    private InteractionState _current;

    public InteractionController(IEditorOperations operations)
    {
        _current = new Idle(operations, this);
    }

    public void Exclusive(HitTarget target) => _current.OnExclusive(target);
    public void Toggle(HitTarget target) => _current.OnToggle(target);
    public void Grab(HitTarget target, SurfacePosition position) => _current.OnGrab(target, position);
    public void PointerMoved(SurfacePosition position) => _current.OnPointerMoved(position);
    public void PointerReleased() => _current.OnPointerReleased();
    public void CreateNode(SurfacePosition position) => _current.OnCreateNode(position);
    public void DeleteNodes() => _current.OnDeleteNodes();
    public void ConnectNodes() => _current.OnConnectNodes();
    public void DisconnectNodes() => _current.OnDisconnectNodes();

    /// <summary>
    /// Replaces the current state, calling
    /// <see cref="InteractionState.OnExit"/> on the outgoing state and
    /// <see cref="InteractionState.OnEnter"/> on the incoming one.
    /// </summary>
    /// <param name="next">The next state.</param>
    public void TransitionTo(InteractionState next)
    {
        _current.OnExit();
        _current = next;
        _current.OnEnter();
    }
}
