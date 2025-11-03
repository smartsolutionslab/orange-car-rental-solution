using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

/// <summary>
/// Handler for SearchVehiclesQuery.
/// Delegates filtering and pagination to the repository for database-level performance.
/// </summary>
public sealed class SearchVehiclesQueryHandler(IVehicleRepository vehicles)
{
    public async Task<SearchVehiclesResult> HandleAsync(
        SearchVehiclesQuery queryCommand,
        CancellationToken cancellationToken = default)
    {
        var searchParameters = queryCommand.ToVehicleSearchParameters();
        var pagedResult = await vehicles.SearchAsync(searchParameters, cancellationToken);

        return pagedResult.ToDto();
    }
}
