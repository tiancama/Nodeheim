namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Defines a receiver for the values of a document, reported in document order.
/// </summary>
/// <remarks>
/// Each method call is a write call. In terms of the Builder pattern, write calls are the steps, and the caller
/// that makes them is the director. A director reports exactly one root value.
/// A list is reported as <see cref="BeginList"/>, followed by its items and the matching <see cref="EndList"/>.
/// A compound is reported as <see cref="BeginCompound"/>, followed by pairs of <see cref="Key"/> and a value,
/// and the matching <see cref="EndCompound"/>. Lists and compounds may be nested.
/// Keeping to this protocol is the director's responsibility. An implementation may throw
/// <see cref="InvalidOperationException"/> when the protocol is violated.
/// How values that violate the rules of the value model are handled is defined by the implementation.
/// </remarks>
public interface IDocumentValueWriter
{
    /// <summary>
    /// Reports the beginning of a compound.
    /// </summary>
    void BeginCompound();

    /// <summary>
    /// Reports the key of the next value in the innermost open compound.
    /// </summary>
    /// <param name="key">The key.</param>
    /// <exception cref="ArgumentNullException"><paramref name="key"/> is <see langword="null"/>.</exception>
    void Key(string key);

    /// <summary>
    /// Reports the end of the innermost open compound.
    /// </summary>
    void EndCompound();

    /// <summary>
    /// Reports the beginning of a list.
    /// </summary>
    void BeginList();

    /// <summary>
    /// Reports the end of the innermost open list.
    /// </summary>
    void EndList();

    /// <summary>
    /// Reports a text value.
    /// </summary>
    /// <param name="value">The text.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    void Text(string value);

    /// <summary>
    /// Reports an integer value.
    /// </summary>
    /// <param name="value">The integer value.</param>
    void Integer(long value);

    /// <summary>
    /// Reports a floating-point value.
    /// </summary>
    /// <param name="value">The floating-point value.</param>
    void Real(double value);

    /// <summary>
    /// Reports a Boolean value.
    /// </summary>
    /// <param name="value">The Boolean value.</param>
    void Boolean(bool value);

    /// <summary>
    /// Reports a null value.
    /// </summary>
    void Null();

    /// <summary>
    /// Reports a reference to an identity within the document.
    /// </summary>
    /// <param name="value">The text that identifies the referenced identity.</param>
    /// <exception cref="ArgumentNullException"><paramref name="value"/> is <see langword="null"/>.</exception>
    void Reference(string value);
}
