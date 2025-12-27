using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;

/// <summary>
///     Handler for GetLocationsQuery.
///     Returns all available rental locations.
/// </summary>
public sealed class GetLocationsQueryHandler(ILocationRepository locations)
    : IQueryHandler<GetLocationsQuery, IReadOnlyList<LocationDto>>
{
    /// <summary>
    ///     Handles the query to retrieve all rental locations.
    /// </summary>
    /// <param name="query">The query requesting all locations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of all available rental locations.</returns>
    public async Task<IReadOnlyList<LocationDto>> HandleAsync(
        GetLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        var allLocations = await locations.GetAllAsync(cancellationToken);

        var locationDtos = allLocations
            .Select(location => new LocationDto(
                location.Code.Value,
                location.Name.Value,
                location.Address.Street.Value,
                location.Address.City.Value,
                location.Address.PostalCode.Value,
                location.Address.FullAddress))
            .ToList();

        return locationDtos;
    }
}
