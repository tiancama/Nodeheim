using Avalonia.Data.Converters;

namespace Nodeheim.Editor.Desktop;

public static class NodeConverters
{
    public static readonly SubtractConverter Subtract = new();

    public static readonly FuncValueConverter<double, double> Diameter = new(r => r * 2);

    public static readonly PointConverter Point = new();
}
