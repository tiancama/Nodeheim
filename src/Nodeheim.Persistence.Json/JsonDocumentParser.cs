using System.Diagnostics;
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
    // An entry in a repetition needs two levels of syntax, so the builder always reports a document tree that is
    // too deep before the reader does.
    private static readonly JsonReaderOptions ReaderOptions = new() { MaxDepth = 2 * DocumentEntry.MaxLevels + 1 };

    private static ReadOnlySpan<byte> Utf8Bom => [0xEF, 0xBB, 0xBF];

    /// <inheritdoc />
    public DocumentEntry? Parse(ReadOnlyMemory<byte> bytes, IIssueCollector issues)
    {
        ArgumentNullException.ThrowIfNull(issues);

        DocumentEntryBuilder entryBuilder = new(issues);

        ReadOnlySpan<byte> json = bytes.Span;
        if (json.StartsWith(Utf8Bom))
            json = json[Utf8Bom.Length..];

        Utf8JsonReader reader = new(json, ReaderOptions);

        try
        {
            reader.Read();
            if (!ReadValue(ref reader, DocumentMapper.RootName, entryBuilder, issues))
                return null;

            // Rejects any content after the root value.
            reader.Read();
        }
        catch (JsonException)
        {
            issues.Add(new Issue(FormatIssues.InvalidSyntax, "Invalid syntax: the input is not valid JSON.", null));
            return null;
        }

        return entryBuilder.GetResult();
    }

    private static bool ReadValue(
        ref Utf8JsonReader reader, string name, DocumentEntryBuilder entryBuilder, IIssueCollector issues)
    {
        if (reader.TokenType == JsonTokenType.StartArray)
            return Reject(issues, FormatIssues.UnmappableArray,
                "Unmappable array: an array is only permitted as the value of a property.");

        entryBuilder.BeginEntry(name);

        switch (reader.TokenType)
        {
            case JsonTokenType.StartObject:
                while (reader.Read() && reader.TokenType == JsonTokenType.PropertyName)
                {
                    if (!TryGetString(ref reader, out string childName))
                        return RejectInvalidString(issues);

                    reader.Read();
                    if (!ReadProperty(ref reader, childName, entryBuilder, issues))
                        return false;
                }

                break;
            case JsonTokenType.String:
                if (!TryGetString(ref reader, out string text))
                    return RejectInvalidString(issues);

                entryBuilder.Text(text);
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
                throw new UnreachableException($"Unexpected token '{reader.TokenType}' at the position of a value.");
        }

        entryBuilder.EndEntry();
        return true;
    }

    private static bool ReadProperty(
        ref Utf8JsonReader reader, string name, DocumentEntryBuilder entryBuilder, IIssueCollector issues)
    {
        if (reader.TokenType != JsonTokenType.StartArray)
            return ReadValue(ref reader, name, entryBuilder, issues);

        reader.Read();
        if (reader.TokenType == JsonTokenType.EndArray)
            return Reject(issues, FormatIssues.UnmappableArray,
                "Unmappable array: an array must not be empty.");

        do
        {
            if (!ReadValue(ref reader, name, entryBuilder, issues))
                return false;
        }
        while (reader.Read() && reader.TokenType != JsonTokenType.EndArray);

        return true;
    }

    private static bool TryGetString(ref Utf8JsonReader reader, out string value)
    {
        try
        {
            value = reader.GetString()!;
            return true;
        }
        catch (InvalidOperationException)
        {
            value = string.Empty;
            return false;
        }
    }

    private static bool RejectInvalidString(IIssueCollector issues) =>
        Reject(issues, FormatIssues.InvalidCharacter,
            "Invalid character: a string contains invalid UTF-8 or a lone surrogate.");

    private static bool Reject(IIssueCollector issues, IssueDefinition definition, string message)
    {
        issues.Add(new Issue(definition, message, null));
        return false;
    }
}
