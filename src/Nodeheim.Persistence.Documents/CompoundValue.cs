using System.Collections.ObjectModel;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a set of named values with unique keys and no meaningful order.
/// </summary>
/// <remarks>
/// Format contract: keys use the character set of text values, excluding tab, line feed and carriage return.
/// Keys are compared ordinally. Equality does not depend on the order of the entries.
/// </remarks>
public sealed record CompoundValue : DocumentValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CompoundValue"/> record.
    /// </summary>
    /// <param name="entries">The named values. The sequence is copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="entries"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException">
    /// A key is <see langword="null"/>, invalid or duplicated, or a value is <see langword="null"/>.
    /// </exception>
    public CompoundValue(IEnumerable<KeyValuePair<string, DocumentValue>> entries)
    {
        ArgumentNullException.ThrowIfNull(entries);

        KeyValuePair<string, DocumentValue>[] copy = entries.ToArray();
        var dictionary = new Dictionary<string, DocumentValue>(copy.Length, StringComparer.Ordinal);
        foreach ((string key, DocumentValue value) in copy)
        {
            if (key is null || !TextValue.IsValid(key) || key.AsSpan().ContainsAny('\t', '\n', '\r'))
                throw new ArgumentException($"The key '{key}' is null or contains a character that is not permitted.", nameof(entries));

            if (value is null)
                throw new ArgumentException($"The value for key '{key}' is null; use NullValue instead.", nameof(entries));

            if (!dictionary.TryAdd(key, value))
                throw new ArgumentException($"The key '{key}' occurs more than once.", nameof(entries));
        }

        Entries = new ReadOnlyDictionary<string, DocumentValue>(dictionary);
    }

    /// <summary>
    /// Gets the named values.
    /// </summary>
    public IReadOnlyDictionary<string, DocumentValue> Entries { get; }

    /// <inheritdoc />
    public bool Equals(CompoundValue? other)
    {
        if (other is null || Entries.Count != other.Entries.Count)
            return false;

        foreach ((string key, DocumentValue value) in Entries)
        {
            if (!other.Entries.TryGetValue(key, out DocumentValue? otherValue) || !value.Equals(otherValue))
                return false;
        }

        return true;
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        // Summing makes the hash independent of the entry order; overflow is intended.
        int hash = 0;
        foreach ((string key, DocumentValue value) in Entries)
            hash = unchecked(hash + HashCode.Combine(key, value));

        return hash;
    }
}
