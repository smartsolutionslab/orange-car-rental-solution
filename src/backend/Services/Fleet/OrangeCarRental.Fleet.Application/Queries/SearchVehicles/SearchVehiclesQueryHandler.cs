using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

/// <summary>
///     Handler for SearchVehiclesQuery.
///     Delegates filtering and pagination to the repository for database-level performance.
/// </summary>
public sealed class SearchVehiclesQueryHandler(IVehicleRepository vehicles)
    : IQueryHandler<SearchVehiclesQuery, SearchVehiclesResult>
{
    /// <summary>
    ///     Handles the vehicle search query with filtering and pagination.
    /// </summary>
    /// <param name="queryCommand">The search query with filter criteria and pagination parameters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Paginated search results containing matching vehicles.</returns>
    public async Task<SearchVehiclesResult> HandleAsync(
        SearchVehiclesQuery queryCommand,
        CancellationToken cancellationToken = default)
    {
        var searchParameters = queryCommand.ToVehicleSearchParameters();
        var pagedResult = await vehicles.SearchAsync(searchParameters, cancellationToken);

        return pagedResult.ToDto();
    }
}
