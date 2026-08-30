using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Nodeheim.Domain;

namespace Nodeheim.Editor;

public partial class MainWindow : Window
{
    private readonly NodeViewModel _vm;
    
    public MainWindow()
    {
        InitializeComponent();
        var node = new Node();
        _vm = new NodeViewModel(node) {X=80, Y=100};
        DataContext = _vm;
    }

    private void OnCoordKeyDown(object? sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter)
            MainCanvas.Focus();
    }
}
