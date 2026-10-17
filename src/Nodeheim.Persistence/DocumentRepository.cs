using Nodeheim.Domain;
using Nodeheim.Issues;
using Nodeheim.Persistence.Documents;

namespace Nodeheim.Persistence;

public sealed class DocumentRepository
{
    private readonly IDocumentStorage _storage;

    public DocumentRepository(IDocumentStorage storage)
    {
        _storage = storage;
    }

    /// <returns>
    /// The loaded graph, or <see langword="null"/> if the document or the content version of the graph is rejected,
    /// or the document has no <c>core.graph</c> section.
    /// </returns>
    /// <remarks>
    /// Sections of an unknown section type are skipped and reported as discrepancies.
    /// A return value of <see langword="null"/> alone does not indicate a failure, because no section is required.
    /// Only a rejection in <paramref name="issues"/> does. A graph is never returned together with a rejection.
    /// The structure of the document and the content of the graph are not yet checked; malformed input causes
    /// exceptions. Errors of the environment, such as I/O errors, are not yet reported as issues and also cause
    /// exceptions.
    /// </remarks>
    public Graph? Load(IIssueCollector issues)
    {
        DocumentValue? root = _storage.Load(issues);
        if (root is null) return null;

        Document document = DocumentMapper.Read(root);

        foreach (string key in document.Sections.Keys)
        {
            if (key != GraphMapper.SectionType)
                issues.Add(new Issue(FormatIssues.UnknownSection,
                    $"Unknown section '{key}' is skipped.", $"/sections/{key}"));
        }

        if (!document.Sections.TryGetValue(GraphMapper.SectionType, out DocumentSection? section))
            return null;

        return GraphMapper.Read(section, issues);
    }
}
