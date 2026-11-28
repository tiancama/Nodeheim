namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a section type.
/// </summary>
/// <param name="Name">The section type; it starts with the name of its owner and a dot.</param>
public sealed record SectionTypeDescriptor(string Name);
