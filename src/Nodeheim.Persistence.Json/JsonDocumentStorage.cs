using System.Diagnostics;
using System.Text.Json;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence.Json;

public sealed class JsonDocumentStorage : IDocumentStorage
{
    private readonly string _path;

    public JsonDocumentStorage(string path)
    {
        _path = path;
    }

    /// <inheritdoc />
    public DocumentValue? Load(IIssueCollector issues)
    {
        byte[] bytes = File.ReadAllBytes(_path);
        Utf8JsonReader reader = new(bytes);
        DocumentValueBuilder builder = new(issues);

        try
        {
            while (reader.Read())
            {
                switch (reader.TokenType)
                {
                    case JsonTokenType.StartObject: builder.BeginCompound(); break;
                    case JsonTokenType.EndObject: builder.EndCompound(); break;
                    case JsonTokenType.StartArray: builder.BeginList(); break;
                    case JsonTokenType.EndArray: builder.EndList(); break;
                    case JsonTokenType.PropertyName: builder.Key(reader.GetString()!); break;
                    case JsonTokenType.String: builder.Text(reader.GetString()!); break;
                    case JsonTokenType.True: builder.Boolean(true); break;
                    case JsonTokenType.False: builder.Boolean(false); break;
                    case JsonTokenType.Null: builder.Null(); break;
                    default: throw new UnreachableException();
                }
            }

            return builder.GetResult();
        }
        catch (JsonException exception)
        {
            issues.Add(new Issue(FormatIssues.InvalidSyntax, exception.Message, null));
            return null;
        }
    }
}
