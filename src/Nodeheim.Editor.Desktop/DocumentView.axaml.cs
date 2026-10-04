using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Nodeheim.Editor.Desktop;

public partial class DocumentView : UserControl
{
    public DocumentView()
    {
        InitializeComponent();
    }

    private InteractionController? Controller =>
        (DataContext as DocumentViewModel)?.InteractionController;

    private void OnCanvasPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is not Visual canvas)
            throw new ArgumentException("Handler must be attached to a Visual.", nameof(sender));

        if (Controller is not { } controller)
            return;

        HitTarget target = Resolve(e);
        var isCtrlPressed = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        var position = e.GetCurrentPoint(canvas).Position.ToSurfacePosition();

        if (isCtrlPressed)
        {
            controller.Toggle(target);
        }
        else
        {
            switch (target)
            {
                case EmptyHit:
                    controller.Exclusive(target);
                    break;
                default:
                    controller.Grab(target, position);
                    break;
            }
        }
    }

    private void OnCanvasPointerMoved(object? sender, PointerEventArgs e)
    {
        if (sender is not Visual canvas)
            throw new ArgumentException("Handler must be attached to a Visual.", nameof(sender));

        PointerPoint point = e.GetCurrentPoint(canvas);
        if (point.Properties.IsLeftButtonPressed)
        {
            Controller?.PointerMoved(point.Position.ToSurfacePosition());
        }
    }

    private void OnCanvasPointerReleased(object? sender, PointerReleasedEventArgs e) =>
        Controller?.PointerReleased();

    private void OnCanvasDoubleTapped(object? sender, TappedEventArgs e)
    {
        if (sender is not Visual canvas)
            throw new ArgumentException("Handler must be attached to a Visual.", nameof(sender));

        Controller?.CreateNode(e.GetPosition(canvas).ToSurfacePosition());
    }

    private void OnConnectionLineLoaded(object? sender, RoutedEventArgs e)
    {
        if (sender is not Line line || line.DataContext is not ConnectionViewModel connection)
            return;

        var selectionBinding = new MultiBinding
        {
            Converter = BoolConverters.And,
            Bindings =
            {
                new ReflectionBinding("IsSelected") { Source = connection.NodeA },
                new ReflectionBinding("IsSelected") { Source = connection.NodeB },
            },
        };

        // anchor is annotated non-nullable but unused (Avalonia 12.1.3);
        // Avalonia's XAML compiler passes null here for Classes bindings too.
        line.BindClass("selected", selectionBinding, null!);
    }

    private HitTarget Resolve(PointerPressedEventArgs e)
    {
        if (e.Source is StyledElement { DataContext: NodeViewModel node })
        {
            return new NodeHit(node);
        }

        if (e.Source is StyledElement { DataContext: ConnectionViewModel connection })
        {
            return new ConnectionHit(connection);
        }

        return new EmptyHit();
    }
}
