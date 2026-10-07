namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a named, versioned block of a document.
/// </summary>
/// <param name="ContentVersion">The version of the content.</param>
/// <param name="Content">The content.</param>
public sealed record DocumentSection(string ContentVersion, DocumentValue Content);
