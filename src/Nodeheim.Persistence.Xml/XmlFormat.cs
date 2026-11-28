using Nodeheim.Persistence.Documents.Adapters;

namespace Nodeheim.Persistence.Xml;

/// <summary>
/// Provides the XML document format.
/// </summary>
public static class XmlFormat
{
    /// <summary>
    /// Gets the XML document format.
    /// </summary>
    public static DocumentFormat Value { get; } =
        new("XML", [".xml"], new XmlDocumentParser(), stream => new XmlDocumentWriter(stream));
}
