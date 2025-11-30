using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;

/// <summary>
///     Extension methods for IQueryable to support paging, sorting, and filtering.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    ///     Applies pagination to the query and returns a PagedResult.
    ///     Executes count query and applies Skip/Take at the database level.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The query to paginate.</param>
    /// <param name="paging">The paging information.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A paged result containing items and pagination metadata.</returns>
    public static async Task<PagedResult<T>> ToPagedResultAsync<T>(
        this IQueryable<T> query,
        PagingInfo paging,
        CancellationToken cancellationToken = default)
    {
        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .Skip(paging.Skip)
            .Take(paging.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = paging.PageNumber,
            PageSize = paging.PageSize
        };
    }

    /// <summary>
    ///     Applies sorting to the query using a field selector dictionary.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The query to sort.</param>
    /// <param name="sorting">The sorting information.</param>
    /// <param name="fieldSelectors">Dictionary mapping field names to selector expressions.</param>
    /// <param name="defaultSelector">Default selector when no sort field specified.</param>
    /// <param name="defaultDescending">Default sort direction when no sort field specified.</param>
    /// <returns>The sorted query.</returns>
    public static IQueryable<T> ApplySorting<T>(
        this IQueryable<T> query,
        SortingInfo sorting,
        Dictionary<string, Expression<Func<T, object?>>> fieldSelectors,
        Expression<Func<T, object?>> defaultSelector,
        bool defaultDescending = false)
    {
        if (!sorting.HasSorting)
        {
            return defaultDescending
                ? query.OrderByDescending(defaultSelector)
                : query.OrderBy(defaultSelector);
        }

        var sortField = sorting.SortBy!.Trim().ToLowerInvariant();

        if (fieldSelectors.TryGetValue(sortField, out var selector))
        {
            return sorting.Descending
                ? query.OrderByDescending(selector)
                : query.OrderBy(selector);
        }

        // Field not found, use default
        return defaultDescending
            ? query.OrderByDescending(defaultSelector)
            : query.OrderBy(defaultSelector);
    }

    /// <summary>
    ///     Applies an optional filter condition to the query.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The query to filter.</param>
    /// <param name="condition">Whether to apply the filter.</param>
    /// <param name="predicate">The filter predicate.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<T> WhereIf<T>(
        this IQueryable<T> query,
        bool condition,
        Expression<Func<T, bool>> predicate)
    {
        return condition ? query.Where(predicate) : query;
    }

    /// <summary>
    ///     Applies an IntRange filter to the query for minimum value.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The query to filter.</param>
    /// <param name="range">The integer range.</param>
    /// <param name="minPredicate">Predicate when minimum is specified.</param>
    /// <param name="maxPredicate">Predicate when maximum is specified.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<T> WhereInRange<T>(
        this IQueryable<T> query,
        IntRange? range,
        Expression<Func<T, bool>> minPredicate,
        Expression<Func<T, bool>> maxPredicate)
    {
        if (range is not { HasFilter: true })
            return query;

        if (range.Min.HasValue)
            query = query.Where(minPredicate);

        if (range.Max.HasValue)
            query = query.Where(maxPredicate);

        return query;
    }

    /// <summary>
    ///     Applies a DateRange filter to the query.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The query to filter.</param>
    /// <param name="range">The date range.</param>
    /// <param name="fromPredicate">Predicate when from date is specified.</param>
    /// <param name="toPredicate">Predicate when to date is specified.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<T> WhereInDateRange<T>(
        this IQueryable<T> query,
        DateRange? range,
        Expression<Func<T, bool>> fromPredicate,
        Expression<Func<T, bool>> toPredicate)
    {
        if (range is not { HasFilter: true })
            return query;

        if (range.From.HasValue)
            query = query.Where(fromPredicate);

        if (range.To.HasValue)
            query = query.Where(toPredicate);

        return query;
    }

    /// <summary>
    ///     Applies a PriceRange filter to the query.
    /// </summary>
    /// <typeparam name="T">The entity type.</typeparam>
    /// <param name="query">The query to filter.</param>
    /// <param name="range">The price range.</param>
    /// <param name="minPredicate">Predicate when minimum price is specified.</param>
    /// <param name="maxPredicate">Predicate when maximum price is specified.</param>
    /// <returns>The filtered query.</returns>
    public static IQueryable<T> WhereInPriceRange<T>(
        this IQueryable<T> query,
        PriceRange? range,
        Expression<Func<T, bool>> minPredicate,
        Expression<Func<T, bool>> maxPredicate)
    {
        if (range is not { HasFilter: true })
            return query;

        if (range.Min.HasValue)
            query = query.Where(minPredicate);

        if (range.Max.HasValue)
            query = query.Where(maxPredicate);

        return query;
    }
}
