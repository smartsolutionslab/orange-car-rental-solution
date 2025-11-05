using System.Diagnostics.CodeAnalysis;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

/// <summary>
/// Provides fluent validation extension methods for collection values.
/// </summary>
public static class EnsureCollectionExtensions
{
    /// <summary>
    /// Ensures the collection is not null.
    /// </summary>
    public static Ensurer<T> IsNotNull<T>([NotNull] this Ensurer<T> ensurer) where T : class, IEnumerable<object>
    {
        if (ensurer.Value is null)
            throw new ArgumentNullException(ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the collection is not empty.
    /// </summary>
    public static Ensurer<IEnumerable<T>> IsNotEmpty<T>(this Ensurer<IEnumerable<T>> ensurer)
    {
        if (ensurer.Value is null || !ensurer.Value.Any())
            throw new ArgumentException("Collection cannot be empty.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the collection is not null or empty.
    /// </summary>
    public static Ensurer<IEnumerable<T>> IsNotNullOrEmpty<T>([NotNull] this Ensurer<IEnumerable<T>> ensurer)
    {
        if (ensurer.Value is null)
            throw new ArgumentNullException(ensurer.ParameterName);

        if (!ensurer.Value.Any())
            throw new ArgumentException("Collection cannot be empty.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the collection has a minimum number of items.
    /// </summary>
    public static Ensurer<IEnumerable<T>> AndHasMinCount<T>(this Ensurer<IEnumerable<T>> ensurer, int minCount)
    {
        var count = ensurer.Value?.Count() ?? 0;
        if (count < minCount)
            throw new ArgumentException($"Collection must have at least {minCount} items but has {count}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the collection has a maximum number of items.
    /// </summary>
    public static Ensurer<IEnumerable<T>> AndHasMaxCount<T>(this Ensurer<IEnumerable<T>> ensurer, int maxCount)
    {
        var count = ensurer.Value?.Count() ?? 0;
        if (count > maxCount)
            throw new ArgumentException($"Collection must have at most {maxCount} items but has {count}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the collection count is within the specified range.
    /// </summary>
    public static Ensurer<IEnumerable<T>> AndHasCountBetween<T>(this Ensurer<IEnumerable<T>> ensurer, int minCount, int maxCount)
    {
        var count = ensurer.Value?.Count() ?? 0;
        if (count < minCount || count > maxCount)
            throw new ArgumentException($"Collection count must be between {minCount} and {maxCount} but has {count}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the collection has exactly the specified number of items.
    /// </summary>
    public static Ensurer<IEnumerable<T>> AndHasExactCount<T>(this Ensurer<IEnumerable<T>> ensurer, int expectedCount)
    {
        var count = ensurer.Value?.Count() ?? 0;
        if (count != expectedCount)
            throw new ArgumentException($"Collection must have exactly {expectedCount} items but has {count}.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the collection contains the specified item.
    /// </summary>
    public static Ensurer<IEnumerable<T>> AndContains<T>(this Ensurer<IEnumerable<T>> ensurer, T item)
    {
        if (ensurer.Value is null || !ensurer.Value.Contains(item))
            throw new ArgumentException($"Collection must contain the specified item.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the collection does not contain the specified item.
    /// </summary>
    public static Ensurer<IEnumerable<T>> AndDoesNotContain<T>(this Ensurer<IEnumerable<T>> ensurer, T item)
    {
        if (ensurer.Value is not null && ensurer.Value.Contains(item))
            throw new ArgumentException($"Collection must not contain the specified item.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures all items in the collection satisfy the specified condition.
    /// </summary>
    public static Ensurer<IEnumerable<T>> AndAllItemsSatisfy<T>(this Ensurer<IEnumerable<T>> ensurer, Func<T, bool> predicate, string? errorMessage = null)
    {
        if (ensurer.Value is null || !ensurer.Value.All(predicate))
        {
            var message = errorMessage ?? "Not all items in the collection satisfy the required condition.";
            throw new ArgumentException(message, ensurer.ParameterName);
        }

        return ensurer;
    }

    /// <summary>
    /// Ensures at least one item in the collection satisfies the specified condition.
    /// </summary>
    public static Ensurer<IEnumerable<T>> AndAnyItemSatisfies<T>(this Ensurer<IEnumerable<T>> ensurer, Func<T, bool> predicate, string? errorMessage = null)
    {
        if (ensurer.Value is null || !ensurer.Value.Any(predicate))
        {
            var message = errorMessage ?? "No item in the collection satisfies the required condition.";
            throw new ArgumentException(message, ensurer.ParameterName);
        }

        return ensurer;
    }

    /// <summary>
    /// Ensures the collection contains no null items.
    /// </summary>
    public static Ensurer<IEnumerable<T>> AndContainsNoNulls<T>(this Ensurer<IEnumerable<T>> ensurer) where T : class
    {
        if (ensurer.Value is not null && ensurer.Value.Any(item => item is null))
            throw new ArgumentException("Collection must not contain null items.", ensurer.ParameterName);

        return ensurer;
    }

    /// <summary>
    /// Ensures the collection contains only distinct items.
    /// </summary>
    public static Ensurer<IEnumerable<T>> AndContainsDistinctItems<T>(this Ensurer<IEnumerable<T>> ensurer)
    {
        if (ensurer.Value is null)
            return ensurer;

        var count = ensurer.Value.Count();
        var distinctCount = ensurer.Value.Distinct().Count();

        if (count != distinctCount)
            throw new ArgumentException("Collection must contain only distinct items (no duplicates).", ensurer.ParameterName);

        return ensurer;
    }
}
