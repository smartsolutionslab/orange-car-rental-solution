namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

/// <summary>
/// Provides fluent validation extension methods for numeric values.
/// </summary>
public static class EnsureNumericExtensions
{
    /// <summary>
    /// Ensures the value is greater than the specified minimum.
    /// </summary>
    public static Ensurer<T> IsGreaterThan<T>(this Ensurer<T> ensurer, T minimum) where T : IComparable<T>
    {
        if (ensurer.Value.CompareTo(minimum) <= 0)
            throw new ArgumentException($"Value must be greater than {minimum} but is {ensurer.Value}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the value is greater than or equal to the specified minimum.
    /// </summary>
    public static Ensurer<T> IsGreaterThanOrEqual<T>(this Ensurer<T> ensurer, T minimum) where T : IComparable<T>
    {
        if (ensurer.Value.CompareTo(minimum) < 0)
            throw new ArgumentException($"Value must be greater than or equal to {minimum} but is {ensurer.Value}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the value is less than the specified maximum.
    /// </summary>
    public static Ensurer<T> IsLessThan<T>(this Ensurer<T> ensurer, T maximum) where T : IComparable<T>
    {
        if (ensurer.Value.CompareTo(maximum) >= 0)
            throw new ArgumentException($"Value must be less than {maximum} but is {ensurer.Value}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the value is less than or equal to the specified maximum.
    /// </summary>
    public static Ensurer<T> IsLessThanOrEqual<T>(this Ensurer<T> ensurer, T maximum) where T : IComparable<T>
    {
        if (ensurer.Value.CompareTo(maximum) > 0)
            throw new ArgumentException($"Value must be less than or equal to {maximum} but is {ensurer.Value}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the value is within the specified range (inclusive).
    /// </summary>
    public static Ensurer<T> AndIsBetween<T>(this Ensurer<T> ensurer, T minimum, T maximum) where T : IComparable<T>
    {
        if (ensurer.Value.CompareTo(minimum) < 0 || ensurer.Value.CompareTo(maximum) > 0)
            throw new ArgumentException($"Value must be between {minimum} and {maximum} but is {ensurer.Value}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the value is positive (greater than zero).
    /// </summary>
    public static Ensurer<T> IsPositive<T>(this Ensurer<T> ensurer) where T : IComparable<T>
    {
        dynamic value = ensurer.Value;
        dynamic zero = default(T)!;

        if (value <= zero)
            throw new ArgumentException($"Value must be positive but is {ensurer.Value}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the value is negative (less than zero).
    /// </summary>
    public static Ensurer<T> IsNegative<T>(this Ensurer<T> ensurer) where T : IComparable<T>
    {
        dynamic value = ensurer.Value;
        dynamic zero = default(T)!;

        if (value >= zero)
            throw new ArgumentException($"Value must be negative but is {ensurer.Value}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the value is zero.
    /// </summary>
    public static Ensurer<T> IsZero<T>(this Ensurer<T> ensurer) where T : IComparable<T>
    {
        dynamic value = ensurer.Value;
        dynamic zero = default(T)!;

        if (value != zero)
            throw new ArgumentException($"Value must be zero but is {ensurer.Value}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the value is not zero.
    /// </summary>
    public static Ensurer<T> IsNotZero<T>(this Ensurer<T> ensurer) where T : IComparable<T>
    {
        dynamic value = ensurer.Value;
        dynamic zero = default(T)!;

        if (value == zero)
            throw new ArgumentException($"Value must not be zero.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Continues the fluent chain with 'And' for readability.
    /// </summary>
    public static Ensurer<T> And<T>(this Ensurer<T> ensurer) where T : IComparable<T>
    {
        return ensurer;
    }

    /// <summary>
    /// Ensures the value is less than the specified maximum (alias for fluent readability).
    /// </summary>
    public static Ensurer<T> AndIsLessThan<T>(this Ensurer<T> ensurer, T maximum) where T : IComparable<T>
    {
        return ensurer.IsLessThan(maximum);
    }

    /// <summary>
    /// Ensures the value is greater than the specified minimum (alias for fluent readability).
    /// </summary>
    public static Ensurer<T> AndIsGreaterThan<T>(this Ensurer<T> ensurer, T minimum) where T : IComparable<T>
    {
        return ensurer.IsGreaterThan(minimum);
    }
}
