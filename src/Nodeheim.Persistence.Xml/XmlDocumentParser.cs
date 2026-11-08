using System.Runtime.InteropServices;
using System.Text;
using System.Xml;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence.Xml;

/// <summary>
/// Represents a parser for documents in XML.
/// </summary>
public sealed class XmlDocumentParser : IDocumentParser
{
    /// <inheritdoc />
    public DocumentEntry? Parse(ReadOnlyMemory<byte> bytes, IIssueCollector issues)
    {
        DocumentEntryBuilder entryBuilder = new(issues);

        using Stream stream = MemoryMarshal.TryGetArray(bytes, out ArraySegment<byte> segment)
            ? new MemoryStream(segment.Array!, segment.Offset, segment.Count, writable: false)
            : new MemoryStream(bytes.ToArray(), writable: false);

        XmlReaderSettings settings = new()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
        };

        using XmlReader reader = XmlReader.Create(stream, settings);

        StringBuilder pendingText = new();
        bool hasChildren = false;

        while (reader.Read())
        {
            switch (reader.NodeType)
            {
                case XmlNodeType.Element:
                    pendingText.Clear();
                    hasChildren = false;
                    entryBuilder.BeginEntry(reader.LocalName);
                    if (reader.IsEmptyElement)
                    {
                        entryBuilder.EndEntry();
                        hasChildren = true;
                    }

                    break;
                case XmlNodeType.EndElement:
                    if (!hasChildren)
                    {
                        entryBuilder.Text(pendingText.ToString());
                    }

                    entryBuilder.EndEntry();
                    hasChildren = true;
                    break;
                case XmlNodeType.Text:
                case XmlNodeType.CDATA:
                case XmlNodeType.Whitespace:
                case XmlNodeType.SignificantWhitespace:
                    if (!hasChildren)
                    {
                        pendingText.Append(reader.Value);
                    }

                    break;
                default:
                    break;
            }
        }

        return entryBuilder.GetResult();
    }
}
