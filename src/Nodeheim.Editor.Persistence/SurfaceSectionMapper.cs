using Nodeheim.Editor.Domain;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;
using Nodeheim.Persistence.Documents.Owners.Engine;

namespace Nodeheim.Editor.Persistence;

/// <summary>
/// Represents the section mapper of <c>editor.surface</c>, which maps the section to a <see cref="Surface"/>.
/// </summary>
internal sealed class SurfaceSectionMapper : ISectionMapper<Surface>
{
    private const string ContentVersion = "0.1";
    private const string DefaultsName = "defaults";
    private const string NodesName = "nodes";
    private const string NodeName = "node";
    private const string IdName = "id";
    private const string XName = "x";
    private const string YName = "y";
    private const string RadiusName = "radius";

    public SectionTypeDescriptor SectionType { get; } = new("editor.surface", "Surface");

    public Surface? Read(DocumentSection? section, IIssueCollector issues)
    {
        if (section is null)
            return new Surface();

        DocumentEntry content = section.Content;
        DocumentEntry defaultNode = content.GetSingleChild(DefaultsName)!.GetSingleChild(NodeName)!;
        Surface surface = new(ReadNumber(defaultNode, RadiusName));

        foreach (DocumentEntry node in content.GetSingleChild(NodesName)!.GetChildrenNamed(NodeName)!)
        {
            NodeIdText.TryParse(node.GetSingleChild(IdName)!.GetLeafText(), out Guid id);
            SurfacePosition position = new(ReadNumber(node, XName), ReadNumber(node, YName));
            double? radius = node.GetSingleChild(RadiusName) is null ? null : ReadNumber(node, RadiusName);
            surface.SetNode(id, new SurfaceNode(position, radius));
        }

        return surface;
    }

    public DocumentSectionContribution Contribute(Surface model) =>
        new(SectionType.Name, ContentVersion, writer => Write(model, writer));

    private static void Write(Surface surface, IDocumentWriter writer)
    {
        List<(string Id, SurfaceNode Node)> nodes = [];
        foreach ((Guid id, SurfaceNode node) in surface.Nodes)
        {
            nodes.Add((NodeIdText.Format(id), node));
        }

        nodes.Sort((a, b) => NodeIdText.Comparer.Compare(a.Id, b.Id));

        writer.BeginGroup();

        writer.Name(DefaultsName);
        writer.BeginGroup();
        writer.Name(NodeName);
        writer.BeginGroup();
        writer.Name(RadiusName);
        writer.Text(NumberText.Format(surface.DefaultNodeRadius));
        writer.EndGroup();
        writer.EndGroup();

        writer.Name(NodesName);
        writer.BeginGroup();
        writer.Name(NodeName);
        writer.BeginRepeated();
        foreach ((string id, SurfaceNode node) in nodes)
        {
            writer.BeginGroup();
            writer.Name(IdName);
            writer.Text(id);
            writer.Name(XName);
            writer.Text(NumberText.Format(node.Position.X));
            writer.Name(YName);
            writer.Text(NumberText.Format(node.Position.Y));
            if (node.Radius is { } radius)
            {
                writer.Name(RadiusName);
                writer.Text(NumberText.Format(radius));
            }

            writer.EndGroup();
        }

        writer.EndRepeated();
        writer.EndGroup();

        writer.EndGroup();
    }

    private static double ReadNumber(DocumentEntry entry, string name)
    {
        NumberText.TryParse(entry.GetSingleChild(name)!.GetLeafText(), out double value);
        return value;
    }
}
