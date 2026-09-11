using System.ComponentModel;
using System.Runtime.CompilerServices;
using Nodeheim.Domain;

namespace Nodeheim.Editor;

public class NodeViewModel : INotifyPropertyChanged
{
    private readonly Node _node;
    private const double DefaultRadius = 13;

    public NodeViewModel(Node node)
    {
        _node = node;
        Radius = DefaultRadius;
    }

    private double _x;

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

    public Guid Id => _node.Id;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
