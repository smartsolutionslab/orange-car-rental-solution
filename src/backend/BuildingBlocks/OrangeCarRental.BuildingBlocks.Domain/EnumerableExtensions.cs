namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Extension methods for IEnumerable to support pagination and other common query operations.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     Applies pagination to an enumerable (in-memory collection) and returns a paged result.
    /// </summary>
    public static PagedResult<TItem> ToPagedResult<TItem>(
        this IEnumerable<TItem> source,
        SearchParameters parameters)
    {
        var items = source as IList<TItem> ?? source.ToList();

        var pagedItems = items
            .Skip(parameters.Skip)
            .Take(parameters.Take)
            .ToList();

        return new PagedResult<TItem>
        {
            Items = pagedItems,
            TotalCount = items.Count,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }
}
