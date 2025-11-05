using System.Diagnostics.CodeAnalysis;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

/// <summary>
/// Provides fluent validation extension methods for object values.
/// </summary>
public static class EnsureObjectExtensions
{
    /// <summary>
    /// Ensures the value is not null.
    /// </summary>
    public static Ensurer<T> IsNotNull<T>([NotNull] this Ensurer<T> ensurer) where T : class
    {
        if (ensurer.Value is null)
            throw new ArgumentNullException(ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the nullable value is not null.
    /// </summary>
    public static Ensurer<T?> IsNotNull<T>([NotNull] this Ensurer<T?> ensurer) where T : struct
    {
        if (!ensurer.Value.HasValue)
            throw new ArgumentNullException(ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the value is null.
    /// </summary>
    public static Ensurer<T> IsNull<T>(this Ensurer<T> ensurer) where T : class
    {
        if (ensurer.Value is not null)
            throw new ArgumentException($"Value must be null.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the value equals the specified value.
    /// </summary>
    public static Ensurer<T> AndEquals<T>(this Ensurer<T> ensurer, T expected) where T : IEquatable<T>
    {
        if (ensurer.Value is null || !ensurer.Value.Equals(expected))
            throw new ArgumentException($"Value must equal '{expected}'.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the value does not equal the specified value.
    /// </summary>
    public static Ensurer<T> AndNotEquals<T>(this Ensurer<T> ensurer, T notExpected) where T : IEquatable<T>
    {
        if (ensurer.Value is not null && ensurer.Value.Equals(notExpected))
            throw new ArgumentException($"Value must not equal '{notExpected}'.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the value is of the specified type.
    /// </summary>
    public static Ensurer<T> AndIsOfType<T, TExpected>(this Ensurer<T> ensurer)
    {
        if (ensurer.Value is not TExpected)
            throw new ArgumentException($"Value must be of type '{typeof(TExpected).Name}'.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the value satisfies the specified condition.
    /// </summary>
    public static Ensurer<T> AndSatisfies<T>(this Ensurer<T> ensurer, Func<T, bool> predicate, string? errorMessage = null)
    {
        if (!predicate(ensurer.Value))
        {
            var message = errorMessage ?? "Value does not satisfy the required condition.";
            throw new ArgumentException(message, ensurer.ParameterName);
        }

        return ensurer;
    }
}
