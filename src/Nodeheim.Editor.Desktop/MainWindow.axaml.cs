using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;

namespace Nodeheim.Editor.Desktop;

public partial class MainWindow : Window
{
    private readonly ShellViewModel _shellViewModel;

    public MainWindow()
    {
        InitializeComponent();
        _shellViewModel = new ShellViewModel();
        DataContext = _shellViewModel;
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        InteractionController controller = _shellViewModel.ActiveDocument.InteractionController;

        if (e.Key == Key.Delete)
        {
            controller.DeleteNodes();
            e.Handled = true;
        }
        else if (e.Key == Key.C)
        {
            controller.ConnectNodes();
        }
        else if (e.Key == Key.D)
        {
            controller.DisconnectNodes();
        }
    }

    private void OnNewClick(object? sender, RoutedEventArgs e) => _shellViewModel.New();

    private void OnQuitClick(object? sender, RoutedEventArgs e) => Close();

    private void OnAboutClick(object? sender, RoutedEventArgs e)
    {
        string? version = Assembly.GetEntryAssembly()
            ?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion.Split('+')[0];
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
