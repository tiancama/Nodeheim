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
    /// <summary>
    /// Represents the maximum nesting depth of a value.
    /// </summary>
    /// <remarks>
    /// Format contract: A document is a single root value, so the limit covers all of its levels, including those
    /// of the envelope and the manifest. List and compound levels count together. A scalar has depth 0, an empty
    /// container depth 1.
    /// </remarks>
    public const int MaxNestingDepth = 32;

    // Closes the hierarchy. Gap via the protected copy constructor (CS8878): accepted.
    private protected DocumentValue()
    {
    }
}
