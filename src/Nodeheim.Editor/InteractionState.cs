namespace Nodeheim.Editor;

/// <summary>
/// Base class for interaction states. A state responds to the input intents it cares about
/// and ignores the rest: every handler is a virtual no-op, so a concrete state overrides
/// only the intents relevant to it. States drive their own transitions through the controller.
/// </summary>
public abstract class InteractionState
{
    protected IEditorOperations Operations { get; }
    protected InteractionController Controller { get; }

    protected InteractionState(IEditorOperations operations, InteractionController controller)
    {
        Operations = operations;
        Controller = controller;
    }

    /// <summary>Called when this state becomes current, for entry effects.</summary>
    public virtual void OnEnter() { }

    /// <summary>Called when this state stops being current, for cleanup.</summary>
    public virtual void OnExit() { }

    /// <summary>
    /// An exclusive selection gesture on the given target.
    /// </summary>
    /// <param name="target">The target that was hit.</param>
    public virtual void OnExclusive(HitTarget target) { }

    /// <summary>
    /// A toggling selection gesture on the given target.
    /// </summary>
    /// <param name="target">The target that was hit.</param>
    public virtual void OnToggle(HitTarget target) { }

    /// <summary>
    /// A press on the given target that may begin a drag.
    /// </summary>
    /// <param name="target">The target that was hit.</param>
    /// <param name="position">The press location used as the drag anchor.</param>
    public virtual void OnGrab(HitTarget target, SurfacePosition position) { }

    /// <summary>
    /// The pointer moved.
    /// </summary>
    /// <param name="position">The current position of the pointer.</param>
    public virtual void OnPointerMoved(SurfacePosition position) { }

    /// <summary>The pointer was released.</summary>
    public virtual void OnPointerReleased() { }

    /// <summary>
    /// A request to create a node.
    /// </summary>
    /// <param name="position">The position for the new node.</param>
    public virtual void OnCreateNode(SurfacePosition position) { }

    /// <summary>A command to delete the current selection.</summary>
    public virtual void OnDeleteNodes() { }

    /// <summary>A command to connect the current selection.</summary>
    public virtual void OnConnectNodes() { }

    /// <summary>A command to disconnect the current selection.</summary>
    public virtual void OnDisconnectNodes() { }
}
