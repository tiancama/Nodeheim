namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents an owner together with its display name.
/// </summary>
/// <param name="Name">
/// The name of the owner; each of its section types starts with this name and a dot.
/// </param>
/// <param name="DisplayName">The name shown to the user.</param>
/// <remarks>
/// Further information about the owner is added only as optional <see langword="init"/> properties, so that
/// existing owners remain valid.
/// </remarks>
public sealed record OwnerDescriptor(string Name, string DisplayName);
