namespace Nodeheim.Persistence;

/// <summary>
/// Represents the models of the owners of a document, at most one per model type.
/// </summary>
/// <remarks>
/// Model types are compared exactly, not through inheritance.
/// </remarks>
public sealed class OwnerModelCollection
{
    private readonly Dictionary<Type, object> _models = new();

    /// <summary>
    /// Gets the model of the specified type.
    /// </summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    /// <returns>The model, or <see langword="null"/> if the collection holds no model of type <typeparamref name="T"/>.</returns>
    public T? Get<T>()
        where T : class =>
        _models.TryGetValue(typeof(T), out object? model) ? (T)model : null;

    /// <summary>
    /// Sets the model of the specified type, replacing any model of that type.
    /// </summary>
    /// <typeparam name="T">The type of the model.</typeparam>
    /// <param name="model">The model.</param>
    /// <exception cref="ArgumentNullException"><paramref name="model"/> is <see langword="null"/>.</exception>
    public void Set<T>(T model)
        where T : class
    {
        ArgumentNullException.ThrowIfNull(model);

        _models[typeof(T)] = model;
    }

    internal object? Get(Type modelType) =>
        _models.GetValueOrDefault(modelType);

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
