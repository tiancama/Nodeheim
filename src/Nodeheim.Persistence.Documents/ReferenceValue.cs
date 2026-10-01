namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a reference to an identity within the document.
/// </summary>
/// <remarks>
/// Format contract: non-empty text within the XML 1.0 character set.
/// What the identity denotes is defined by the document type. Equality is ordinal text comparison.
/// </remarks>
public sealed record ReferenceValue : DocumentValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ReferenceValue"/> record.
    /// </summary>
    /// <param name="value">The identity the reference points to.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// <paramref name="value"/> is empty or contains a character that XML 1.0 does not permit.
    /// </exception>
    public ReferenceValue(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException("The reference is empty or contains a character that XML 1.0 does not permit.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Gets the identity the reference points to.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Determines whether a text is a valid reference.
    /// </summary>
    /// <param name="value">The text to check.</param>
    /// <returns>
    /// <see langword="true"/> if the text is not empty and every character is permitted; otherwise, <see langword="false"/>.
    /// </returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static bool IsValid(string value)
    {
        ArgumentNullException.ThrowIfNull(value);
        return TextValue.IsValid(value) && value.Length > 0;
    }
}
