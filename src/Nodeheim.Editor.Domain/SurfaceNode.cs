namespace Nodeheim.Editor.Domain;

/// <summary>
/// Represents the appearance of a node on the editor surface: its position and, if it deviates from the
/// default, its radius.
/// </summary>
/// <param name="Position">The center position of the node.</param>
/// <param name="Radius">
/// The radius of the node, or <see langword="null"/> if the node uses <see cref="Surface.DefaultNodeRadius"/>.
/// </param>
public readonly record struct SurfaceNode(SurfacePosition Position, double? Radius = null);
