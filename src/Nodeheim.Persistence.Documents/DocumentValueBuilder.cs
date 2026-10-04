using System.Diagnostics;

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
/// <see cref="ArgumentNullException"/> is still thrown. A write call that violates the protocol throws
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
    private readonly Stack<Frame> _frames = new();

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

    /// <inheritdoc />
    public void BeginCompound()
    {
        if (!CanAcceptValue()) return;

        if (_frames.Count == DocumentValue.MaxNestingDepth)
        {
            Fail(DocumentRejectionKind.MaxNestingDepthExceeded,
                $"The maximum nesting depth of {DocumentValue.MaxNestingDepth} is exceeded.");
            return;
        }

        _frames.Push(new CompoundFrame());
    }

    /// <inheritdoc />
    public void Key(string key)
    {
        ArgumentNullException.ThrowIfNull(key);

        if (!CanAccept()) return;

        CompoundFrame compound = PeekFrame<CompoundFrame>("compound");

        if (compound.PendingKey is not null)
            throw new InvalidOperationException("The previous key has no value yet.");

        if (!CompoundValue.IsValidKey(key))
        {
            Fail(DocumentRejectionKind.InvalidCharacter, "A key contains a character that is not permitted.");
            return;
        }

        if (compound.Entries.ContainsKey(key))
        {
            Fail(DocumentRejectionKind.DuplicateKey, "A key occurs more than once in a compound.");
            return;
        }

        compound.PendingKey = key;
    }

    /// <inheritdoc />
    public void EndCompound()
    {
        if (!CanAccept()) return;

        CompoundFrame compound = PeekFrame<CompoundFrame>("compound");

        if (compound.PendingKey is not null)
            throw new InvalidOperationException("The last key has no value.");

        _frames.Pop();
        Add(new CompoundValue(compound.Entries));
    }

    /// <inheritdoc />
    public void BeginList()
    {
        if (!CanAcceptValue()) return;

        if (_frames.Count == DocumentValue.MaxNestingDepth)
        {
            Fail(DocumentRejectionKind.MaxNestingDepthExceeded,
                $"The maximum nesting depth of {DocumentValue.MaxNestingDepth} is exceeded.");
            return;
        }

        _frames.Push(new ListFrame());
    }

    /// <inheritdoc />
    public void EndList()
    {
        if (!CanAccept()) return;

        ListFrame list = PeekFrame<ListFrame>("list");

        _frames.Pop();
        Add(new ListValue(list.Items));
    }

    /// <inheritdoc />
    public void Text(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!CanAcceptValue()) return;

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
        if (!CanAcceptValue()) return;

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
        if (!CanAcceptValue()) return;

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
        if (!CanAcceptValue()) return;

        Add(new BooleanValue(value));
    }

    /// <inheritdoc />
    public void Null()
    {
        if (!CanAcceptValue()) return;

        Add(NullValue.Instance);
    }

    /// <inheritdoc />
    public void Reference(string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        if (!CanAcceptValue()) return;

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

    private bool CanAcceptValue()
    {
        if (!CanAccept())
            return false;

        if (_frames.TryPeek(out Frame? frame) && frame is CompoundFrame { PendingKey: null })
            throw new InvalidOperationException("A value in a compound requires a key.");

        return true;
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

    private void Add(DocumentValue value)
    {
        if (_frames.Count > 0)
        {
            switch (_frames.Peek())
            {
                case ListFrame list:
                    list.Items.Add(value);
                    break;
                case CompoundFrame { PendingKey: { } key } compound:
                    compound.Entries.Add(key, value);
                    compound.PendingKey = null;
                    break;
                default:
                    throw new UnreachableException();
            }
        }
        else
        {
            _root = value;
        }
    }

    private void Fail(DocumentRejectionKind kind, string message) => _rejection = new DocumentRejection(kind, message);

    private T PeekFrame<T>(string containerName) where T : Frame
    {
        if (!_frames.TryPeek(out Frame? frame))
            throw new InvalidOperationException($"No {containerName} is open.");

        if (frame is not T typedFrame)
            throw new InvalidOperationException($"The innermost open container is not a {containerName}.");

        return typedFrame;
    }

    /// <summary>
    /// Represents an open container whose values are still being reported.
    /// </summary>
    private abstract class Frame
    {
        private protected Frame()
        {
        }
    }

    private sealed class ListFrame : Frame
    {
        public List<DocumentValue> Items { get; } = new();
    }

    private sealed class CompoundFrame : Frame
    {
        public Dictionary<string, DocumentValue> Entries { get; } = new(StringComparer.Ordinal);

        /// <summary>
        /// Gets or sets the key reported for the next value, or <see langword="null"/> if no key is pending.
        /// </summary>
        public string? PendingKey { get; set; }
    }
}
