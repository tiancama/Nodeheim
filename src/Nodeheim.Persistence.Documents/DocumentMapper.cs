using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Provides the mapping from the root value of a document to a <see cref="Document"/>.
/// </summary>
public static class DocumentMapper
{
    private const string FormatKey = "nodeheimFormat";
    private const string SupportedFormat = "0.1";
    private const string StateIdKey = "stateId";
    private const string SectionsKey = "sections";
    private const string ContentVersionKey = "contentVersion";
    private const string ContentKey = "content";

    /// <summary>
    /// Represents the length of a GUID in the format <c>00000000-0000-0000-0000-000000000000</c>.
    /// </summary>
    /// <remarks>
    /// <see cref="Guid.TryParseExact(string, string, out Guid)"/> ignores leading and trailing white space; the
    /// length check rejects it.
    /// </remarks>
    private const int GuidLength = 36;

    /// <summary>
    /// Reads a document from the root value of the input.
    /// </summary>
    /// <param name="root">The root value of the input.</param>
    /// <param name="issues">
    /// The collector that receives the rejection if <paramref name="root"/> is not a valid document.
    /// </param>
    /// <returns>
    /// The document, or <see langword="null"/> if <paramref name="root"/> is not a valid document.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="root"/> or <paramref name="issues"/> is <see langword="null"/>.
    /// </exception>
    /// <remarks>
    /// <para>
    /// Reading stops at the first rejection, so <see langword="null"/> comes with exactly one rejection in
    /// <paramref name="issues"/>: <see cref="FormatIssues.NotNodeheimDocument"/>,
    /// <see cref="FormatIssues.UnsupportedFormatVersion"/>, or <see cref="FormatIssues.InvalidStructure"/>. A
    /// returned document comes without any issue. Section types, content versions, and the content of the sections
    /// are not checked; that is up to the caller.
    /// </para>
    /// <para>
    /// Format contract: The root value is a compound with exactly the entries <c>nodeheimFormat</c>,
    /// <c>stateId</c>, and <c>sections</c>. The value of <c>nodeheimFormat</c> is the text <c>0.1</c>. It is
    /// checked before everything else, because another format version may structure the document differently.
    /// The value of <c>stateId</c> is a text with a GUID in the format <c>00000000-0000-0000-0000-000000000000</c>;
    /// letter case is ignored. The value of <c>sections</c> is a compound with at least one entry; no section type
    /// is required. Each section is a compound with exactly the entries <c>contentVersion</c>, whose value is a
    /// text, and <c>content</c>, whose value can be of any value kind.
    /// </para>
    /// </remarks>
    public static Document? Read(DocumentValue root, IIssueCollector issues)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(issues);

        if (root is not CompoundValue compound)
            return Reject(issues, FormatIssues.NotNodeheimDocument,
                "Not a Nodeheim document: the root value is not a compound.", "");

        IReadOnlyDictionary<string, DocumentValue> entries = compound.Entries;

        if (!entries.TryGetValue(FormatKey, out DocumentValue? format))
            return Reject(issues, FormatIssues.NotNodeheimDocument,
                "Not a Nodeheim document: the key 'nodeheimFormat' is missing.", "");

        if (format is not TextValue formatText)
            return Reject(issues, FormatIssues.InvalidStructure,
                "Invalid structure: the value of 'nodeheimFormat' is not a text.", "/nodeheimFormat");

        if (formatText.Value != SupportedFormat)
            return Reject(issues, FormatIssues.UnsupportedFormatVersion,
                $"Unsupported format version '{formatText.Value}'.", "/nodeheimFormat");

        if (!entries.TryGetValue(StateIdKey, out DocumentValue? stateIdValue))
            return Reject(issues, FormatIssues.InvalidStructure,
                "Invalid structure: the key 'stateId' is missing.", "");

        if (stateIdValue is not TextValue stateIdText)
            return Reject(issues, FormatIssues.InvalidStructure,
                "Invalid structure: the value of 'stateId' is not a text.", "/stateId");

        if (stateIdText.Value.Length != GuidLength
            || !Guid.TryParseExact(stateIdText.Value, "D", out Guid stateId))
            return Reject(issues, FormatIssues.InvalidStructure,
                "Invalid structure: the value of 'stateId' is not a GUID in format D.", "/stateId");

        if (!entries.TryGetValue(SectionsKey, out DocumentValue? sectionsValue))
            return Reject(issues, FormatIssues.InvalidStructure,
                "Invalid structure: the key 'sections' is missing.", "");

        foreach (string key in entries.Keys)
        {
            if (key is not (FormatKey or StateIdKey or SectionsKey))
                return Reject(issues, FormatIssues.InvalidStructure,
                    $"Invalid structure: unexpected key '{key}'.", $"/{key}");
        }

        if (sectionsValue is not CompoundValue sectionsCompound)
            return Reject(issues, FormatIssues.InvalidStructure,
                "Invalid structure: the value of 'sections' is not a compound.", "/sections");

        if (sectionsCompound.Entries.Count == 0)
            return Reject(issues, FormatIssues.InvalidStructure,
                "Invalid structure: 'sections' has no entries.", "/sections");

        var sections = new Dictionary<string, DocumentSection>(StringComparer.Ordinal);
        foreach ((string name, DocumentValue value) in sectionsCompound.Entries)
        {
            if (value is not CompoundValue sectionCompound)
                return Reject(issues, FormatIssues.InvalidStructure,
                    $"Invalid structure: the value of section '{name}' is not a compound.",
                    $"/sections/{name}");

            IReadOnlyDictionary<string, DocumentValue> section = sectionCompound.Entries;

            if (!section.TryGetValue(ContentVersionKey, out DocumentValue? contentVersionValue))
                return Reject(issues, FormatIssues.InvalidStructure,
                    $"Invalid structure: the key 'contentVersion' is missing in section '{name}'.",
                    $"/sections/{name}");

            if (!section.TryGetValue(ContentKey, out DocumentValue? content))
                return Reject(issues, FormatIssues.InvalidStructure,
                    $"Invalid structure: the key 'content' is missing in section '{name}'.",
                    $"/sections/{name}");

            foreach (string key in section.Keys)
            {
                if (key is not (ContentVersionKey or ContentKey))
                    return Reject(issues, FormatIssues.InvalidStructure,
                        $"Invalid structure: unexpected key '{key}' in section '{name}'.",
                        $"/sections/{name}/{key}");
            }

            if (contentVersionValue is not TextValue contentVersionText)
                return Reject(issues, FormatIssues.InvalidStructure,
                    $"Invalid structure: the value of 'contentVersion' in section '{name}' is not a text.",
                    $"/sections/{name}/contentVersion");

            sections.Add(name, new DocumentSection(contentVersionText.Value, content));
        }

        return new Document(stateId, sections);
    }

    private static Document? Reject(IIssueCollector issues, IssueDefinition definition, string message, string path)
    {
        issues.Add(new Issue(definition, message, path));
        return null;
    }
}
