namespace SmartSolutionsLab.Fleet.Application.Queries.SearchVehicles;

/// <summary>
/// Result of vehicle search query with pagination information.
/// </summary>
public sealed record SearchVehiclesResult
{
    /// <summary>
    /// List of vehicles matching the search criteria.
    /// </summary>
    public required List<VehicleDto> Vehicles { get; init; }

    /// <summary>
    /// Total number of vehicles matching the criteria (across all pages).
    /// </summary>
    public required int TotalCount { get; init; }

    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public required int PageNumber { get; init; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public required int TotalPages { get; init; }

    /// <summary>
    /// Whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
