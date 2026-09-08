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
        if (e.Source is StyledElement { DataContext: NodeViewModel node })
            _vm.SelectOnly(node);
        else
            _vm.DeselectAll();

        _vm.BeginDrag(e.GetCurrentPoint((Visual)sender).Position.ToSurfacePosition());
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
