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
    private const string NodeheimNamespace = "https://schemas.nodeheim.net/2026/anthology";

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

        try
        {
            using XmlReader reader = XmlReader.Create(stream, settings);

            StringBuilder pendingText = new();
            bool hasChildren = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.Element:
                        if (reader.NamespaceURI != NodeheimNamespace)
                        {
                            if (reader.Depth == 0)
                                return Reject(issues, FormatIssues.NotNodeheimDocument,
                                    "Not a Nodeheim document: the root element is not in the Nodeheim namespace.", "");

                            return Reject(issues, FormatIssues.ForeignElement,
                                "Foreign element: an element below the root is not in the Nodeheim namespace.", null);
                        }
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
        }
        catch (XmlException)
        {
            return Reject(issues, FormatIssues.InvalidSyntax,
                "Invalid syntax: the input is not well-formed XML.", null);
        }

        return entryBuilder.GetResult();
    }

    private static DocumentEntry? Reject(IIssueCollector issues, IssueDefinition definition, string message,
        string? path)
    {
        issues.Add(new Issue(definition, message, path));
        return null;
    }
}
