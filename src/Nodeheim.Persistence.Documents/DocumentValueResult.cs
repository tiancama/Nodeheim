namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents the result of building a document value.
/// </summary>
public abstract record DocumentValueResult
{
    private protected DocumentValueResult()
    {
    }
}

/// <summary>
/// Represents the successful result of building a document value.
/// </summary>
/// <param name="Value">The built value.</param>
public sealed record DocumentValueSuccess(DocumentValue Value) : DocumentValueResult;

/// <summary>
/// Represents a failure to build a document value.
/// </summary>
/// <param name="Rejection">The first rule violation, which caused the failure.</param>
public sealed record DocumentValueFailure(DocumentRejection Rejection) : DocumentValueResult;
