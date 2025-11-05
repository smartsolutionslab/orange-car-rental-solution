namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

/// <summary>
///     Provides fluent validation methods for ensuring parameter and value constraints.
/// </summary>
/// <remarks>
///     Usage example:
///     <code>
/// Ensure.That(value, nameof(value)).IsNotNull().AndIsLongerThan(4).AndHasMaxLength(40);
/// Ensure.That(age, nameof(age)).IsGreaterThan(0).AndIsLessThan(150);
/// Ensure.That(items, nameof(items)).IsNotNull().AndIsNotEmpty();
/// </code>
/// </remarks>
public static class Ensure
{
    /// <summary>
    ///     Creates an ensurer for a value of type T.
    /// </summary>
    /// <typeparam name="T">The type of the value to ensure.</typeparam>
    /// <param name="value">The value to validate.</param>
    /// <param name="parameterName">The name of the parameter (for error messages).</param>
    /// <returns>An Ensurer instance for fluent validation.</returns>
    public static Ensurer<T> That<T>(T value, string parameterName) => new(value, parameterName);
}

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
