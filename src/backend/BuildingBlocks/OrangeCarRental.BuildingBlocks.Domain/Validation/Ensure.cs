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
