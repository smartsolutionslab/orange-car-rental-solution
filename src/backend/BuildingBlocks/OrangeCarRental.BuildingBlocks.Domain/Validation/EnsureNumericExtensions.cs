namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

/// <summary>
///     Provides fluent validation extension methods for numeric values.
/// </summary>
public static class EnsureNumericExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for Ensurer&lt;T&gt; where T : IComparable&lt;T&gt;.
    /// </summary>
    extension<T>(Ensurer<T> ensurer) where T : IComparable<T>
    {
        /// <summary>
        ///     Ensures the value is greater than the specified minimum.
        /// </summary>
        public Ensurer<T> IsGreaterThan(T minimum)
        {
            if (ensurer.Value.CompareTo(minimum) <= 0)
                throw new ArgumentException($"Value must be greater than {minimum} but is {ensurer.Value}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the value is greater than or equal to the specified minimum.
        /// </summary>
        public Ensurer<T> IsGreaterThanOrEqual(T minimum)
        {
            if (ensurer.Value.CompareTo(minimum) < 0)
                throw new ArgumentException($"Value must be greater than or equal to {minimum} but is {ensurer.Value}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the value is less than the specified maximum.
        /// </summary>
        public Ensurer<T> IsLessThan(T maximum)
        {
            if (ensurer.Value.CompareTo(maximum) >= 0)
                throw new ArgumentException($"Value must be less than {maximum} but is {ensurer.Value}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the value is less than or equal to the specified maximum.
        /// </summary>
        public Ensurer<T> IsLessThanOrEqual(T maximum)
        {
            if (ensurer.Value.CompareTo(maximum) > 0)
                throw new ArgumentException($"Value must be less than or equal to {maximum} but is {ensurer.Value}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the value is within the specified range (inclusive).
        /// </summary>
        public Ensurer<T> AndIsBetween(T minimum, T maximum)
        {
            if (ensurer.Value.CompareTo(minimum) < 0 || ensurer.Value.CompareTo(maximum) > 0)
                throw new ArgumentException($"Value must be between {minimum} and {maximum} but is {ensurer.Value}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the value is positive (greater than zero).
        /// </summary>
        public Ensurer<T> IsPositive()
        {
            dynamic value = ensurer.Value;
            dynamic zero = default(T)!;

            if (value <= zero)
                throw new ArgumentException($"Value must be positive but is {ensurer.Value}.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the value is negative (less than zero).
        /// </summary>
        public Ensurer<T> IsNegative()
        {
            dynamic value = ensurer.Value;
            dynamic zero = default(T)!;

            if (value >= zero)
                throw new ArgumentException($"Value must be negative but is {ensurer.Value}.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the value is not negative (greater than or equal to zero).
        /// </summary>
        public Ensurer<T> IsNotNegative()
        {
            dynamic value = ensurer.Value;
            dynamic zero = default(T)!;

            if (value < zero)
                throw new ArgumentException($"Value cannot be negative but is {ensurer.Value}.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the value is zero.
        /// </summary>
        public Ensurer<T> IsZero()
        {
            dynamic value = ensurer.Value;
            dynamic zero = default(T)!;

            if (value != zero)
                throw new ArgumentException($"Value must be zero but is {ensurer.Value}.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the value is not zero.
        /// </summary>
        public Ensurer<T> IsNotZero()
        {
            dynamic value = ensurer.Value;
            dynamic zero = default(T)!;

            if (value == zero)
                throw new ArgumentException("Value must not be zero.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Continues the fluent chain with 'And' for readability.
        /// </summary>
        public Ensurer<T> And() => ensurer;

        /// <summary>
        ///     Ensures the value is less than the specified maximum (alias for fluent readability).
        /// </summary>
        public Ensurer<T> AndIsLessThan(T maximum) => ensurer.IsLessThan(maximum);

        /// <summary>
        ///     Ensures the value is greater than the specified minimum (alias for fluent readability).
        /// </summary>
        public Ensurer<T> AndIsGreaterThan(T minimum) => ensurer.IsGreaterThan(minimum);

        /// <summary>
        ///     Ensures the value is less than or equal to the specified maximum (alias for fluent readability).
        /// </summary>
        public Ensurer<T> AndIsLessThanOrEqual(T maximum) => ensurer.IsLessThanOrEqual(maximum);

        /// <summary>
        ///     Ensures the value is greater than or equal to the specified minimum (alias for fluent readability).
        /// </summary>
        public Ensurer<T> AndIsGreaterThanOrEqual(T minimum) => ensurer.IsGreaterThanOrEqual(minimum);
    }
}
