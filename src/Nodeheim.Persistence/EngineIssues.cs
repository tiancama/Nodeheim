using Nodeheim.Issues;

namespace Nodeheim.Persistence;

/// <summary>
/// Provides the definitions of the issues that concern the sections of the owner <c>engine</c>.
/// </summary>
internal static class EngineIssues
{
    /// <summary>
    /// Represents the rejection that the section <c>engine.graph</c> has an unsupported content version.
    /// </summary>
    internal static readonly IssueDefinition UnsupportedContentVersion =
        new("engine.graph.unsupportedContentVersion", IssueKind.Rejection);

    /// <summary>
    /// Represents the rejection that the content of <c>engine.graph</c> does not have the expected structure.
    /// </summary>
    internal static readonly IssueDefinition InvalidStructure =
        new("engine.graph.invalidStructure", IssueKind.Rejection);

    /// <summary>
    /// Represents the discrepancy that an item in <c>engine.graph</c> is invalid or duplicate and is skipped.
    /// </summary>
    internal static readonly IssueDefinition InvalidItem =
        new("engine.graph.invalidItem", IssueKind.Discrepancy);
}
