namespace Nodeheim.Persistence.Xml;

/// <summary>
/// Provides the XML namespaces of Nodeheim documents.
/// </summary>
internal static class XmlNamespaces
{
    /// <summary>
    /// The namespace of all elements in a Nodeheim document.
    /// </summary>
    /// <remarks>
    /// Format contract: The namespace is compared ordinally and never changes; the year denotes when it was
    /// defined, not a version.
    /// </remarks>
    internal const string Nodeheim = "https://schemas.nodeheim.net/2026/anthology";
}
