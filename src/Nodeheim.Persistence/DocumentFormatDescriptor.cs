namespace Nodeheim.Persistence;

/// <summary>
/// Represents the description of a document format, such as JSON, for display and file selection.
/// </summary>
/// <param name="Name">The name of the document format, such as JSON.</param>
/// <param name="FileExtensions">
/// The file extensions of the document format, each with a leading period, such as <c>.json</c>.
/// </param>
public sealed record DocumentFormatDescriptor(string Name, IReadOnlyList<string> FileExtensions);
