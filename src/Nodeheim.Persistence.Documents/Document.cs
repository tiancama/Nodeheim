using System.Collections.ObjectModel;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents a document with its state id and its sections.
/// </summary>
/// <remarks>
/// Format contract: Sections are keyed by name, such as <c>core.graph</c>. Names are compared ordinally.
/// </remarks>
public sealed class Document
{
    public Document(Guid stateId, IReadOnlyDictionary<string, DocumentSection> sections)
    {
        ArgumentNullException.ThrowIfNull(sections);

        StateId = stateId;
        Sections = new ReadOnlyDictionary<string, DocumentSection>(
            new Dictionary<string, DocumentSection>(sections, StringComparer.Ordinal));
    }

    public Guid StateId { get; }

    public IReadOnlyDictionary<string, DocumentSection> Sections { get; }
}
