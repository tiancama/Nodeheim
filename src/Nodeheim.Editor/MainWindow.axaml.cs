using Avalonia.Controls;

namespace Nodeheim.Editor;

public partial class MainWindow : Window
{
    private readonly EditorViewModel _vm = new();

    public MainWindow()
    {
        InitializeComponent();
        DataContext = _vm;
    }
}
