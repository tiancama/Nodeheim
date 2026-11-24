using Nodeheim.Domain;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

internal static class GraphMapper
{
    private const string NodesName = "nodes";
    private const string ConnectionsName = "connections";
    private const string ConnectionName = "connection";
    private const string IdName = "id";

    /// <summary>
    /// Reads a graph from the content of a section.
    /// </summary>
    /// <param name="content">The content to read.</param>
    /// <param name="issues">The collector that receives the issues found while reading.</param>
    /// <returns>
    /// The graph, or <see langword="null"/> if the content is rejected; the rejection is added to
    /// <paramref name="issues"/>.
    /// </returns>
    /// <remarks>
    /// Format contract: The content has exactly one child each named <c>nodes</c> and <c>connections</c>, and
    /// no other children. All children of <c>nodes</c> are named <c>id</c>; all children of <c>connections</c>
    /// are named <c>connection</c>. A leaf whose text is empty or only whitespace counts as no children. Each
    /// <c>id</c> is a leaf with a GUID in any format that <see cref="Guid.TryParse(string, out Guid)"/> accepts.
    /// Each <c>connection</c> has exactly two children named <c>id</c> with different IDs from <c>nodes</c>;
    /// <c>a, b</c> and <c>b, a</c> are the same connection. An invalid or duplicate item is skipped and reported
    /// as <c>engine.graph.invalidItem</c>; any other deviation rejects the content with
    /// <c>engine.graph.invalidStructure</c>.
    /// </remarks>
    internal static Graph? Read(DocumentEntry content, IIssueCollector issues)
    {
        DocumentEntry? nodesEntry = content.GetSingleChild(NodesName);

        if (nodesEntry is null)
            return Reject(issues, "Invalid structure: 'nodes' is missing or occurs more than once.");

        DocumentEntry? connectionsEntry = content.GetSingleChild(ConnectionsName);
        if (connectionsEntry is null)
            return Reject(issues, "Invalid structure: 'connections' is missing or occurs more than once.");

        foreach (DocumentEntry child in content.Children)
        {
            if (child.Name is not (NodesName or ConnectionsName))
                return Reject(issues, $"Invalid structure: unexpected entry '{child.Name}'.");
        }

        IReadOnlyList<DocumentEntry>? ids = GetItems(nodesEntry, IdName);
        if (ids is null)
            return Reject(issues, "Invalid structure: 'nodes' is not a list of 'id' entries.");

        IReadOnlyList<DocumentEntry>? connections = GetItems(connectionsEntry, ConnectionName);
        if (connections is null)
            return Reject(issues, "Invalid structure: 'connections' is not a list of 'connection' entries.");

        Graph graph = new();
        Dictionary<Guid, Node> nodesById = new();

        for (int i = 0; i < ids.Count; i++)
        {
            if (!TryReadId(ids[i], out Guid id))
            {
                issues.Add(new Issue(EngineIssues.InvalidItem,
                    $"Invalid item: 'nodes' item {i} is not a GUID.", null));
                continue;
            }

            Node node = new() { Id = id };
            if (!nodesById.TryAdd(id, node))
            {
                issues.Add(new Issue(EngineIssues.InvalidItem,
                    $"Invalid item: 'nodes' item {i} duplicates an earlier item.", null));
                continue;
            }

            graph.AddNode(node);
        }

        for (int i = 0; i < connections.Count; i++)
        {
            IReadOnlyList<DocumentEntry>? pair = GetItems(connections[i], IdName);
            if (pair is null || pair.Count != 2)
            {
                issues.Add(new Issue(EngineIssues.InvalidItem,
                    $"Invalid item: 'connections' item {i} does not consist of exactly two 'id' entries.", null));
                continue;
            }

            if (!TryReadId(pair[0], out Guid idA) || !TryReadId(pair[1], out Guid idB))
            {
                issues.Add(new Issue(EngineIssues.InvalidItem,
                    $"Invalid item: 'connections' item {i} has an invalid ID.", null));
                continue;
            }

            if (!nodesById.TryGetValue(idA, out Node? nodeA) || !nodesById.TryGetValue(idB, out Node? nodeB))
            {
                issues.Add(new Issue(EngineIssues.InvalidItem,
                    $"Invalid item: 'connections' item {i} refers to a node that is not in 'nodes'.", null));
                continue;
            }

            if (nodeA.Id == nodeB.Id)
            {
                issues.Add(new Issue(EngineIssues.InvalidItem,
                    $"Invalid item: 'connections' item {i} connects a node to itself.", null));
                continue;
            }

            if (!graph.Connect(nodeA, nodeB))
            {
                issues.Add(new Issue(EngineIssues.InvalidItem,
                    $"Invalid item: 'connections' item {i} duplicates an earlier item.", null));
            }
        }

        return graph;
    }

    /// <summary>
    /// Writes the content of the <c>engine.graph</c> section.
    /// </summary>
    /// <param name="graph">The graph to write.</param>
    /// <param name="writer">The writer that receives the content as the value of <c>content</c>.</param>
    /// <remarks>
    /// Format contract: Node IDs are written in ascending order of their text. Each connection is written
    /// once, with the smaller ID first; connections are ordered by their first, then their second ID.
    /// </remarks>
    internal static void Write(Graph graph, IDocumentWriter writer)
    {
        List<string> ids = [];
        List<(string A, string B)> connections = [];

        foreach (Node node in graph.Nodes)
        {
            string id = NodeIdText.Format(node.Id);
            ids.Add(id);

            foreach (Node neighbor in node.Neighbors)
            {
                string neighborId = NodeIdText.Format(neighbor.Id);
                if (NodeIdText.Comparer.Compare(id, neighborId) < 0)
                {
                    connections.Add((id, neighborId));
                }
            }
        }

        ids.Sort(NodeIdText.Comparer);
        connections.Sort(CompareConnections);

        writer.BeginGroup();

        writer.Name(NodesName);
        writer.BeginGroup();
        writer.Name(IdName);
        writer.BeginRepeated();
        foreach (string id in ids)
        {
            writer.Text(id);
        }

        writer.EndRepeated();
        writer.EndGroup();

        writer.Name(ConnectionsName);
        writer.BeginGroup();
        writer.Name(ConnectionName);
        writer.BeginRepeated();
        foreach ((string a, string b) in connections)
        {
            writer.BeginGroup();
            writer.Name(IdName);
            writer.BeginRepeated();
            writer.Text(a);
            writer.Text(b);
            writer.EndRepeated();
            writer.EndGroup();
        }

        writer.EndRepeated();
        writer.EndGroup();

        writer.EndGroup();
    }

    private static int CompareConnections((string A, string B) x, (string A, string B) y)
    {
        int result = NodeIdText.Comparer.Compare(x.A, y.A);
        return result != 0 ? result : NodeIdText.Comparer.Compare(x.B, y.B);
    }

    private static IReadOnlyList<DocumentEntry>? GetItems(DocumentEntry list, string itemName)
    {
        IReadOnlyList<DocumentEntry>? items = list.GetChildrenNamed(itemName);
        if (items is null || items.Count != list.Children.Count)
            return null;

        return items;
    }

    private static bool TryReadId(DocumentEntry entry, out Guid id)
    {
        string? text = entry.GetLeafText();
        if (text is not null && Guid.TryParse(text, out id))
            return true;

        id = Guid.Empty;
        return false;
    }

    private static Graph? Reject(IIssueCollector issues, string message)
    {
        issues.Add(new Issue(EngineIssues.InvalidItem, message, null));
        return null;
    }
}
