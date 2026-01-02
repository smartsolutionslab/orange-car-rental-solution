namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;

/// <summary>
///     Exception thrown when an operation conflicts with the current state.
///     Maps to HTTP 409 Conflict.
/// </summary>
public class ConflictException : Exception
{
    public ConflictException(string message)
        : base(message)
    {
    }

    public ConflictException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ConflictException(Type entityType, object identifier, string message)
        : base(message)
    {
        EntityType = entityType;
        Identifier = identifier;
    }

    /// <summary>
    ///     Gets the type of the entity involved in the conflict.
    /// </summary>
    public Type? EntityType { get; }

    /// <summary>
    ///     Gets the identifier of the entity involved in the conflict.
    /// </summary>
    public object? Identifier { get; }
}
