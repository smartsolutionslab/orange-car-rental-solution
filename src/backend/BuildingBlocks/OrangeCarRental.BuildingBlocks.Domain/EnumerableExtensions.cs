namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Extension methods for IEnumerable to support pagination and other common query operations.
/// </summary>
public static class EnumerableExtensions
{
    /// <summary>
    ///     Applies pagination to an enumerable (in-memory collection) and returns a paged result.
    /// </summary>
    public static PagedResult<T> ToPagedResult<T>(
        this IEnumerable<T> source,
        SearchParameters parameters)
    {
        var items = source as IList<T> ?? source.ToList();

        var pagedItems = items
            .Skip(parameters.Skip)
            .Take(parameters.Take)
            .ToList();

        return new PagedResult<T>
        {
            Items = pagedItems,
            TotalCount = items.Count,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }
}
