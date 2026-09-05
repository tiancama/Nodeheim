using Avalonia;

namespace Nodeheim.Editor;

public static class PointExtensions
{
    public static SurfacePosition ToSurfacePosition(this Point point) => new(point.X, point.Y);
}
