using System.Reflection;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Platform.Storage;

namespace Nodeheim.Editor.Desktop;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private ShellViewModel Shell =>
        DataContext as ShellViewModel ?? throw new InvalidOperationException("DataContext must be a ShellViewModel.");

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        InteractionController controller = Shell.ActiveDocument.InteractionController;

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

    private void OnNewClick(object? sender, RoutedEventArgs e) => Shell.New();

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

    private async void OnOpenClick(object? sender, RoutedEventArgs e)
    {
        IStorageProvider? storageProvider = TopLevel.GetTopLevel(this)?.StorageProvider;
        if (storageProvider is null)
            return;

        IReadOnlyList<IStorageFile> files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "Open",
            AllowMultiple = false,
            FileTypeFilter = [new FilePickerFileType("JSON") { Patterns = ["*.json"] }],
        });

        if (files is not [{ } file] || file.TryGetLocalPath() is not { } path)
            return;

        string? report = Shell.Load(path);
        if (report is not null)
            ShowIssues(report);
    }

    private void ShowIssues(string report)
    {
        var window = new Window
        {
            Title = "Issues",
            Width = 600,
            Height = 300,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            Content = new ScrollViewer
            {
                Content = new SelectableTextBlock
                {
                    Text = report,
                    TextWrapping = TextWrapping.Wrap,
                    Margin = new Thickness(10)
                }
            }
        };
        window.Show(this);
    }
}
