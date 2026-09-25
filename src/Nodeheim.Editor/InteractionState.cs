namespace Nodeheim.Editor;

/// <summary>
/// Represents a state of the pointer interaction on the editor surface.
/// </summary>
/// <remarks>
/// Every handler does nothing by default, so a concrete state overrides only the
/// intents relevant to it. States perform their own transitions through the controller.
/// </remarks>
public abstract class InteractionState
{
    /// <summary>
    /// Gets the editor operations this state acts on.
    /// </summary>
    protected IEditorOperations Operations { get; }

    /// <summary>
    /// Gets the controller that holds this state.
    /// </summary>
    protected InteractionController Controller { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="InteractionState"/> class.
    /// </summary>
    /// <param name="operations">The editor operations the state acts on.</param>
    /// <param name="controller">The controller that holds the state and performs transitions.</param>
    protected InteractionState(IEditorOperations operations, InteractionController controller)
    {
        Operations = operations;
        Controller = controller;
    }

    /// <summary>
    /// Performs entry effects when this state becomes current.
    /// </summary>
    public virtual void OnEnter() { }

    /// <summary>
    /// Performs cleanup when this state stops being current.
    /// </summary>
    public virtual void OnExit() { }

    /// <summary>
    /// Handles an exclusive selection gesture on a target.
    /// </summary>
    /// <param name="target">The target that was hit.</param>
    public virtual void OnExclusive(HitTarget target) { }

    /// <summary>
    /// Handles a toggling selection gesture on a target.
    /// </summary>
    /// <param name="target">The target that was hit.</param>
    public virtual void OnToggle(HitTarget target) { }

    /// <summary>
    /// Handles a press on a target that may begin a drag.
    /// </summary>
    /// <param name="target">The target that was hit.</param>
    /// <param name="position">The press position, used as the drag anchor.</param>
    public virtual void OnGrab(HitTarget target, SurfacePosition position) { }

    /// <summary>
    /// Handles a movement of the pointer.
    /// </summary>
    /// <param name="position">The current pointer position.</param>
    public virtual void OnPointerMoved(SurfacePosition position) { }

    /// <summary>
    /// Handles the release of the pointer.
    /// </summary>
    public virtual void OnPointerReleased() { }

    /// <summary>
    /// Handles a request to create a node.
    /// </summary>
    /// <param name="position">The center position of the new node.</param>
    public virtual void OnCreateNode(SurfacePosition position) { }

    /// <summary>
    /// Handles a command to delete the selected nodes.
    /// </summary>
    public virtual void OnDeleteNodes() { }

    /// <summary>
    /// Handles a command to connect the selected nodes.
    /// </summary>
    public virtual void OnConnectNodes() { }

    /// <summary>
    /// Handles a command to disconnect the selected nodes.
    /// </summary>
    public virtual void OnDisconnectNodes() { }
}
