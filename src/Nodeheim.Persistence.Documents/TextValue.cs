using System.Xml;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a text value restricted to the characters that XML 1.0 permits.
/// </summary>
/// <remarks>Format contract: characters permitted by XML 1.0 only; no lone surrogates.</remarks>
public sealed record TextValue : DocumentValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TextValue"/> record.
    /// </summary>
    /// <param name="value">The text.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="value"/> contains a character that XML 1.0 does not permit.</exception>
    public TextValue(string value)
    {
        if (!IsValid(value))
            throw new ArgumentException("The text contains a character that XML 1.0 does not permit.", nameof(value));

        Value = value;
    }

    /// <summary>
    /// Gets the text.
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Determines whether a text consists only of characters that XML 1.0 permits.
    /// </summary>
    /// <param name="value">The text to check.</param>
    /// <returns><see langword="true"/> if every character is permitted; otherwise, <see langword="false"/>.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    public static bool IsValid(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        for (int i = 0; i < value.Length; i++)
        {
            if (XmlConvert.IsXmlChar(value[i]))
                continue;

            if (i + 1 < value.Length && XmlConvert.IsXmlSurrogatePair(value[i + 1], value[i]))
            {
                i++;
                continue;
            }

            return false;
        }

        return true;
    }
}
