using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

/// <summary>
///     Base class for search parameters with pagination and sorting support.
/// </summary>
public abstract record SearchParameters(PagingInfo Paging, SortingInfo Sorting)
{
    /// <summary>
    ///     Creates SearchParameters with default paging and no sorting.
    /// </summary>
    protected SearchParameters() : this(PagingInfo.Default, SortingInfo.None)
    {
    }

    /// <summary>
    ///     Creates SearchParameters with specified paging and no sorting.
    /// </summary>
    protected SearchParameters(PagingInfo paging) : this(paging, SortingInfo.None)
    {
    }

    /// <summary>
    ///     Calculates the number of items to skip for pagination.
    /// </summary>
    public int Skip => Paging.Skip;

    /// <summary>
    ///     Gets the number of items to take (same as PageSize).
    /// </summary>
    public int Take => Paging.Take;

    /// <summary>
    ///     Gets the page number.
    /// </summary>
    public int PageNumber => Paging.PageNumber;

    /// <summary>
    ///     Gets the page size.
    /// </summary>
    public int PageSize => Paging.PageSize;

    /// <summary>
    ///     Gets the sort field.
    /// </summary>
    public string? SortBy => Sorting.SortBy;

    /// <summary>
    ///     Gets whether sorting is descending.
    /// </summary>
    public bool SortDescending => Sorting.Descending;

    /// <summary>
    ///     Validates the search parameters.
    /// </summary>
    public virtual void Validate()
    {
        // PagingInfo validates itself on creation
        // SortingInfo is always valid
        // Derived classes can add additional validation
    }
}
