namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a finite floating-point value.
/// </summary>
/// <remarks>Format contract: finite values only; negative zero is stored as zero.</remarks>
public sealed record RealValue : DocumentValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RealValue"/> record.
    /// </summary>
    /// <param name="value">The floating-point value. Negative zero is stored as zero.</param>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="value"/> is NaN or infinite.
    /// </exception>
    public RealValue(double value)
    {
        if (!double.IsFinite(value))
            throw new ArgumentOutOfRangeException(nameof(value), value, "The value must be finite.");

        Value = value == 0 ? 0 : value;
    }

    /// <summary>
    /// Gets the floating-point value.
    /// </summary>
    public double Value { get; }
}
