namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a boolean value.
/// </summary>
public sealed record BooleanValue : DocumentValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="BooleanValue"/> record.
    /// </summary>
    /// <param name="value">The boolean value.</param>
    public BooleanValue(bool value)
    {
        Value = value;
    }

    /// <summary>
    /// Gets the boolean value.
    /// </summary>
    public bool Value { get; }
}
