using System.ComponentModel;
using System.Runtime.CompilerServices;
using Nodeheim.Domain;
using Nodeheim.Editor.Domain;

namespace Nodeheim.Editor;

/// <summary>
/// Represents a node on the editor surface, wrapping a domain <see cref="Node"/> and projecting its entry in the
/// <see cref="Surface"/>.
/// </summary>
public class NodeViewModel : INotifyPropertyChanged
{
    private readonly Surface _surface;
    private readonly ISurfaceOperations _operations;

    /// <summary>
    /// Initializes a new instance of the <see cref="NodeViewModel"/> class.
    /// </summary>
    /// <param name="model">The domain node to wrap.</param>
    /// <param name="surface">The surface that holds the node's entry.</param>
    /// <param name="operations">The operations through which the node's position is changed.</param>
    public NodeViewModel(Node model, Surface surface, ISurfaceOperations operations)
    {
        Model = model;
        _surface = surface;
        _operations = operations;
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the identifier of the wrapped node.
    /// </summary>
    public Guid Id => Model.Id;

    /// <summary>
    /// Gets or sets the horizontal coordinate of the node's center.
    /// </summary>
    /// <remarks>
    /// The setter does not store the value; it moves the node through
    /// <see cref="ISurfaceOperations.MoveNode"/>, which changes the surface and notifies this view model.
    /// </remarks>
    public double X
    {
        get => Entry.Position.X;
        set => _operations.MoveNode(this, new SurfacePosition(value, Y));
    }

    /// <summary>
    /// Gets or sets the vertical coordinate of the node's center.
    /// </summary>
    /// <remarks>
    /// The setter does not store the value; it moves the node through
    /// <see cref="ISurfaceOperations.MoveNode"/>, which changes the surface and notifies this view model.
    /// </remarks>
    public double Y
    {
        get => Entry.Position.Y;
        set => _operations.MoveNode(this, new SurfacePosition(X, value));
    }

    /// <summary>
    /// Gets the radius of the node: its own radius if it has one; otherwise, the surface's default.
    /// </summary>
    public double Radius => Entry.Radius ?? _surface.DefaultNodeRadius;

    /// <summary>
    /// Gets or sets a value indicating whether the node is selected.
    /// </summary>
    /// <remarks>
    /// Mirrors the selection held by <see cref="SurfaceViewModel"/>, which keeps this flag in sync.
    /// </remarks>
    public bool IsSelected { get; set => SetProperty(ref field, value); }

    internal Node Model { get; }

    /// <summary>
    /// Gets the node's entry in the surface.
    /// </summary>
    /// <exception cref="InvalidOperationException">The surface has no entry for the node.</exception>
    internal SurfaceNode Entry =>
        _surface.TryGetNode(Id, out SurfaceNode entry)
            ? entry
            : throw new InvalidOperationException($"The surface has no entry for node '{Id}'.");

    /// <summary>
    /// Raises <see cref="PropertyChanged"/> for the values read from the node's entry in the surface.
    /// </summary>
    /// <remarks>
    /// Called by whoever changed the entry.
    /// </remarks>
    internal void NotifySurfaceNodeChanged()
    {
        OnPropertyChanged(nameof(X));
        OnPropertyChanged(nameof(Y));
        OnPropertyChanged(nameof(Radius));
    }

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return;
        storage = value;
        OnPropertyChanged(name);
    }
}
