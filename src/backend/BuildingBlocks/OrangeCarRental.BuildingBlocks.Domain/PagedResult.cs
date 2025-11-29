namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Represents a paged result set from a query.
/// </summary>
/// <typeparam name="TItem">The type of items in the result set.</typeparam>
public sealed class PagedResult<TItem>
{
    /// <summary>
    ///     Default page size used across the application.
    /// </summary>
    public const int DefaultPageSize = 20;

    /// <summary>
    ///     Maximum allowed page size.
    /// </summary>
    public const int MaxPageSize = 100;

    /// <summary>
    ///     The items in the current page. This collection is read-only to prevent modification.
    /// </summary>
    public IReadOnlyList<TItem> Items { get; init; } = [];

    public int TotalCount { get; init; }
    public int PageNumber { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling((double)TotalCount / PageSize) : 0;
    public bool HasPreviousPage => PageNumber > 1;
    public bool HasNextPage => PageNumber < TotalPages;

    /// <summary>
    ///     Creates an empty paged result.
    /// </summary>
    public static PagedResult<TItem> Empty(int pageNumber = 1, int pageSize = DefaultPageSize) => new()
    {
        Items = [],
        TotalCount = 0,
        PageNumber = pageNumber,
        PageSize = pageSize
    };

    /// <summary>
    ///     Maps items to a different type while preserving pagination info.
    /// </summary>
    public PagedResult<TResult> Map<TResult>(Func<TItem, TResult> selector) => new()
    {
        Items = Items.Select(selector).ToList(),
        TotalCount = TotalCount,
        PageNumber = PageNumber,
        PageSize = PageSize
    };
}
