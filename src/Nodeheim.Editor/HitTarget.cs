namespace Nodeheim.Editor;

public abstract record HitTarget;
public sealed record NodeHit(NodeViewModel Node) : HitTarget;
public sealed record ConnectionHit(ConnectionViewModel Connection) : HitTarget;
public sealed record EmptyHit : HitTarget;
