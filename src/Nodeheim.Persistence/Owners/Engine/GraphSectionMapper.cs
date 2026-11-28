using Nodeheim.Domain;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

/// <summary>
/// Represents the section mapper of <c>engine.graph</c>, which maps the section to a <see cref="Graph"/>.
/// </summary>
/// <remarks>
/// Format contract: The content version of <c>engine.graph</c> is <c>0.2</c>; another version rejects the section
/// with <c>engine.graph.unsupportedContentVersion</c>. Without <c>engine.graph</c>, the model is an empty graph.
/// </remarks>
internal sealed class GraphSectionMapper : ISectionMapper<Graph>
{
    private const string SupportedContentVersion = "0.2";

    public SectionTypeDescriptor SectionType { get; } = new("engine.graph");

    public Graph? Read(DocumentSection? section, IIssueCollector issues)
    {
        ArgumentNullException.ThrowIfNull(issues);

        if (section is null)
            return new Graph();

        if (section.ContentVersion != SupportedContentVersion)
        {
            issues.Add(new Issue(EngineIssues.UnsupportedContentVersion, "Unsupported content version.", null));
            return null;
        }

        return GraphMapper.Read(section.Content, issues);
    }

    /// <remarks>
    /// The contribution reads <paramref name="model"/> each time it writes its content, so the graph must not
    /// change while the document is saved.
    /// </remarks>
    public DocumentSectionContribution Contribute(Graph model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return new DocumentSectionContribution(SectionType.Name, SupportedContentVersion,
            writer => GraphMapper.Write(model, writer));
    }
}
