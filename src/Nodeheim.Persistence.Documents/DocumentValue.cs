namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a value in the format-neutral value model of a document.
/// </summary>
/// <remarks>
/// Format contract: the set of value kinds must never change,
/// or older programs cannot read documents at all (planning §9).
/// Every switch over value kinds must throw in its default case.
/// </remarks>
public abstract record DocumentValue
{
    // Closes the hierarchy. Gap via the protected copy constructor (CS8878): accepted.
    private protected DocumentValue()
    {
    }
}
