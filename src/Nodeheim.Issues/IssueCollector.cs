namespace Nodeheim.Issues;

/// <summary>
/// Represents a collector that keeps the added issues in the order in which they were added.
/// </summary>
public sealed class IssueCollector : IIssueCollector
{
    private readonly List<Issue> _issues = new();

    /// <summary>
    /// Gets the added issues in the order in which they were added.
    /// </summary>
    public IReadOnlyList<Issue> Issues => _issues;

    /// <inheritdoc />
    public void Add(Issue issue) => _issues.Add(issue);
}
