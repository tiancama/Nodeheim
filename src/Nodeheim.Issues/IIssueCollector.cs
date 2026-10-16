namespace Nodeheim.Issues;

/// <summary>
/// Defines a receiver for the issues that a stage reports.
/// </summary>
public interface IIssueCollector
{
    /// <summary>
    /// Adds an issue.
    /// </summary>
    /// <param name="issue">The issue to add.</param>
    void Add(Issue issue);
}
