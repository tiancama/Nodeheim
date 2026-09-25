namespace Nodeheim.Editor;

/// <summary>
/// Represents the result of a hit test on the editor surface.
/// </summary>
public abstract record HitTarget;

/// <summary>
/// Represents a hit on a node.
/// </summary>
/// <param name="Node">The node that was hit.</param>
public sealed record NodeHit(NodeViewModel Node) : HitTarget;

/// <summary>
/// Represents a hit on a connection.
/// </summary>
/// <param name="Connection">The connection that was hit.</param>
public sealed record ConnectionHit(ConnectionViewModel Connection) : HitTarget;

/// <summary>
/// Represents a hit on empty space.
/// </summary>
public sealed record EmptyHit : HitTarget;
