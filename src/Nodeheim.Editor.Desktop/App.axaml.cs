using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Nodeheim.Persistence;
using Nodeheim.Persistence.Json;

namespace Nodeheim.Editor.Desktop;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            DocumentRepository documentRepository =
                new(new DocumentStorageDescriptor("JSON", [".json"]),
                    location => new JsonDocumentStorage(location));
            ShellViewModel shell = new(documentRepository);
            desktop.MainWindow = new MainWindow() { DataContext = shell };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
