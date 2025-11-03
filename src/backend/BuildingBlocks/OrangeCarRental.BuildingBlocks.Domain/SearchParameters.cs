namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
/// Base class for search parameters with pagination and sorting support.
/// </summary>
public abstract class SearchParameters
{
    /// <summary>
    /// Page number (1-based).
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Sort field name.
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Sort in descending order.
    /// </summary>
    public bool SortDescending { get; init; }

    /// <summary>
    /// Validates the pagination parameters.
    /// </summary>
    public virtual void Validate()
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(PageNumber, 1, nameof(PageNumber));
        ArgumentOutOfRangeException.ThrowIfLessThan(PageSize, 1, nameof(PageSize));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(PageSize, 100, nameof(PageSize));
    }

    /// <summary>
    /// Calculates the number of items to skip for pagination.
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;

    /// <summary>
    /// Gets the number of items to take (same as PageSize).
    /// </summary>
    public int Take => PageSize;
}
