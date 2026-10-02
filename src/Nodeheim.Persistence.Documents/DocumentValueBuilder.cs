namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a writer that builds a <see cref="DocumentValue"/> from write calls.
/// </summary>
/// <remarks>
/// <para>
/// A builder is used once. After <see cref="GetResult"/> has been called, every further call throws
/// <see cref="InvalidOperationException"/>.
/// </para>
/// <para>
/// The first rejection is recorded, and all later write calls are ignored without checking the protocol;
/// <see cref="ArgumentNullException"/> is still thrown. A write call after a complete root value throws
/// <see cref="InvalidOperationException"/>.
/// </para>
/// <para>
/// Format contract: An integer outside ±(2⁵³ − 1) is rejected, not converted to a real.
/// </para>
/// </remarks>
public sealed class DocumentValueBuilder : IDocumentValueWriter
{
    private DocumentValue? _root;
    private DocumentRejection? _rejection;
    private bool _isConsumed;

    /// <summary>
    /// Gets a value indicating whether a rejection has been recorded.
    /// </summary>
    /// <value>
    /// <see langword="true"/> if a rejection has been recorded; otherwise, <see langword="false"/>.
    /// </value>
    /// <remarks>
    /// A director can use this value to stop reporting early.
    /// </remarks>
    public bool HasFailed => _rejection is not null;

    public void BeginList() => throw new NotImplementedException();

    public void EndList() => throw new NotImplementedException();

    /// <inheritdoc />
    public void Text(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!CanAccept()) return;

        if (!TextValue.IsValid(value))
        {
            Fail(DocumentRejectionKind.InvalidCharacter,
                "A text contains a character that XML 1.0 does not permit.");
            return;
        }

        Add(new TextValue(value));
    }

    /// <inheritdoc />
    public void Integer(long value)
    {
        if (!CanAccept()) return;

        if (value is < IntegerValue.MinValue or > IntegerValue.MaxValue)
        {
            Fail(DocumentRejectionKind.NumberNotRepresentable, "An integer is outside the range ±(2^53 − 1).");
            return;
        }

        Add(new IntegerValue(value));
    }

    /// <inheritdoc />
    public void Real(double value)
    {
        if (!CanAccept()) return;

        if (!double.IsFinite(value))
        {
            Fail(DocumentRejectionKind.NumberNotRepresentable, "A real is not finite.");
            return;
        }

        Add(new RealValue(value));
    }

    /// <inheritdoc />
    public void Boolean(bool value)
    {
        if (!CanAccept()) return;

        Add(new BooleanValue(value));
    }

    /// <inheritdoc />
    public void Null()
    {
        if (!CanAccept()) return;

        Add(NullValue.Instance);
    }

    /// <inheritdoc />
    public void Reference(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!CanAccept()) return;

        if (value.Length == 0)
        {
            Fail(DocumentRejectionKind.EmptyReference, "A reference is empty.");
            return;
        }

        if (!TextValue.IsValid(value))
        {
            Fail(DocumentRejectionKind.InvalidCharacter,
                "A reference contains a character that XML 1.0 does not permit.");
            return;
        }

        Add(new ReferenceValue(value));
    }

    /// <summary>
    /// Returns the result of the build.
    /// </summary>
    /// <returns>
    /// A <see cref="DocumentValueFailure"/> with the first rejection if one has been recorded; otherwise, a
    /// <see cref="DocumentValueSuccess"/> with the root value.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The result has already been retrieved, or no rejection has been recorded and no complete root value
    /// has been reported.
    /// </exception>
    public DocumentValueResult GetResult()
    {
        if (_isConsumed)
            throw new InvalidOperationException("The result has already been retrieved.");

        _isConsumed = true;

        if (_rejection is not null)
            return new DocumentValueFailure(_rejection);

        if (_root is null)
            throw new InvalidOperationException("No complete root value has been reported.");

        return new DocumentValueSuccess(_root);
    }

    private bool CanAccept()
    {
        if (_isConsumed)
            throw new InvalidOperationException("The result has already been retrieved.");

        if (HasFailed) return false;

        if (_root is not null)
            throw new InvalidOperationException("The root value is already complete.");

        return true;
    }

    private void Add(DocumentValue value) => _root = value;

    private void Fail(DocumentRejectionKind kind, string message) => _rejection = new DocumentRejection(kind, message);
}
