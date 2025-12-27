using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;

/// <summary>
///     Extension methods for IQueryable to support paging, sorting, and filtering.
///     Uses C# 14 Extension Members syntax.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for IQueryable&lt;T&gt;.
    /// </summary>
    extension<T>(IQueryable<T> query)
    {
        /// <summary>
        ///     Applies pagination to the query and returns a PagedResult.
        ///     Executes count query and applies Skip/Take at the database level.
        /// </summary>
        /// <param name="paging">The paging information.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>A paged result containing items and pagination metadata.</returns>
        public async Task<PagedResult<T>> ToPagedResultAsync(
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
        /// <param name="sorting">The sorting information.</param>
        /// <param name="fieldSelectors">Dictionary mapping field names to selector expressions.</param>
        /// <param name="defaultSelector">Default selector when no sort field specified.</param>
        /// <param name="defaultDescending">Default sort direction when no sort field specified.</param>
        /// <returns>The sorted query.</returns>
        public IQueryable<T> ApplySorting(
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
        /// <param name="condition">Whether to apply the filter.</param>
        /// <param name="predicate">The filter predicate.</param>
        /// <returns>The filtered query.</returns>
        public IQueryable<T> WhereIf(
            bool condition,
            Expression<Func<T, bool>> predicate)
        {
            return condition ? query.Where(predicate) : query;
        }

        /// <summary>
        ///     Applies an IntRange filter to the query for minimum value.
        /// </summary>
        /// <param name="range">The integer range.</param>
        /// <param name="minPredicate">Predicate when minimum is specified.</param>
        /// <param name="maxPredicate">Predicate when maximum is specified.</param>
        /// <returns>The filtered query.</returns>
        public IQueryable<T> WhereInRange(
            IntRange? range,
            Expression<Func<T, bool>> minPredicate,
            Expression<Func<T, bool>> maxPredicate)
        {
            if (range is not { HasFilter: true })
                return query;

            var result = query;

            if (range.Min.HasValue)
                result = result.Where(minPredicate);

            if (range.Max.HasValue)
                result = result.Where(maxPredicate);

            return result;
        }

        /// <summary>
        ///     Applies a DateRange filter to the query.
        /// </summary>
        /// <param name="range">The date range.</param>
        /// <param name="fromPredicate">Predicate when from date is specified.</param>
        /// <param name="toPredicate">Predicate when to date is specified.</param>
        /// <returns>The filtered query.</returns>
        public IQueryable<T> WhereInDateRange(
            DateRange? range,
            Expression<Func<T, bool>> fromPredicate,
            Expression<Func<T, bool>> toPredicate)
        {
            if (range is not { HasFilter: true })
                return query;

            var result = query;

            if (range.From.HasValue)
                result = result.Where(fromPredicate);

            if (range.To.HasValue)
                result = result.Where(toPredicate);

            return result;
        }

        /// <summary>
        ///     Applies a PriceRange filter to the query.
        /// </summary>
        /// <param name="range">The price range.</param>
        /// <param name="minPredicate">Predicate when minimum price is specified.</param>
        /// <param name="maxPredicate">Predicate when maximum price is specified.</param>
        /// <returns>The filtered query.</returns>
        public IQueryable<T> WhereInPriceRange(
            PriceRange? range,
            Expression<Func<T, bool>> minPredicate,
            Expression<Func<T, bool>> maxPredicate)
        {
            if (range is not { HasFilter: true })
                return query;

            var result = query;

            if (range.Min.HasValue)
                result = result.Where(minPredicate);

            if (range.Max.HasValue)
                result = result.Where(maxPredicate);

            return result;
        }
    }
}
