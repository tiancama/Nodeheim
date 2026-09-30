namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents an integer value within the range that I-JSON can represent exactly.
/// </summary>
/// <remarks>Format contract: range ±(2^53 − 1) (I-JSON).</remarks>
public sealed record IntegerValue : DocumentValue
{
    /// <summary>
    /// The largest permitted value, 2^53 - 1.
    /// </summary>
    public const long MaxValue = 9_007_199_254_740_991;

    /// <summary>
    /// The smallest permitted value, -(2^53 - 1).
    /// </summary>
    public const long MinValue = -MaxValue;

    /// <summary>
    /// Initializes a new instance of the <see cref="IntegerValue"/> record.
    /// </summary>
    /// <param name="value">The integer value.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> is outside <see cref="MinValue"/> to <see cref="MaxValue"/>.
    /// </exception>
    public IntegerValue(long value)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(value, MinValue);
        ArgumentOutOfRangeException.ThrowIfGreaterThan(value, MaxValue);
        Value = value;
    }

    /// <summary>
    /// Gets the integer value.
    /// </summary>
    public long Value { get; }
}
