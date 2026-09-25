namespace Nodeheim.Editor;

/// <summary>
/// Represents a position on the editor surface, independent of any UI framework.
/// </summary>
/// <param name="X">The horizontal coordinate.</param>
/// <param name="Y">The vertical coordinate.</param>
public readonly record struct SurfacePosition(double X, double Y);
