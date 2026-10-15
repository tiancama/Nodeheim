namespace Nodeheim.Issues;

/// <summary>
/// Represents the definition of an issue, which fixes its code and its kind.
/// </summary>
/// <param name="Code">The code that identifies the issue, in the form <c>owner.name</c>.</param>
/// <param name="Kind">The kind of the issue.</param>
public sealed record IssueDefinition(string Code, IssueKind Kind);
