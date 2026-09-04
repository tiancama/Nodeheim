using System.Collections.ObjectModel;
using Nodeheim.Domain;

namespace Nodeheim.Editor;

public class EditorViewModel
{
    private readonly Graph _graph = new();
    private readonly HashSet<NodeViewModel> _selectedNodes = new ();

    public EditorViewModel()
    {
        _graph.AddNode(new Node());
        _graph.AddNode(new Node());
        _graph.AddNode(new Node());
        int startX = 666;
        foreach (var node in _graph.Nodes)
        {
            NodeViewModel nodeViewModel = new(node) { X = startX, Y = 256 };
            startX += 42;
            Nodes.Add(nodeViewModel);
        }
    }

    public ObservableCollection<NodeViewModel> Nodes { get; } = new();

    public void ClearSelection()
    {
        foreach (var node in _selectedNodes)
            node.IsSelected = false;

        _selectedNodes.Clear();
    }

    public void SelectSingleNode(NodeViewModel selectedNode)
    {
         ClearSelection();

         selectedNode.IsSelected = true;
         _selectedNodes.Add(selectedNode);
    }
}
