namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;

/// <summary>
///     Exception thrown when an entity is not found in the repository.
///     This exception is used instead of returning null to enforce explicit error handling.
/// </summary>
public class EntityNotFoundException : Exception
{
    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
    /// </summary>
    /// <param name="entityType">The type of the entity that was not found.</param>
    /// <param name="identifier">The identifier that was used to search for the entity.</param>
    public EntityNotFoundException(Type entityType, object identifier)
        : base($"{entityType.Name} with identifier '{identifier}' was not found.")
    {
        EntityType = entityType;
        Identifier = identifier;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
    /// </summary>
    /// <param name="entityType">The type of the entity that was not found.</param>
    /// <param name="identifier">The identifier that was used to search for the entity.</param>
    /// <param name="message">Custom error message.</param>
    public EntityNotFoundException(Type entityType, object identifier, string message)
        : base(message)
    {
        EntityType = entityType;
        Identifier = identifier;
    }

    /// <summary>
    ///     Initializes a new instance of the <see cref="EntityNotFoundException"/> class.
    /// </summary>
    /// <param name="message">The error message.</param>
    public EntityNotFoundException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///     Gets the type of the entity that was not found.
    /// </summary>
    public Type? EntityType { get; }

    /// <summary>
    ///     Gets the identifier that was used to search for the entity.
    /// </summary>
    public object? Identifier { get; }
}
