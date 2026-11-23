namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a section type together with its display name.
/// </summary>
/// <param name="SectionType">The section type; it starts with the name of its owner and a dot.</param>
/// <param name="DisplayName">The name shown to the user.</param>
public sealed record SectionTypeDescriptor(string SectionType, string DisplayName);
