namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Value object representing pagination information.
/// </summary>
public sealed record PagingInfo
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
    ///     The page number (1-based).
    /// </summary>
    public int PageNumber { get; }

    /// <summary>
    ///     The number of items per page.
    /// </summary>
    public int PageSize { get; }

    private PagingInfo(int pageNumber, int pageSize)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
    }

    /// <summary>
    ///     Creates a new PagingInfo instance.
    /// </summary>
    /// <param name="pageNumber">Page number (1-based, defaults to 1).</param>
    /// <param name="pageSize">Items per page (defaults to DefaultPageSize).</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when parameters are invalid.</exception>
    public static PagingInfo Create(int pageNumber = 1, int pageSize = DefaultPageSize)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(pageNumber, 1, nameof(pageNumber));
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1, nameof(pageSize));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(pageSize, MaxPageSize, nameof(pageSize));

        return new PagingInfo(pageNumber, pageSize);
    }

    /// <summary>
    ///     Tries to create a PagingInfo instance.
    /// </summary>
    public static bool TryCreate(int pageNumber, int pageSize, out PagingInfo? result)
    {
        if (pageNumber < 1 || pageSize < 1 || pageSize > MaxPageSize)
        {
            result = null;
            return false;
        }

        result = new PagingInfo(pageNumber, pageSize);
        return true;
    }

    /// <summary>
    ///     Default paging info (page 1, default size).
    /// </summary>
    public static PagingInfo Default => new(1, DefaultPageSize);

    /// <summary>
    ///     Creates paging info for the first page with specified size.
    /// </summary>
    public static PagingInfo FirstPage(int pageSize = DefaultPageSize) => Create(1, pageSize);

    /// <summary>
    ///     Calculates the number of items to skip for pagination.
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;

    /// <summary>
    ///     Gets the number of items to take (same as PageSize).
    /// </summary>
    public int Take => PageSize;

    /// <summary>
    ///     Creates paging info for the next page.
    /// </summary>
    public PagingInfo NextPage() => new(PageNumber + 1, PageSize);

    /// <summary>
    ///     Creates paging info for the previous page (minimum page 1).
    /// </summary>
    public PagingInfo PreviousPage() => new(Math.Max(1, PageNumber - 1), PageSize);

    /// <summary>
    ///     Creates paging info for a specific page number.
    /// </summary>
    public PagingInfo WithPage(int pageNumber) => Create(pageNumber, PageSize);

    /// <summary>
    ///     Creates paging info with a different page size.
    /// </summary>
    public PagingInfo WithSize(int pageSize) => Create(PageNumber, pageSize);
}
