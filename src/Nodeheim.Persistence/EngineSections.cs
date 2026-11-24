using Nodeheim.Domain;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

/// <summary>
/// Represents the section mapper of the owner <c>engine</c>, which maps the section <c>engine.graph</c> to a
/// <see cref="Graph"/>.
/// </summary>
/// <remarks>
/// Format contract: The content version of <c>engine.graph</c> is <c>0.2</c>; another version rejects the section
/// with <c>engine.graph.unsupportedContentVersion</c>. Without <c>engine.graph</c>, the model is an empty graph.
/// </remarks>
public sealed class EngineSections : ISectionMapper<Graph>
{
    private const string GraphSectionType = "engine.graph";

    private const string SupportedContentVersion = "0.2";

    private static readonly IReadOnlyList<SectionTypeDescriptor> GraphSectionTypes =
        [new SectionTypeDescriptor(GraphSectionType, "Graph")];

    /// <inheritdoc />
    public string OwnerName => "engine";

    /// <inheritdoc />
    public string DisplayName => "Engine";

    /// <inheritdoc />
    public IReadOnlyList<SectionTypeDescriptor> SectionTypes => GraphSectionTypes;

    /// <inheritdoc />
    public Graph? Read(IReadOnlyDictionary<string, DocumentSection> sections, IIssueCollector issues)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(issues);

        if (!sections.TryGetValue(GraphSectionType, out DocumentSection? section))
            return new Graph();

        if (section.ContentVersion != SupportedContentVersion)
        {
            issues.Add(new Issue(EngineIssues.UnsupportedContentVersion, "Unsupported content version.", null));
            return null;
        }

        return GraphMapper.Read(section.Content, issues);
    }

    /// <inheritdoc />
    /// <remarks>
    /// The contribution reads <paramref name="model"/> each time it writes its content, so the graph must not
    /// change while the document is saved.
    /// </remarks>
    public IReadOnlyList<DocumentSectionContribution> Contribute(Graph model)
    {
        ArgumentNullException.ThrowIfNull(model);

        return [new DocumentSectionContribution(GraphSectionType, SupportedContentVersion,
            writer => GraphMapper.Write(model, writer))];
    }
}
