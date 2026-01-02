namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;

/// <summary>
///     Exception thrown when input validation fails.
///     Maps to HTTP 400 Bad Request.
/// </summary>
public class ValidationException : Exception
{
    public ValidationException(string message)
        : base(message)
    {
    }

    public ValidationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public ValidationException(string field, string message)
        : base(message)
    {
        Field = field;
    }

    /// <summary>
    ///     Gets the field that failed validation, if applicable.
    /// </summary>
    public string? Field { get; }
}
