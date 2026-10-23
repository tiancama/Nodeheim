using System.Diagnostics;
using System.Text.Json;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence.Json;

public sealed class JsonDocumentStorage : IDocumentStorage
{
    private readonly string _path;
    private static ReadOnlySpan<byte> Utf8Bom => [0xEF, 0xBB, 0xBF];

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
    /// <see cref="FormatIssues.InvalidSyntax"/>, <see cref="FormatIssues.NumberNotRepresentable"/>, and
    /// <see cref="FormatIssues.InvalidCharacter"/>.
    /// </para>
    /// <para>
    /// Format contract: The input is UTF-8; a byte order mark at the beginning is ignored. A number with <c>.</c>,
    /// <c>e</c>, or <c>E</c> is a real, such as <c>1.0</c>; every other number is an integer.
    /// </para>
    /// </remarks>
    public DocumentValue? Load(IIssueCollector issues)
    {
        byte[] bytes = File.ReadAllBytes(_path);
        ReadOnlySpan<byte> json = bytes;
        if (json.StartsWith(Utf8Bom)) json = json[Utf8Bom.Length..];
        Utf8JsonReader reader = new(json);
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
                    case JsonTokenType.PropertyName:
                        if (!TryGetString(ref reader, issues, out string key)) return null;
                        builder.Key(key); break;
                    case JsonTokenType.String:
                        if (!TryGetString(ref reader, issues, out string text)) return null;
                        builder.Text(text);
                        break;
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

    /// <summary>
    /// Decodes the current JSON string token or property name token.
    /// </summary>
    /// <returns>
    /// <see langword="true"/> if the token has been decoded; otherwise, <see langword="false"/>, and a rejection
    /// has been added to <paramref name="issues"/>.
    /// </returns>
    /// <remarks>
    /// Only <see cref="Utf8JsonReader.GetString"/> is enclosed in the <see langword="try"/> block, not the write
    /// call to the builder: <see cref="DocumentValueBuilder"/> also throws <see cref="InvalidOperationException"/>,
    /// and there it indicates a contract violation, not a rejection.
    /// </remarks>
    private static bool TryGetString(ref Utf8JsonReader reader, IIssueCollector issues, out string value)
    {
        try
        {
            value = reader.GetString()!;
            return true;
        }
        catch (InvalidOperationException)
        {
            issues.Add(new Issue(FormatIssues.InvalidCharacter,
                "A text or key contains a lone surrogate or invalid UTF-8.", null));
            value = "";
            return false;
        }
    }
}
