using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;

/// <summary>
///     Handler for GetLocationsQuery.
///     Returns all available rental locations.
/// </summary>
public sealed class GetLocationsQueryHandler : IQueryHandler<GetLocationsQuery, IReadOnlyList<LocationDto>>
{
    /// <summary>
    ///     Handles the query to retrieve all rental locations.
    /// </summary>
    /// <param name="query">The query requesting all locations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of all available rental locations.</returns>
    public Task<IReadOnlyList<LocationDto>> HandleAsync(GetLocationsQuery query, CancellationToken cancellationToken = default)
    {
        var locations = Location.All
            .Select(location => new LocationDto(
                location.Code.Value,
                location.Name.Value,
                location.Address.Street.Value,
                location.Address.City.Value,
                location.Address.PostalCode.Value,
                location.Address.FullAddress))
            .ToList();

        return Task.FromResult<IReadOnlyList<LocationDto>>(locations);
    }
}
