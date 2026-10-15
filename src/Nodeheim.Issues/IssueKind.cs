namespace Nodeheim.Issues;

/// <summary>
/// Specifies the kind of an issue.
/// </summary>
/// <remarks>
/// New kinds may be added. Code that switches over this enumeration should handle unrecognized values in its
/// default case.
/// </remarks>
public enum IssueKind
{
    /// <summary>
    /// The input is not accepted because of its content, including syntax errors.
    /// </summary>
    Rejection,

    /// <summary>
    /// The input deviates in a way that does not prevent processing; the affected part is not processed.
    /// </summary>
    Discrepancy,

    /// <summary>
    /// The operation fails because of its environment, such as an I/O error.
    /// </summary>
    EnvironmentFailure,
}
