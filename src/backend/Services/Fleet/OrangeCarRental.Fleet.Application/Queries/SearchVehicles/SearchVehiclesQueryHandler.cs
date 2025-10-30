using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Enums;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Repositories;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

/// <summary>
/// Handler for SearchVehiclesQuery.
/// Delegates filtering and pagination to the repository for database-level performance.
/// </summary>
public sealed class SearchVehiclesQueryHandler(IVehicleRepository repository)
{
    public async Task<SearchVehiclesResult> HandleAsync(
        SearchVehiclesQuery query,
        CancellationToken cancellationToken = default)
    {
        var searchParameters = BuildVehicleSearchParameters(query);
        var pagedResult = await repository.SearchAsync(searchParameters, cancellationToken);

        // Map to DTOs
        var dtos = pagedResult.Items.Select(vehicle => vehicle.ToDto()).ToList();

        return new SearchVehiclesResult
        {
            Vehicles = dtos,
            TotalCount = pagedResult.TotalCount,
            PageNumber = pagedResult.PageNumber,
            PageSize = pagedResult.PageSize,
            TotalPages = pagedResult.TotalPages
        };
    }

    private static VehicleSearchParameters BuildVehicleSearchParameters(SearchVehiclesQuery query)
    {
        var searchParameters = new VehicleSearchParameters
        {
            LocationCode = query.LocationCode,
            CategoryCode = query.CategoryCode,
            MinSeats = query.MinSeats,
            FuelType = query.FuelType.TryParseFuelType(),
            TransmissionType = query.TransmissionType.TryParseTransmissionType(),
            MaxDailyRateGross = query.MaxDailyRateGross,
            Status = VehicleStatus.Available, // Always filter to available vehicles
            PageNumber = query.PageNumber ?? 1,
            PageSize = query.PageSize ?? 20
        };
        return searchParameters;
    }
}
