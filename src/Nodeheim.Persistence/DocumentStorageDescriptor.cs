namespace Nodeheim.Persistence;

/// <summary>
/// Represents the description of a kind of document storage, such as JSON files, for display and file selection.
/// </summary>
/// <param name="DisplayName">The name shown to the user, such as JSON.</param>
/// <param name="FileExtensions">
/// The file extensions, each with a leading period, such as <c>.json</c>; empty if the storage does not use files.
/// </param>
public sealed record DocumentStorageDescriptor(string DisplayName, IReadOnlyList<string> FileExtensions);
