using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Nodeheim.Persistence;
using Nodeheim.Persistence.Files;
using Nodeheim.Persistence.Documents;
using Nodeheim.Persistence.Json;
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
            DocumentRepository documentRepository = new(
                [
                    new DocumentStorageDescriptor("JSON", [".json"]),
                    new DocumentStorageDescriptor("XML", [".xml"]),
                ],
                location => new FileDocumentStorage(location, CreateParser(location)));
            ShellViewModel shell = new(documentRepository);
            desktop.MainWindow = new MainWindow() { DataContext = shell };
        }

        base.OnFrameworkInitializationCompleted();
    }

    // Provisional until the format is detected from the content.
    private static IDocumentParser CreateParser(string location) =>
        Path.GetExtension(location).Equals(".xml", StringComparison.OrdinalIgnoreCase)
            ? new XmlDocumentParser()
            : new JsonDocumentParser();
}
