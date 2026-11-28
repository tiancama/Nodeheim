using Nodeheim.Persistence.Documents.Adapters;

namespace Nodeheim.Persistence.Json;

/// <summary>
/// Provides the JSON document format.
/// </summary>
public static class JsonFormat
{
    /// <summary>
    /// Gets the JSON document format.
    /// </summary>
    public static DocumentFormat Value { get; } =
        new("JSON", [".json"], new JsonDocumentParser(), stream => new JsonDocumentWriter(stream));
}
