using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Nodeheim.Domain;

namespace Nodeheim.Editor;

/// <summary>
/// Represents the editor application with its currently active document.
/// </summary>
public class ShellViewModel : INotifyPropertyChanged
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class with an empty document.
    /// </summary>
    public ShellViewModel()
    {
        New();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the document that is currently open in the editor.
    /// </summary>
    public DocumentViewModel ActiveDocument { get; private set => SetProperty(ref field, value); }

    /// <summary>
    /// Replaces the active document with a new document containing an empty graph.
    /// </summary>
    [MemberNotNull(nameof(ActiveDocument))]
    public void New() => ActiveDocument = new(new Graph());

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return;
        storage = value;
        OnPropertyChanged(name);
    }
}
