using System.Diagnostics;
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

    /// <summary>
    /// Loads the graph from the document.
    /// </summary>
    /// <param name="issues">The collector that receives the issues found while loading.</param>
    /// <returns>
    /// The loaded graph, or <see langword="null"/> if the content version of the graph is rejected or the document
    /// has no <c>core.graph</c> section.
    /// </returns>
    /// <exception cref="InvalidOperationException">
    /// The adapter or the document mapper rejects the document.
    /// </exception>
    /// <remarks>
    /// Sections of an unknown section type are skipped and reported as discrepancies.
    /// A return value of <see langword="null"/> alone does not indicate a failure, because no section is required.
    /// Only a rejection in <paramref name="issues"/> does. A graph is never returned together with a rejection.
    /// The content of the graph is not yet checked; malformed content causes exceptions.
    /// </remarks>
    public Graph? Load(IIssueCollector issues)
    {
        DocumentValue root = _storage.Load() switch
        {
            DocumentValueSuccess success => success.Value,
            DocumentValueFailure failure => throw new InvalidOperationException(failure.Rejection.Message),
            _ => throw new UnreachableException()
        };

        Document document = DocumentMapper.Read(root) switch
        {
            DocumentSuccess success => success.Document,
            DocumentFailure failure => throw new InvalidOperationException(failure.Rejection.Message),
            _ => throw new UnreachableException()
        };

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
