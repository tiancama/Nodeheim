namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents an ordered list of values.
/// </summary>
public sealed record ListValue : DocumentValue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ListValue"/> record.
    /// </summary>
    /// <param name="items">The values in their order. The sequence is copied.</param>
    /// <exception cref="ArgumentNullException"><paramref name="items"/> is <see langword="null"/>.</exception>
    /// <exception cref="ArgumentException"><paramref name="items"/> contains <see langword="null"/>.</exception>
    public ListValue(IEnumerable<DocumentValue> items)
    {
        ArgumentNullException.ThrowIfNull(items);

        DocumentValue[] copy = items.ToArray();
        if (copy.Contains(null))
            throw new ArgumentException("The list must not contain null; use NullValue instead.", nameof(items));

        Items = Array.AsReadOnly(copy);
    }

    /// <summary>
    /// Gets the values in their order.
    /// </summary>
    public IReadOnlyList<DocumentValue> Items { get; }

    /// <inheritdoc />
    public bool Equals(ListValue? other) =>
        other is not null && Items.SequenceEqual(other.Items);

    /// <inheritdoc />
    public override int GetHashCode()
    {
        var hash = new HashCode();
        foreach (DocumentValue item in Items)
            hash.Add(item);

        return hash.ToHashCode();
    }
}
