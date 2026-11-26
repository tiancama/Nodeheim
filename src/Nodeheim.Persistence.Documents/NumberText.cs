using System.Globalization;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents the text form of numbers in a document.
/// </summary>
/// <remarks>
/// Format contract: Numbers are written with the invariant culture in the shortest form that reads back as the
/// same value; negative zero is written as <c>0</c>. When read, any text that
/// <see cref="double.TryParse(string?, NumberStyles, IFormatProvider?, out double)"/> accepts with
/// <see cref="NumberStyles.Float"/> and the invariant culture is valid.
/// </remarks>
public static class NumberText
{
    /// <summary>
    /// Formats a number as it is written to a document.
    /// </summary>
    /// <param name="value">The number.</param>
    /// <returns>The number in the shortest form that reads back as the same value.</returns>
    /// <remarks>
    /// Format contract: Negative zero is written as <c>0</c>.
    /// </remarks>
    public static string Format(double value) =>
        value == 0 ? "0" : value.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Tries to parse a number from its text in a document.
    /// </summary>
    /// <param name="text">The text to parse.</param>
    /// <param name="value">The number if <paramref name="text"/> is valid; otherwise, zero.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="text"/> is a valid number; otherwise, <see langword="false"/>.
    /// </returns>
    public static bool TryParse(string? text, out double value) =>
        double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
}
