namespace Nodeheim.Persistence.Documents;

/// <summary>
/// Represents the contribution of an owner to a document that is being written.
/// </summary>
/// <param name="SectionType">The section type.</param>
/// <param name="ContentVersion">The version of the content.</param>
/// <param name="WriteContent">
/// The function that writes the content as a single value to the given writer. The document is written in
/// more than one pass, so each invocation must write the same calls.
/// </param>
public sealed record DocumentSectionContribution(
    string SectionType,
    string ContentVersion,
    Action<IDocumentWriter> WriteContent);
