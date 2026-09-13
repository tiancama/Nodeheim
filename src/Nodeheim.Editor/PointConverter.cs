using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Nodeheim.Editor;

public class PointConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
        => values is [double x, double y] ? new Point(x, y) : AvaloniaProperty.UnsetValue;
}
