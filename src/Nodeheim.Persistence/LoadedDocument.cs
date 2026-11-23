namespace Nodeheim.Persistence;

/// <summary>
/// Represents a document as the application holds it between loading and saving.
/// </summary>
/// <remarks>
/// A new document starts with an empty collection of models. The application keeps the models it does not know
/// and passes the document back unchanged when saving.
/// </remarks>
public sealed class LoadedDocument
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LoadedDocument"/> class with no models.
    /// </summary>
    public LoadedDocument()
    {
    }

    /// <summary>
    /// Gets the models of the owners of the document.
    /// </summary>
    public OwnerModelCollection Models { get; } = new();
}
