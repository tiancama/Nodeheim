namespace Nodeheim.Issues;

/// <summary>
/// Represents an issue that a stage reports, together with its location.
/// </summary>
public sealed record Issue
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Issue"/> record.
    /// </summary>
    /// <param name="definition">The definition that supplies the code and the kind.</param>
    /// <param name="message">A description of the issue for developers and logs.</param>
    /// <param name="path">
    /// The location of the affected value, following <see href="https://www.rfc-editor.org/rfc/rfc6901">JSON
    /// Pointer</see>, or <see langword="null"/> if the issue has no location.
    /// </param>
    public Issue(IssueDefinition definition, string message, string? path)
    {
        Code = definition.Code;
        Kind = definition.Kind;
        Message = message;
        Path = path;
    }

    /// <summary>
    /// Gets the code that identifies the issue, in the form <c>owner.name</c>, such as
    /// <c>core.graph.duplicateNode</c>.
    /// </summary>
    public string Code { get; }

    /// <summary>
    /// Gets the kind of the issue.
    /// </summary>
    public IssueKind Kind { get; }

    /// <summary>
    /// Gets a description of the issue for developers and logs.
    /// </summary>
    public string Message { get; }

    /// <summary>
    /// Gets the location of the affected value, following <see href="https://www.rfc-editor.org/rfc/rfc6901">JSON
    /// Pointer</see>, or <see langword="null"/> if the issue has no location.
    /// </summary>
    /// <remarks>
    /// The location refers to the value, not to the text of the input, such as
    /// <c>/sections/core.graph/content/nodes/3</c>. The exact rules are defined in <c>Specification.md</c>.
    /// </remarks>
    public string? Path { get; }
}
