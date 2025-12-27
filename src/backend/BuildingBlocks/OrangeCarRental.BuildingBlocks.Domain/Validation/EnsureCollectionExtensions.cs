using System.Diagnostics.CodeAnalysis;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

/// <summary>
///     Provides fluent validation extension methods for collection values.
/// </summary>
public static class EnsureCollectionExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for Ensurer&lt;T&gt; where T : class, IEnumerable&lt;object&gt;.
    /// </summary>
    extension<T>(Ensurer<T> ensurer) where T : class, IEnumerable<object>
    {
        /// <summary>
        ///     Ensures the collection is not null.
        /// </summary>
        [return: NotNull]
        public Ensurer<T> IsNotNull()
        {
            if (ensurer.Value is null)
                throw new ArgumentNullException(ensurer.ParameterName);

            return ensurer;
        }
    }

    /// <summary>
    ///     C# 14 Extension Members for Ensurer&lt;IEnumerable&lt;T&gt;&gt;.
    /// </summary>
    extension<T>(Ensurer<IEnumerable<T>> ensurer)
    {
        /// <summary>
        ///     Ensures the collection is not empty.
        /// </summary>
        public Ensurer<IEnumerable<T>> IsNotEmpty()
        {
            if (ensurer.Value is null || !ensurer.Value.Any())
                throw new ArgumentException("Collection cannot be empty.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the collection is not null or empty.
        /// </summary>
        [return: NotNull]
        public Ensurer<IEnumerable<T>> IsNotNullOrEmpty()
        {
            if (ensurer.Value is null)
                throw new ArgumentNullException(ensurer.ParameterName);

            if (!ensurer.Value.Any())
                throw new ArgumentException("Collection cannot be empty.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the collection has a minimum number of items.
        /// </summary>
        public Ensurer<IEnumerable<T>> AndHasMinCount(int minCount)
        {
            var count = ensurer.Value?.Count() ?? 0;
            if (count < minCount)
                throw new ArgumentException($"Collection must have at least {minCount} items but has {count}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the collection has a maximum number of items.
        /// </summary>
        public Ensurer<IEnumerable<T>> AndHasMaxCount(int maxCount)
        {
            var count = ensurer.Value?.Count() ?? 0;
            if (count > maxCount)
                throw new ArgumentException($"Collection must have at most {maxCount} items but has {count}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the collection count is within the specified range.
        /// </summary>
        public Ensurer<IEnumerable<T>> AndHasCountBetween(int minCount, int maxCount)
        {
            var count = ensurer.Value?.Count() ?? 0;
            if (count < minCount || count > maxCount)
                throw new ArgumentException($"Collection count must be between {minCount} and {maxCount} but has {count}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the collection has exactly the specified number of items.
        /// </summary>
        public Ensurer<IEnumerable<T>> AndHasExactCount(int expectedCount)
        {
            var count = ensurer.Value?.Count() ?? 0;
            if (count != expectedCount)
                throw new ArgumentException($"Collection must have exactly {expectedCount} items but has {count}.",
                    ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the collection contains the specified item.
        /// </summary>
        public Ensurer<IEnumerable<T>> AndContains(T item)
        {
            if (ensurer.Value is null || !ensurer.Value.Contains(item))
                throw new ArgumentException("Collection must contain the specified item.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures the collection does not contain the specified item.
        /// </summary>
        public Ensurer<IEnumerable<T>> AndDoesNotContain(T item)
        {
            if (ensurer.Value is not null && ensurer.Value.Contains(item))
                throw new ArgumentException("Collection must not contain the specified item.", ensurer.ParameterName);

            return ensurer;
        }

        /// <summary>
        ///     Ensures all items in the collection satisfy the specified condition.
        /// </summary>
        public Ensurer<IEnumerable<T>> AndAllItemsSatisfy(Func<T, bool> predicate, string? errorMessage = null)
        {
            if (ensurer.Value is null || !ensurer.Value.All(predicate))
            {
                var message = errorMessage ?? "Not all items in the collection satisfy the required condition.";
                throw new ArgumentException(message, ensurer.ParameterName);
            }

            return ensurer;
        }

        /// <summary>
        ///     Ensures at least one item in the collection satisfies the specified condition.
        /// </summary>
        public Ensurer<IEnumerable<T>> AndAnyItemSatisfies(Func<T, bool> predicate, string? errorMessage = null)
        {
            if (ensurer.Value is null || !ensurer.Value.Any(predicate))
            {
                var message = errorMessage ?? "No item in the collection satisfies the required condition.";
                throw new ArgumentException(message, ensurer.ParameterName);
            }

            return ensurer;
        }

        /// <summary>
        ///     Ensures the collection contains only distinct items.
        /// </summary>
        public Ensurer<IEnumerable<T>> AndContainsDistinctItems()
        {
            if (ensurer.Value is null)
                return ensurer;

            var count = ensurer.Value.Count();
            var distinctCount = ensurer.Value.Distinct().Count();

            if (count != distinctCount)
                throw new ArgumentException("Collection must contain only distinct items (no duplicates).",
                    ensurer.ParameterName);

            return ensurer;
        }
    }

    /// <summary>
    ///     C# 14 Extension Members for Ensurer&lt;IEnumerable&lt;T&gt;&gt; where T : class.
    /// </summary>
    extension<T>(Ensurer<IEnumerable<T>> ensurer) where T : class
    {
        /// <summary>
        ///     Ensures the collection contains no null items.
        /// </summary>
        public Ensurer<IEnumerable<T>> AndContainsNoNulls()
        {
            if (ensurer.Value is not null && ensurer.Value.Any(item => item is null))
                throw new ArgumentException("Collection must not contain null items.", ensurer.ParameterName);

            return ensurer;
        }
    }
}
