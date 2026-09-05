using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;

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
            _vm.SelectSingleNode(node);
        else
            _vm.ClearSelection();

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
}
