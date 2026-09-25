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

    private double _x;

    /// <summary>
    /// Gets or sets the horizontal coordinate of the node's center.
    /// </summary>
    public double X
    {
        get => _x;
        set
        {
            if (_x.Equals(value)) return;
            _x = value;
            OnPropertyChanged();
        }
    }

    private double _y;

    /// <summary>
    /// Gets or sets the vertical coordinate of the node's center.
    /// </summary>
    public double Y
    {
        get => _y;
        set
        {
            if (_y.Equals(value)) return;
            _y = value;
            OnPropertyChanged();
        }
    }

    private double _radius;

    /// <summary>
    /// Gets or sets the radius of the node.
    /// </summary>
    public double Radius
    {
        get => _radius;
        set
        {
            if (_radius.Equals(value)) return;
            _radius = value;
            OnPropertyChanged();
        }
    }

    private bool _isSelected;

    /// <summary>
    /// Gets or sets a value indicating whether the node is selected.
    /// </summary>
    /// <remarks>
    /// Mirrors the selection held by <see cref="EditorViewModel"/>, which keeps this flag in sync.
    /// </remarks>
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected.Equals(value)) return;
            _isSelected = value;
            OnPropertyChanged();
        }
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
