using Nodeheim.Domain;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

internal static class GraphMapper
{
    internal const string SectionType = "core.graph";

    private const string SupportedContentVersion = "0.1";
    private const string NodesKey = "nodes";
    private const string ConnectionsKey = "connections";

    private static readonly IssueDefinition UnsupportedContentVersionIssue =
        new("core.graph.unsupportedContentVersion", IssueKind.Rejection);

    private static readonly IssueDefinition InvalidStructureIssue =
        new("core.graph.invalidStructure", IssueKind.Rejection);

    private static readonly IssueDefinition InvalidItemIssue =
        new("core.graph.invalidItem", IssueKind.Discrepancy);

    /// <summary>
    /// Reads a graph from the <c>core.graph</c> section.
    /// </summary>
    /// <param name="section">The section to read.</param>
    /// <param name="issues">The collector that receives the issues found while reading.</param>
    /// <returns>
    /// The graph, or <see langword="null"/> if the section is rejected; the rejection is added to
    /// <paramref name="issues"/>.
    /// </returns>
    /// <remarks>
    /// Format contract: The content is a compound with exactly the entries <c>nodes</c> and <c>connections</c>,
    /// both lists. Each item of <c>nodes</c> is a text with a GUID in any format that
    /// <see cref="Guid.TryParse(string, out Guid)"/> accepts. Each item of <c>connections</c> is a list of two
    /// different IDs from <c>nodes</c>; <c>[a, b]</c> and <c>[b, a]</c> are the same connection. An invalid or
    /// duplicate item is skipped and reported as <c>core.graph.invalidItem</c>; any other deviation rejects the
    /// section with <c>core.graph.invalidStructure</c>.
    /// </remarks>
    internal static Graph? Read(DocumentSection section, IIssueCollector issues)
    {
        if (section.ContentVersion != SupportedContentVersion)
        {
            issues.Add(new Issue(UnsupportedContentVersionIssue,
                $"Unsupported content version '{section.ContentVersion}'.", null));
            return null;
        }

        if (section.Content is not CompoundValue content)
            return Reject(issues, "Invalid structure: the content is not a compound.");

        IReadOnlyDictionary<string, DocumentValue> entries = content.Entries;

        if (!entries.TryGetValue(NodesKey, out DocumentValue? nodesValue))
            return Reject(issues, "Invalid structure: the key 'nodes' is missing.");

        if (!entries.TryGetValue(ConnectionsKey, out DocumentValue? connectionsValue))
            return Reject(issues, "Invalid structure: the key 'connections' is missing.");

        foreach (string key in entries.Keys)
        {
            if (key is not (NodesKey or ConnectionsKey))
                return Reject(issues, $"Invalid structure: unexpected key '{key}'.");
        }

        if (nodesValue is not ListValue nodes)
            return Reject(issues, "Invalid structure: the value of 'nodes' is not a list.");

        if (connectionsValue is not ListValue connections)
            return Reject(issues, "Invalid structure: the value of 'connections' is not a list.");

        Graph graph = new();
        Dictionary<Guid, Node> nodesById = new();

        for (int i = 0; i < nodes.Items.Count; i++)
        {
            if (!TryReadId(nodes.Items[i], out Guid id))
            {
                issues.Add(new Issue(InvalidItemIssue,
                    $"Invalid item: 'nodes' item {i} is not a GUID.", null));
                continue;
            }

            Node node = new() { Id = id };
            if (!nodesById.TryAdd(id, node))
            {
                issues.Add(new Issue(InvalidItemIssue,
                    $"Invalid item: 'nodes' item {i} duplicates an earlier item.", null));
                continue;
            }

            graph.AddNode(node);
        }

        for (int i = 0; i < connections.Items.Count; i++)
        {
            if (connections.Items[i] is not ListValue pair)
            {
                issues.Add(new Issue(InvalidItemIssue,
                    $"Invalid item: 'connections' item {i} is not a list.", null));
                continue;
            }

            if (pair.Items.Count != 2)
            {
                issues.Add(new Issue(InvalidItemIssue,
                    $"Invalid item: 'connections' item {i} does not have exactly two items.", null));
                continue;
            }

            if (!TryReadId(pair.Items[0], out Guid idA) || !TryReadId(pair.Items[1], out Guid idB))
            {
                issues.Add(new Issue(InvalidItemIssue,
                    $"Invalid item: 'connections' item {i} has an invalid ID.", null));
                continue;
            }

            if (!nodesById.TryGetValue(idA, out Node? nodeA) || !nodesById.TryGetValue(idB, out Node? nodeB))
            {
                issues.Add(new Issue(InvalidItemIssue,
                    $"Invalid item: 'connections' item {i} refers to a node that is not in 'nodes'.", null));
                continue;
            }

            if (nodeA.Id == nodeB.Id)
            {
                issues.Add(new Issue(InvalidItemIssue,
                    $"Invalid item: 'connections' item {i} connects a node to itself.", null));
                continue;
            }

            if (!graph.Connect(nodeA, nodeB))
            {
                issues.Add(new Issue(InvalidItemIssue,
                    $"Invalid item: 'connections' item {i} duplicates an earlier item.", null));
            }
        }

        return graph;
    }

    private static bool TryReadId(DocumentValue item, out Guid id)
    {
        if (item is TextValue text && Guid.TryParse(text.Value, out id))
            return true;

        id = Guid.Empty;
        return false;
    }

    private static Graph? Reject(IIssueCollector issues, string message)
    {
        issues.Add(new Issue(InvalidStructureIssue, message, null));
        return null;
    }
}
