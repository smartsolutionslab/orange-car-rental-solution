namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

/// <summary>
///     Represents a fluent validation context for a value of type T.
/// </summary>
/// <typeparam name="T">The type of the value being validated.</typeparam>
public sealed class Ensurer<T>
{
    internal Ensurer(T value, string parameterName)
    {
        Value = value;
        ParameterName = parameterName;
    }

    /// <summary>
    ///     Gets the value being validated.
    /// </summary>
    public T Value { get; }

    /// <summary>
    ///     Gets the parameter name used in error messages.
    /// </summary>
    public string ParameterName { get; }

    /// <summary>
    ///     Throws an ArgumentException with the specified message.
    /// </summary>
    public Ensurer<T> Throw(string message) => throw new ArgumentException(message, ParameterName);

    /// <summary>
    ///     Throws an ArgumentException if the condition is false.
    /// </summary>
    public Ensurer<T> ThrowIf(bool condition, string message)
    {
        if (condition)
            throw new ArgumentException(message, ParameterName);

        return this;
    }
}
