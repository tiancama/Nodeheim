using System.ComponentModel;
using System.Runtime.CompilerServices;
using Nodeheim.Domain;

namespace Nodeheim.Editor;

/// <summary>
/// Represents a node on the editor surface, wrapping a domain <see cref="Node"/>.
/// </summary>
public class NodeViewModel : INotifyPropertyChanged
{
    private const double DefaultRadius = 13;

    /// <summary>
    /// Initializes a new instance of the <see cref="NodeViewModel"/> class.
    /// </summary>
    /// <param name="model">The domain node to wrap.</param>
    public NodeViewModel(Node model)
    {
        Model = model;
        Radius = DefaultRadius;
    }

    internal Node Model { get; }

    /// <summary>
    /// Gets the identifier of the wrapped node.
    /// </summary>
    public Guid Id => Model.Id;

    /// <summary>
    /// Gets or sets the horizontal coordinate of the node's center.
    /// </summary>
    public double X { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets the vertical coordinate of the node's center.
    /// </summary>
    public double Y { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets the radius of the node.
    /// </summary>
    public double Radius { get; set => SetProperty(ref field, value); }

    /// <summary>
    /// Gets or sets a value indicating whether the node is selected.
    /// </summary>
    /// <remarks>
    /// Mirrors the selection held by <see cref="EditorViewModel"/>, which keeps this flag in sync.
    /// </remarks>
    public bool IsSelected { get; set => SetProperty(ref field, value); }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    private void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return;
        storage = value;
        OnPropertyChanged(name);
    }
}
