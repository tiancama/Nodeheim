using System.Buffers;
using System.Text;
using System.Xml;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;
using Nodeheim.Persistence.Documents.Adapters;

namespace Nodeheim.Persistence.Xml;

/// <summary>
/// Represents a parser for documents in XML.
/// </summary>
public sealed class XmlDocumentParser : IDocumentParser
{
    private static readonly SearchValues<char> XmlWhitespace = SearchValues.Create(" \t\n\r");

    private static readonly UTF8Encoding StrictUtf8 =
        new(encoderShouldEmitUTF8Identifier: false, throwOnInvalidBytes: true);

    private static ReadOnlySpan<byte> Utf8Bom => [0xEF, 0xBB, 0xBF];

    /// <inheritdoc />
    public DocumentEntry? Parse(ReadOnlyMemory<byte> bytes, IIssueCollector issues)
    {
        DocumentEntryBuilder entryBuilder = new(issues);

        ReadOnlySpan<byte> utf8 = bytes.Span;
        if (utf8.StartsWith(Utf8Bom))
            utf8 = utf8[Utf8Bom.Length..];

        string xml;
        try
        {
            xml = StrictUtf8.GetString(utf8);
        }
        catch (DecoderFallbackException)
        {
            return Reject(issues, FormatIssues.InvalidEncoding,
                "Invalid encoding: the input is not valid UTF-8.", null);
        }

        XmlReaderSettings settings = new()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
            IgnoreComments = true,
            IgnoreProcessingInstructions = true,
        };

        try
        {
            using XmlReader reader = XmlReader.Create(new StringReader(xml), settings);

            StringBuilder pendingText = new();
            bool hasChildren = false;
            bool hasPendingContent = false;

            while (reader.Read())
            {
                switch (reader.NodeType)
                {
                    case XmlNodeType.XmlDeclaration:
                        string? encoding = reader.GetAttribute("encoding");
                        if (encoding is not null && !encoding.Equals("UTF-8", StringComparison.OrdinalIgnoreCase))
                            return Reject(issues, FormatIssues.InvalidEncoding,
                                "Invalid encoding: the XML declaration names an encoding other than UTF-8.", null);

                        break;
                    case XmlNodeType.Element:
                        if (reader.NamespaceURI != XmlNamespaces.Nodeheim)
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
                        hasPendingContent = false;
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
