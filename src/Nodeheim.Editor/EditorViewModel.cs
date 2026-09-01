using System.Collections.ObjectModel;
using Nodeheim.Domain;

namespace Nodeheim.Editor;

public class EditorViewModel
{
    private readonly Graph _graph = new();

    public ObservableCollection<NodeViewModel> Nodes { get; } = new();

    public EditorViewModel()
    {
        foreach (var node in _graph.Nodes)
        {
            NodeViewModel nodeViewModel = new(node);
            Nodes.Add(nodeViewModel);
        }
    }
}
