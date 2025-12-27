namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Value object representing sorting information.
/// </summary>
public sealed record SortingInfo
{
    /// <summary>
    ///     The field to sort by.
    /// </summary>
    public string? SortBy { get; }

    /// <summary>
    ///     Whether to sort in descending order.
    /// </summary>
    public bool Descending { get; }

    private SortingInfo(string? sortBy, bool descending)
    {
        SortBy = sortBy?.Trim();
        Descending = descending;
    }

    /// <summary>
    ///     Creates a new SortingInfo instance.
    /// </summary>
    /// <param name="sortBy">The field to sort by (null for default sorting).</param>
    /// <param name="descending">Whether to sort descending.</param>
    public static SortingInfo Create(string? sortBy = null, bool descending = false)
    {
        return new SortingInfo(sortBy, descending);
    }

    /// <summary>
    ///     No sorting applied (use default).
    /// </summary>
    public static SortingInfo None => new(null, false);

    /// <summary>
    ///     Creates ascending sort by the specified field.
    /// </summary>
    public static SortingInfo AscendingBy(string sortBy) => new(sortBy, false);

    /// <summary>
    ///     Creates descending sort by the specified field.
    /// </summary>
    public static SortingInfo DescendingBy(string sortBy) => new(sortBy, true);

    /// <summary>
    ///     Whether sorting is specified.
    /// </summary>
    public bool HasSorting => !string.IsNullOrWhiteSpace(SortBy);

    /// <summary>
    ///     Creates a new SortingInfo with the same field but ascending order.
    /// </summary>
    public SortingInfo ToAscending() => new(SortBy, false);

    /// <summary>
    ///     Creates a new SortingInfo with the same field but descending order.
    /// </summary>
    public SortingInfo ToDescending() => new(SortBy, true);

    /// <summary>
    ///     Creates a new SortingInfo with toggled sort direction.
    /// </summary>
    public SortingInfo Toggle() => new(SortBy, !Descending);

    /// <summary>
    ///     Creates a new SortingInfo with a different field.
    /// </summary>
    public SortingInfo WithField(string sortBy) => new(sortBy, Descending);
}
