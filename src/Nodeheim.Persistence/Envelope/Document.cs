using System.Collections.ObjectModel;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

/// <summary>
/// Represents a document with its state id and its sections.
/// </summary>
/// <remarks>
/// Format contract: Sections are keyed by name, such as <c>engine.graph</c>. Names are compared ordinally.
/// </remarks>
internal sealed class Document
{
    public Document(string stateId, IReadOnlyDictionary<string, DocumentSection> sections)
    {
        ArgumentNullException.ThrowIfNull(stateId);
        ArgumentNullException.ThrowIfNull(sections);

        StateId = stateId;
        Sections = new ReadOnlyDictionary<string, DocumentSection>(
            new Dictionary<string, DocumentSection>(sections, StringComparer.Ordinal));
    }

    public string StateId { get; }

    public IReadOnlyDictionary<string, DocumentSection> Sections { get; }
}
