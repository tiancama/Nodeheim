using System.Text;
using System.Text.Json;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence.Json;

/// <summary>
/// Represents a parser for documents in JSON.
/// </summary>
public sealed class JsonDocumentParser : IDocumentParser
{
    private static ReadOnlySpan<byte> Utf8Bom => [0xEF, 0xBB, 0xBF];

    /// <inheritdoc />
    public DocumentEntry? Parse(ReadOnlyMemory<byte> bytes, IIssueCollector issues)
    {
        ArgumentNullException.ThrowIfNull(issues);

        DocumentEntryBuilder entryBuilder = new(issues);

        ReadOnlySpan<byte> json = bytes.Span;
        if (json.StartsWith(Utf8Bom))
            json = json[Utf8Bom.Length..];

        Utf8JsonReader reader = new(json);
        reader.Read();
        ReadValue(ref reader, DocumentMapper.RootName, entryBuilder);

        return entryBuilder.GetResult();
    }

    private static void ReadValue(ref Utf8JsonReader reader, string name, DocumentEntryBuilder entryBuilder)
    {
        entryBuilder.BeginEntry(name);

        switch (reader.TokenType)
        {
            case JsonTokenType.StartObject:
                while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
                {
                    string childName = reader.GetString()!;
                    reader.Read();
                    ReadProperty(ref reader, childName, entryBuilder);
                }

                break;
            case JsonTokenType.String:
                entryBuilder.Text(reader.GetString()!);
                break;
            case JsonTokenType.Number:
                entryBuilder.Text(Encoding.UTF8.GetString(reader.ValueSpan));
                break;
            case JsonTokenType.True:
                entryBuilder.Text("true");
                break;
            case JsonTokenType.False:
                entryBuilder.Text("false");
                break;
            case JsonTokenType.Null:
                break;
            default:
                // Replaced by a rejection when loading is secured (array outside an object).
                throw new NotSupportedException("An array outside an object is not supported yet.");
        }

        entryBuilder.EndEntry();
    }

    private static void ReadProperty(ref Utf8JsonReader reader, string name, DocumentEntryBuilder entryBuilder)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
        {
            ReadValue(ref reader, name, entryBuilder);
            return;
        }

        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray)
        {
            ReadValue(ref reader, name, entryBuilder);
        }
    }
}
