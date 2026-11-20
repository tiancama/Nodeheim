using System.Text.Encodings.Web;
using System.Text.Json;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence.Json;

/// <summary>
/// Represents a writer for documents in JSON.
/// </summary>
/// <remarks>
/// <para>
/// Format contract: The root group is written as an object without a name. A group is written as an object,
/// a repeated field as an array regardless of the number of values, and a text as a string. The output is
/// UTF-8 without a byte order mark, indented by two spaces, with LF as line ending and a final LF.
/// </para>
/// <para>
/// The writer detects the end of the document when the root group is closed. It then flushes its output,
/// writes the final LF, and releases its resources; it does not close the stream.
/// </para>
/// </remarks>
public sealed class JsonDocumentWriter : IDocumentWriter
{
    private static readonly JsonWriterOptions WriterOptions = new()
    {
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        Indented = true,
        IndentCharacter = ' ',
        IndentSize = 2,
        NewLine = "\n",
    };

    private readonly Stream _stream;
    private readonly Utf8JsonWriter _json;
    private int _depth;

    /// <summary>
    /// Initializes a new instance of the <see cref="JsonDocumentWriter"/> class.
    /// </summary>
    /// <param name="stream">The stream that receives the document; it stays open.</param>
    /// <exception cref="ArgumentNullException"><paramref name="stream"/> is <see langword="null"/>.</exception>
    public JsonDocumentWriter(Stream stream)
    {
        ArgumentNullException.ThrowIfNull(stream);

        _stream = stream;
        _json = new Utf8JsonWriter(stream, WriterOptions);
    }

    /// <inheritdoc />
    public void Name(string name)
    {
        if (_depth == 0) return;

        _json.WritePropertyName(name);
    }

    /// <inheritdoc />
    public void Text(string text) => _json.WriteStringValue(text);

    /// <inheritdoc />
    public void BeginGroup()
    {
        _json.WriteStartObject();
        _depth++;
    }

    /// <inheritdoc />
    public void EndGroup()
    {
        _json.WriteEndObject();
        _depth--;

        if (_depth == 0)
            Complete();
    }

    /// <inheritdoc />
    public void BeginRepeated() => _json.WriteStartArray();

    /// <inheritdoc />
    public void EndRepeated() => _json.WriteEndArray();

    private void Complete()
    {
        _json.Flush();
        _json.Dispose();
        _stream.WriteByte((byte)'\n');
    }
}
