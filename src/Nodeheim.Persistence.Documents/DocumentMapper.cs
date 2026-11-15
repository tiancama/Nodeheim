using Nodeheim.Issues;

namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Provides the mapping from the root entry of a document to a <see cref="Document"/>.
/// </summary>
public static class DocumentMapper
{
    /// <summary>
    /// Represents the name of the root entry of every document.
    /// </summary>
    /// <remarks>
    /// Format contract: An adapter whose syntax carries no name at the root uses this name.
    /// </remarks>
    public const string RootName = "nodeheim";

    private const string FormatName = "nodeheimFormat";
    private const string SupportedFormat = "0.2";
    private const string StateIdName = "stateId";
    private const string SectionsName = "sections";
    private const string ContentVersionName = "contentVersion";
    private const string ContentName = "content";

    /// <summary>
    /// Reads a document from the root entry of the input.
    /// </summary>
    /// <param name="root">The root entry of the input.</param>
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
    /// Format contract: The root entry is named <c>nodeheim</c> and has exactly one child each named
    /// <c>nodeheimFormat</c>, <c>stateId</c>, and <c>sections</c>, and no other children. The text of
    /// <c>nodeheimFormat</c> is <c>0.2</c>; it is checked right after the name of the root entry, because another
    /// format version may structure the document differently. <c>stateId</c> is a leaf with any text.
    /// <c>sections</c> has at least one child; the name of each child is its section type, and each section type
    /// occurs only once. Each section has exactly one child each named <c>contentVersion</c>, a leaf, and
    /// <c>content</c>, an entry of any form, and no other children.
    /// </para>
    /// </remarks>
    public static Document? Read(DocumentEntry root, IIssueCollector issues)
    {
        ArgumentNullException.ThrowIfNull(root);
        ArgumentNullException.ThrowIfNull(issues);

        if (root.Name != RootName)
            return Reject(issues, FormatIssues.NotNodeheimDocument,
                $"Not a Nodeheim document: the root entry is not named '{RootName}'.", "");

        DocumentEntry? format = DocumentEntryReader.GetChild(root, FormatName);
        if (format is null)
            return Reject(issues, FormatIssues.NotNodeheimDocument,
                "Not a Nodeheim document: 'nodeheimFormat' is missing or occurs more than once.", "");

        string? formatText = DocumentEntryReader.GetText(format);
        if (formatText is null)
            return Reject(issues, FormatIssues.InvalidStructure,
                "Invalid structure: 'nodeheimFormat' is not a text.", "/nodeheimFormat");

        if (formatText != SupportedFormat)
            return Reject(issues, FormatIssues.UnsupportedFormatVersion,
                "Unsupported format version.", "/nodeheimFormat");

        DocumentEntry? stateIdEntry = DocumentEntryReader.GetChild(root, StateIdName);
        string? stateId = stateIdEntry is null ? null : DocumentEntryReader.GetText(stateIdEntry);
        if (stateId is null)
            return Reject(issues, FormatIssues.InvalidStructure,
                "Invalid structure: 'stateId' is missing, occurs more than once, or is not a text.", "/stateId");

        DocumentEntry? sectionsEntry = DocumentEntryReader.GetChild(root, SectionsName);
        if (sectionsEntry is null)
            return Reject(issues, FormatIssues.InvalidStructure,
                "Invalid structure: 'sections' is missing or occurs more than once.", "/sections");

        foreach (DocumentEntry child in root.Children)
        {
            if (child.Name is not (FormatName or StateIdName or SectionsName))
                return Reject(issues, FormatIssues.InvalidStructure,
                    $"Invalid structure: unexpected entry '{child.Name}'.", $"/{child.Name}");
        }

        if (sectionsEntry.Children.Count == 0)
            return Reject(issues, FormatIssues.InvalidStructure,
                "Invalid structure: 'sections' has no sections.", "/sections");

        Dictionary<string, DocumentSection> sections = new(StringComparer.Ordinal);

        foreach (DocumentEntry sectionEntry in sectionsEntry.Children)
        {
            string name = sectionEntry.Name;
            string path = $"/sections/{name}";

            DocumentEntry? contentVersionEntry = DocumentEntryReader.GetChild(sectionEntry, ContentVersionName);
            string? contentVersion = contentVersionEntry is null
                ? null
                : DocumentEntryReader.GetText(contentVersionEntry);
            if (contentVersion is null)
                return Reject(issues, FormatIssues.InvalidStructure,
                    $"Invalid structure: 'contentVersion' in section '{name}' is missing, occurs more than once, " +
                    "or is not a text.", path);

            DocumentEntry? content = DocumentEntryReader.GetChild(sectionEntry, ContentName);
            if (content is null)
                return Reject(issues, FormatIssues.InvalidStructure,
                    $"Invalid structure: 'content' in section '{name}' is missing or occurs more than once.", path);

            foreach (DocumentEntry child in sectionEntry.Children)
            {
                if (child.Name is not (ContentVersionName or ContentName))
                    return Reject(issues, FormatIssues.InvalidStructure,
                        $"Invalid structure: unexpected entry '{child.Name}' in section '{name}'.",
                        $"{path}/{child.Name}");
            }

            if (!sections.TryAdd(name, new DocumentSection(contentVersion, content)))
                return Reject(issues, FormatIssues.InvalidStructure,
                    $"Invalid structure: section '{name}' occurs more than once.", path);
        }

        return new Document(stateId, sections);
    }

    private static Document? Reject(IIssueCollector issues, IssueDefinition definition, string message, string path)
    {
        issues.Add(new Issue(definition, message, path));
        return null;
    }
}
