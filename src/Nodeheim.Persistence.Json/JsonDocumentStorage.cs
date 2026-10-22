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
    /// <remarks>
    /// <para>
    /// <inheritdoc path="/remarks/node()" />
    /// </para>
    /// <para>
    /// Besides the rejections of <see cref="DocumentValueBuilder"/>, this method adds
    /// <see cref="FormatIssues.InvalidSyntax"/> and <see cref="FormatIssues.NumberNotRepresentable"/>. After either
    /// of them, it stops reading and returns <see langword="null"/>.
    /// </para>
    /// <para>
    /// Format contract: The lexical form of a JSON number determines its value kind. A number without <c>.</c>,
    /// <c>e</c>, and <c>E</c> is an integer, such as <c>-0</c>; every other number is a real, such as <c>1.0</c> or
    /// <c>1e3</c>. An integer outside the range of <see cref="long"/> is rejected and never read as a real. A real
    /// is rounded to the nearest <see cref="double"/>. The range of the value model is checked by
    /// <see cref="DocumentValueBuilder"/>.
    /// </para>
    /// </remarks>
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
                    case JsonTokenType.Number:
                        if(!TryReportNumber(ref reader, builder, issues)) return null;
                        break;
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

    /// <summary>
    /// Reports the current JSON number token to the builder as an integer or a real.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the number has been passed to <paramref name="builder"/>; otherwise,
    /// <see langword="false"/>, and a rejection has been added to <paramref name="issues"/>.
    /// </returns>
    /// <remarks>
    /// The check for <c>.</c>, <c>e</c>, and <c>E</c> comes before <see cref="Utf8JsonReader.TryGetDouble"/>, so
    /// an integer outside the range of <see cref="long"/> is never read as a real. A real outside the range of
    /// <see cref="double"/> is read as infinity and rejected by <see cref="DocumentValueBuilder"/>.
    /// </remarks>
    private static bool TryReportNumber(ref Utf8JsonReader reader, DocumentValueBuilder builder, IIssueCollector issues)
    {
        if (reader.TryGetInt64(out long integer))
        {
            builder.Integer(integer);
            return true;
        }

        if (reader.ValueSpan.IndexOfAny((byte)'.',(byte)'e',(byte)'E') < 0)
        {
            issues.Add(new Issue(FormatIssues.NumberNotRepresentable,
                "An integer is outside the range of a 64-bit integer.", null));
            return false;
        }

        if (!reader.TryGetDouble(out double real))
        {
            issues.Add(new Issue(FormatIssues.NumberNotRepresentable,
                "A real cannot be represented as a double.", null));
            return false;
        }

        builder.Real(real);
        return true;
    }
}
