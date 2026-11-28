namespace Nodeheim.Persistence;

/// <summary>
/// Represents a document as the application holds it between loading and saving.
/// </summary>
/// <remarks>
/// The models are live instances. Changes are made to the models themselves and are written when the document is
/// saved. Replacing a model is not intended.
/// </remarks>
public sealed class LoadedDocument
{
    private readonly Dictionary<Type, object> _models = new();

    internal LoadedDocument()
    {
    }

    /// <summary>
    /// Gets the model of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    /// <returns>The model.</returns>
    /// <exception cref="InvalidOperationException">
    /// No section mapper with a model of type <typeparamref name="T"/> is registered.
    /// </exception>
    /// <remarks>
    /// Model types are compared exactly, not through inheritance.
    /// </remarks>
    public T Get<T>()
        where T : class
    {
        if (!_models.TryGetValue(typeof(T), out object? model))
            throw new InvalidOperationException($"No model of type '{typeof(T)}' is registered.");

        return (T)model;
    }

    internal object Get(Type modelType) => _models[modelType];

    internal void Set(Type modelType, object model)
    {
        ArgumentNullException.ThrowIfNull(modelType);
        ArgumentNullException.ThrowIfNull(model);

        if (!modelType.IsInstanceOfType(model))
            throw new ArgumentException(
                $"The model is of type '{model.GetType()}', not '{modelType}'.", nameof(model));

        _models[modelType] = model;
    }
}
