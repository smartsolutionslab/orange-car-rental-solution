namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Base class for search parameters with pagination and sorting support.
/// </summary>
public abstract record SearchParameters(int PageNumber, int PageSize, string? SortBy, bool SortDescending)
{
    /// <summary>
    ///     Calculates the number of items to skip for pagination.
    /// </summary>
    public int Skip => (PageNumber - 1) * PageSize;

    /// <summary>
    ///     Gets the number of items to take (same as PageSize).
    /// </summary>
    public int Take => PageSize;

    /// <summary>
    ///     Validates the pagination parameters.
    /// </summary>
    public virtual void Validate()
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(PageNumber, 1, nameof(PageNumber));
        ArgumentOutOfRangeException.ThrowIfLessThan(PageSize, 1, nameof(PageSize));
        ArgumentOutOfRangeException.ThrowIfGreaterThan(PageSize, 100, nameof(PageSize));
    }
}
