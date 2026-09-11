using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;

namespace Nodeheim.Editor;

public class SubtractConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture) =>
        values is [double a, double b] ? a - b : AvaloniaProperty.UnsetValue;
}
