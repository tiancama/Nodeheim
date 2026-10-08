namespace Nodeheim.Persistence.Documents;

public static class DocumentMapper
{
    private const string FormatKey = "nodeheimFormat";
    private const string SupportedFormat = "0.1";
    private const string StateIdKey = "stateId";
    private const string SectionsKey = "sections";
    private const string ContentVersionKey = "contentVersion";
    private const string ContentKey = "content";

    public static DocumentResult Read(DocumentValue root)
    {
        IReadOnlyDictionary<string, DocumentValue> entries = ((CompoundValue)root).Entries;
        Guid stateId = Guid.Parse(((TextValue)entries[StateIdKey]).Value);

        var sections = new Dictionary<string, DocumentSection>(StringComparer.Ordinal);
        foreach ((string name, DocumentValue value) in ((CompoundValue)entries[SectionsKey]).Entries)
        {
            IReadOnlyDictionary<string, DocumentValue> section = ((CompoundValue)value).Entries;
            sections.Add(name, new DocumentSection(((TextValue)section[ContentVersionKey]).Value, section[ContentKey]));
        }

        return new DocumentSuccess(new Document(stateId, sections));
    }
}
