using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Nodeheim.Editor;

public partial class MainWindow : Window
{
    private readonly EditorViewModel _vm = new();
    private readonly InteractionController _controller;

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;
        _controller = new(_vm);
    }

    private void OnCanvasPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        HitTarget target = Resolve(e);
        var isCtrlPressed = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        var position = e.GetCurrentPoint((Visual)sender).Position.ToSurfacePosition();

        if (isCtrlPressed)
        {
            _controller.Toggle(target);
        }
        else
        {
            switch (target)
            {
                case EmptyHit:
                    _controller.Exclusive(target);
                    break;
                default:
                    _controller.Grab(target, position);
                    break;
            }
        }
    }

    private void OnCanvasPointerMoved(object? sender, PointerEventArgs e)
    {
        PointerPoint point = e.GetCurrentPoint((Visual)sender);
        if (point.Properties.IsLeftButtonPressed)
        {
            _controller.PointerMoved(point.Position.ToSurfacePosition());
        }
    }

    private void OnCanvasPointerReleased(object? sender, PointerReleasedEventArgs e) =>
        _controller.PointerReleased();

    private void OnCanvasDoubleTapped(object? sender, TappedEventArgs e)
    {
        _controller.CreateNode(e.GetPosition((Visual)sender).ToSurfacePosition());
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Key == Key.Delete)
        {
            _controller.DeleteNodes();
            e.Handled = true;
        }
        else if (e.Key == Key.C)
        {
            _controller.ConnectNodes();
        }
        else if (e.Key == Key.D)
        {
            _controller.DisconnectNodes();
        }
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

        line.BindClass("selected", selectionBinding, null);
    }

    private void OnQuitClick(object? sender, RoutedEventArgs e) => Close();

    private void OnAboutClick(object? sender, RoutedEventArgs e)
    {
        var version = Assembly.GetEntryAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion.Split('+')[0];
        var dialog = new Window
        {
            Title = "About",
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new TextBlock
            {
                Text = $"Nodeheim Editor\nVersion {version}",
                Margin = new Thickness(20),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            }
        };
        dialog.ShowDialog(this);
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
