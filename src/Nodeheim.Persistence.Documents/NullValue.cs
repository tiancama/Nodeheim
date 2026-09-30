namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents the absence of a value.
/// </summary>
public sealed record NullValue : DocumentValue
{
    private NullValue()
    {
    }

    /// <summary>
    /// Gets the single instance of <see cref="NullValue"/>.
    /// </summary>
    public static NullValue Instance { get; } = new();
}
