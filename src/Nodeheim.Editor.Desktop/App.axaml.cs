using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Nodeheim.Persistence;
using Nodeheim.Persistence.Files;
using Nodeheim.Persistence.Xml;

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
                new(new DocumentStorageDescriptor("XML", [".xml"]),
                    location => new FileDocumentStorage(location, new XmlDocumentParser()));
            ShellViewModel shell = new(documentRepository);
            desktop.MainWindow = new MainWindow() { DataContext = shell };
        }

        base.OnFrameworkInitializationCompleted();
    }
}
