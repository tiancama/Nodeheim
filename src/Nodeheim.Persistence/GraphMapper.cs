using Nodeheim.Domain;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

internal static class GraphMapper
{
    internal const string SectionType = "core.graph";

    private const string SupportedContentVersion = "0.1";
    private const string NodesKey = "nodes";
    private const string ConnectionsKey = "connections";

    internal static Graph Read(DocumentSection section)
    {
        if (section.ContentVersion != SupportedContentVersion)
            throw new InvalidOperationException($"Unsupported content version '{section.ContentVersion}'.");

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
