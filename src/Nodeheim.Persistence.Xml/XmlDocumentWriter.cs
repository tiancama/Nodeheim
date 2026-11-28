using System.Text;
using System.Xml;
using Nodeheim.Persistence.Documents;
using Nodeheim.Persistence.Documents.Adapters;

namespace Nodeheim.Persistence.Xml;

/// <summary>
/// Represents a writer for documents in XML.
/// </summary>
/// <remarks>
/// <para>
/// Format contract: Each value is written as an element named after its field, in the Nodeheim namespace as
/// the default namespace without a prefix; each value of a repeated field becomes an element with the name of
/// the field. Text is escaped, without CDATA, and a carriage return is written as <c>&amp;#xD;</c>. An empty
/// text, a group without fields, and a group whose repeated fields have no values are written as
/// <c>&lt;x /&gt;</c>. The output starts with an XML declaration and is UTF-8 without a byte order mark,
/// indented by two spaces, with LF as line ending and a final LF.
/// </para>
/// <para>
/// The writer detects the end of the document when the root group is closed. It then releases its
/// <see cref="XmlWriter"/>, which flushes the output, and writes the final LF; it does not close the stream.
/// </para>
/// </remarks>
internal sealed class XmlDocumentWriter : IDocumentWriter
{
    private static readonly XmlWriterSettings WriterSettings = new()
    {
        Encoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false),
        Indent = true,
        IndentChars = "  ",
        NewLineChars = "\n",
        NewLineHandling = NewLineHandling.Entitize,
        CloseOutput = false,
    };

    private readonly Stream _stream;
    private readonly XmlWriter _xml;
    private readonly DocumentWriterProtocol _protocol = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="XmlDocumentWriter"/> class.
    /// </summary>
    /// <param name="stream">The stream that receives the document; it stays open.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    public XmlDocumentWriter(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        _stream = stream;
        _xml = XmlWriter.Create(stream, WriterSettings);
    }

    /// <inheritdoc />
    public void Name(string name) => _protocol.Name(name);

    /// <inheritdoc />
    public void Text(string text)
    {
        string name = _protocol.Text();
        _xml.WriteStartElement(name, XmlNamespaces.Nodeheim);

        // An empty string counts as content for XmlWriter and would give <x></x> instead of <x />.
        if (text.Length > 0)
            _xml.WriteString(text);

        _xml.WriteEndElement();
    }

    /// <inheritdoc />
    public void BeginGroup()
    {
        bool isRoot = _protocol.OpenGroupCount == 0;
        string name = _protocol.BeginGroup();

        if (isRoot)
            _xml.WriteStartDocument();

        _xml.WriteStartElement(name, XmlNamespaces.Nodeheim);
    }

    /// <inheritdoc />
    public void EndGroup()
    {
        _protocol.EndGroup();
        _xml.WriteEndElement();

        if (_protocol.OpenGroupCount == 0)
            Complete();
    }

    /// <inheritdoc />
    public void BeginRepeated() => _protocol.BeginRepeated();

    /// <inheritdoc />
    public void EndRepeated() => _protocol.EndRepeated();

    private void Complete()
    {
        _xml.Flush();
        _xml.Dispose();
        _stream.WriteByte((byte)'\n');
    }
}
