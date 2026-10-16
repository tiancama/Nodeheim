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

    /// <summary>
    /// Reads a graph from the <c>core.graph</c> section.
    /// </summary>
    /// <param name="section">The section to read.</param>
    /// <param name="issues">The collector that receives the issues found while reading.</param>
    /// <returns>
    /// The graph, or <see langword="null"/> if the section is rejected; the rejection is added to
    /// <paramref name="issues"/>.
    /// </returns>
    internal static Graph? Read(DocumentSection section, IIssueCollector issues)
    {
        if (section.ContentVersion != SupportedContentVersion)
        {
            issues.Add(new Issue(UnsupportedContentVersionIssue,
                $"Unsupported content version '{section.ContentVersion}'.", null));
            return null;
        }

        IReadOnlyDictionary<string, DocumentValue> entries = ((CompoundValue)section.Content).Entries;
        Graph graph = new();
        Dictionary<Guid, Node> nodesById = new();

        foreach (DocumentValue item in ((ListValue)entries[NodesKey]).Items)
        {
            Node node = new() { Id = ReadId(item) };
            graph.AddNode(node);
            nodesById.Add(node.Id, node);
        }

        foreach (DocumentValue item in ((ListValue)entries[ConnectionsKey]).Items)
        {
            IReadOnlyList<DocumentValue> pair = ((ListValue)item).Items;
            graph.Connect(nodesById[ReadId(pair[0])], nodesById[ReadId(pair[1])]);
        }

        return graph;
    }

    private static Guid ReadId(DocumentValue item) => Guid.Parse(((TextValue)item).Value);
}
