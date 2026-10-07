namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents the result of reading a document.
/// </summary>
/// <remarks>
/// The hierarchy is closed: a result is either a <see cref="DocumentSuccess"/> or a <see cref="DocumentFailure"/>.
/// </remarks>
public abstract record DocumentResult
{
    private protected DocumentResult()
    {
    }
}

/// <summary>
/// Represents the successful result of reading a document.
/// </summary>
/// <param name="Document">The document that was read.</param>
public sealed record DocumentSuccess(Document Document) : DocumentResult;

/// <summary>
/// Represents a failure to read a document because its content is rejected.
/// </summary>
/// <param name="Rejection">The rejection that caused the failure.</param>
public sealed record DocumentFailure(DocumentRejection Rejection) : DocumentResult;
