using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Nodeheim.Issues;
using Nodeheim.Persistence;

namespace Nodeheim.Editor;

/// <summary>
/// Represents the editor application with its currently active document.
/// </summary>
public class ShellViewModel : INotifyPropertyChanged
{
    private readonly IDocumentRepository _documentRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="ShellViewModel"/> class with an empty document.
    /// </summary>
    /// <param name="documentRepository">The repository through which documents are loaded.</param>

    public ShellViewModel(IDocumentRepository documentRepository)
    {
        _documentRepository = documentRepository;
        New();
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Gets the document that is currently open in the editor.
    /// </summary>
    public DocumentViewModel ActiveDocument { get; private set => SetProperty(ref field, value); }

    public IReadOnlyList<DocumentStorageDescriptor> StorageDescriptors => _documentRepository.StorageDescriptors;

    [MemberNotNull(nameof(ActiveDocument))]
    public void Open(LoadedDocument document) => ActiveDocument = new(document);

    /// <summary>
    /// Replaces the active document with a new, empty document.
    /// </summary>
    [MemberNotNull(nameof(ActiveDocument))]
    public void New() => Open(new LoadedDocument());

    /// <summary>
    /// Loads the document at the specified location and opens it as the active document.
    /// </summary>
    /// <param name="location">The storage location of the document, such as a file path.</param>
    /// <returns>
    /// A report of the issues found while loading, one line per issue, or <see langword="null"/> if there are none.
    /// </returns>
    /// <remarks>
    /// If the document is rejected, the active document remains unchanged. A report alone does not indicate a
    /// failure, because it can consist of discrepancies only. Exceptions from the document repository are not
    /// caught.
    /// </remarks>
    public string? Load(string location)
    {
        IssueCollector collector = new();
        LoadedDocument? document = _documentRepository.Load(location, collector);

        if (document is not null)
            Open(document);

        return collector.Issues.Count == 0 ? null : FormatIssues(collector.Issues);
    }

    /// <summary>
    /// Saves the active document to the specified location.
    /// </summary>
    /// <param name="location">The storage location of the document, such as a file path.</param>
    /// <returns>
    /// A report of the issues found while saving, one line per issue, or <see langword="null"/> if there are none.
    /// </returns>
    /// <remarks>
    /// Exceptions from the document repository are not caught.
    /// </remarks>
    public string? Save(string location)
    {
        IssueCollector collector = new();
        _documentRepository.Save(location, ActiveDocument.LoadedDocument, collector);

        return collector.Issues.Count == 0 ? null : FormatIssues(collector.Issues);
    }

    private static string FormatIssues(IReadOnlyList<Issue> issues)
    {
        IEnumerable<string> lines = issues.Select(issue => issue.Path is null
            ? $"[{issue.Kind}] {issue.Code}: {issue.Message}"
            : $"[{issue.Kind}] {issue.Code}: {issue.Message} ({issue.Path})");

        return string.Join(Environment.NewLine, lines);
    }

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    private void SetProperty<T>(ref T storage, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(storage, value)) return;
        storage = value;
        OnPropertyChanged(name);
    }
}
