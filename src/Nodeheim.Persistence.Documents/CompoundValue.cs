using System.Buffers;
using System.Collections.ObjectModel;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a set of named values with unique keys and no meaningful order.
/// </summary>
/// <remarks>
/// Format contract: A key consists of at least one ASCII letter, digit, period, hyphen, or underscore. Keys are
/// compared ordinally, so they are case-sensitive. Equality does not depend on the order of the entries.
/// </remarks>
public sealed record CompoundValue : DocumentValue
{
    private static readonly SearchValues<char> KeyCharacters =
        SearchValues.Create("ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789.-_");

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
            if (!IsValidKey(key))
                throw new ArgumentException(
                    $"The key '{key}' is empty or contains a character that is not permitted.", nameof(entries));

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

    /// <summary>
    /// Determines whether a string is a valid key.
    /// </summary>
    /// <param name="key">The string to check.</param>
    /// <returns>
    /// <see langword="true"/> if <paramref name="key"/> is a valid key; otherwise, <see langword="false"/>.
    /// </returns>
    /// <remarks>
    /// Format contract: The rule for keys is described on <see cref="CompoundValue"/>.
    /// </remarks>
    internal static bool IsValidKey(string? key) =>
        key is { Length: > 0 } && !key.AsSpan().ContainsAnyExcept(KeyCharacters);
}
