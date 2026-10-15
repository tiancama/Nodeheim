namespace Nodeheim.Issues;

/// <summary>
/// Defines a receiver for the issues that a stage reports.
/// </summary>
public interface IIssueCollector
{
    void Add(Issue issue);
}
