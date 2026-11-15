using System.Buffers;
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

    private static readonly SearchValues<char> XmlWhitespace = SearchValues.Create(" \t\n\r");

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
            bool hasPendingContent = false;

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
                        if (hasPendingContent)
                            return RejectMixedContent(issues);

                        pendingText.Clear();
                        hasPendingContent = false;
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
                        string value = reader.Value;
                        bool isWhitespace = !value.AsSpan().ContainsAnyExcept(XmlWhitespace);

                        if (hasChildren && !isWhitespace)
                            return RejectMixedContent(issues);

                        if (!hasChildren)
                        {
                            pendingText.Append(value);
                            hasPendingContent |= !isWhitespace;
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

    private static DocumentEntry? RejectMixedContent(IIssueCollector issues) =>
        Reject(issues, FormatIssues.MixedContent,
            "Mixed content: an element contains both child elements and text that is not only whitespace.", null);
}
