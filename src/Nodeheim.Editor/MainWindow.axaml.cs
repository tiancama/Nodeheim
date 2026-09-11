using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Nodeheim.Editor;

public partial class MainWindow : Window
{
    private readonly EditorViewModel _vm = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;
    }

    private void OnCanvasPointerPressed(object? sender, PointerPressedEventArgs e)
    {
        bool ctrl = e.KeyModifiers.HasFlag(KeyModifiers.Control);
        if (e.Source is StyledElement { DataContext: NodeViewModel node })
        {
            if (ctrl)
            {
                _vm.ToggleSelected(node);
            }
            else
            {
                if (!node.IsSelected)
                    _vm.SelectOnly(node);

                _vm.BeginDrag(e.GetCurrentPoint((Visual)sender).Position.ToSurfacePosition());
            }
        }
        else
        {
            if (!ctrl)
                _vm.DeselectAll();
        }
    }

    private void OnCanvasPointerMoved(object? sender, PointerEventArgs e)
    {
        PointerPoint point = e.GetCurrentPoint((Visual)sender);
        if (point.Properties.IsLeftButtonPressed)
        {
            _vm.UpdateDrag(point.Position.ToSurfacePosition());
        }
    }

    private void OnCanvasPointerReleased(object? sender, PointerReleasedEventArgs e) => _vm.EndDrag();

    private void OnCanvasDoubleTapped(object? sender, TappedEventArgs e) =>
        _vm.CreateNode(e.GetPosition((Visual)sender).ToSurfacePosition());

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
}
