using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents;

public static class DocumentMapper
{
    private const string FormatKey = "nodeheimFormat";
    private const string SupportedFormat = "0.1";
    private const string StateIdKey = "stateId";
    private const string SectionsKey = "sections";
    private const string ContentVersionKey = "contentVersion";
    private const string ContentKey = "content";

    public static Document? Read(DocumentValue root, IIssueCollector issues)
    {
        if (root is not CompoundValue compound)
        {
            issues.Add(new Issue(FormatIssues.NotNodeheimDocument,
                "Not a Nodeheim document: the root value is not a compound.", ""));
            return null;
        }

        IReadOnlyDictionary<string, DocumentValue> entries = compound.Entries;

        if (!entries.TryGetValue(FormatKey, out DocumentValue? format))
        {
            issues.Add(new Issue(FormatIssues.NotNodeheimDocument,
                "Not a Nodeheim document: the key 'nodeheimFormat' is missing.", ""));
            return null;
        }

        if (format is not TextValue formatText)
        {
            issues.Add(new Issue(FormatIssues.InvalidStructure,
                "Invalid structure: the value of 'nodeheimFormat' is not a text.", "/nodeheimFormat"));
            return null;
        }

        if (formatText.Value != SupportedFormat)
        {
            issues.Add(new Issue(FormatIssues.UnsupportedFormatVersion,
                $"Unsupported format version '{formatText.Value}'.", "/nodeheimFormat"));
            return null;
        }

        Guid stateId = Guid.Parse(((TextValue)entries[StateIdKey]).Value);

        var sections = new Dictionary<string, DocumentSection>(StringComparer.Ordinal);
        foreach ((string name, DocumentValue value) in ((CompoundValue)entries[SectionsKey]).Entries)
        {
            IReadOnlyDictionary<string, DocumentValue> section = ((CompoundValue)value).Entries;
            sections.Add(name, new DocumentSection(((TextValue)section[ContentVersionKey]).Value, section[ContentKey]));
        }

        return new Document(stateId, sections);
    }
}
