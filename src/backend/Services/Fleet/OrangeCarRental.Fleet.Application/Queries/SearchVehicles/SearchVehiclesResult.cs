namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

/// <summary>
///     Result of vehicle search query with pagination information.
/// </summary>
public sealed record SearchVehiclesResult(
    List<VehicleDto> Vehicles,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages)
{
    /// <summary>
    ///     Whether there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    ///     Whether there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
