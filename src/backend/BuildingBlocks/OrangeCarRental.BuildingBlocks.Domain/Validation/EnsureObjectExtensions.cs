using System.Diagnostics.CodeAnalysis;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

/// <summary>
///     Provides fluent validation extension methods for object values.
/// </summary>
public static class EnsureObjectExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for Ensurer&lt;T&gt; where T : class.
    /// </summary>
    extension<T>(Ensurer<T> ensurer) where T : class
    {
        /// <summary>
        ///     Ensures the value is not null.
        /// </summary>
        [return: NotNull]
        public Ensurer<T> IsNotNull()
        {
            if (ensurer.Value is null)
                throw new ArgumentNullException(ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the value is null.
        /// </summary>
        public Ensurer<T> IsNull()
        {
            if (ensurer.Value is not null)
                throw new ArgumentException("Value must be null.", ensurer.ParameterName);

            return ensurer;
        }
    }

    /// <summary>
    ///     C# 14 Extension Members for Ensurer&lt;T?&gt; where T : struct.
    /// </summary>
    extension<T>(Ensurer<T?> ensurer) where T : struct
    {
        /// <summary>
        ///     Ensures the nullable value is not null.
        /// </summary>
        [return: NotNull]
        public Ensurer<T?> IsNotNull()
        {
            if (!ensurer.Value.HasValue)
                throw new ArgumentNullException(ensurer.ParameterName);

            return ensurer;
        }
    }

    /// <summary>
    ///     C# 14 Extension Members for Ensurer&lt;T&gt; where T : IEquatable&lt;T&gt;.
    /// </summary>
    extension<T>(Ensurer<T> ensurer) where T : IEquatable<T>
    {
        /// <summary>
        ///     Ensures the value equals the specified value.
        /// </summary>
        public Ensurer<T> AndEquals(T expected)
        {
            if (ensurer.Value is null || !ensurer.Value.Equals(expected))
                throw new ArgumentException($"Value must equal '{expected}'.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the value does not equal the specified value.
        /// </summary>
        public Ensurer<T> AndNotEquals(T notExpected)
        {
            if (ensurer.Value is not null && ensurer.Value.Equals(notExpected))
                throw new ArgumentException($"Value must not equal '{notExpected}'.", ensurer.ParameterName);

            return ensurer;
        }
    }

    /// <summary>
    ///     C# 14 Extension Members for generic Ensurer&lt;T&gt;.
    /// </summary>
    extension<T>(Ensurer<T> ensurer)
    {
        /// <summary>
        ///     Ensures the value is of the specified type.
        /// </summary>
        public Ensurer<T> AndIsOfType<TExpected>()
        {
            if (ensurer.Value is not TExpected)
                throw new ArgumentException($"Value must be of type '{nameof(TExpected)}'.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the value satisfies the specified condition.
        /// </summary>
        public Ensurer<T> AndSatisfies(Func<T, bool> predicate, string? errorMessage = null)
        {
            if (!predicate(ensurer.Value))
            {
                var message = errorMessage ?? "Value does not satisfy the required condition.";
                throw new ArgumentException(message, ensurer.ParameterName);
            }

            return ensurer;
        }
    }
}
