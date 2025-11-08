using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;

/// <summary>
///     Handler for GetLocationsQuery.
///     Returns all available rental locations.
/// </summary>
public sealed class GetLocationsQueryHandler
    : IQueryHandler<GetLocationsQuery, IReadOnlyList<LocationDto>>
{
    /// <summary>
    ///     Handles the query to retrieve all rental locations.
    /// </summary>
    /// <param name="query">The query requesting all locations.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A read-only list of all available rental locations.</returns>
    public Task<IReadOnlyList<LocationDto>> HandleAsync(
        GetLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        var locations = Location.All
            .Select(location => new LocationDto
            {
                Code = location.Code.Value,
                Name = location.Name.Value,
                Street = location.Address.Street.Value,
                City = location.Address.City.Value,
                PostalCode = location.Address.PostalCode.Value,
                FullAddress = location.Address.FullAddress
            })
            .ToList();

        return Task.FromResult<IReadOnlyList<LocationDto>>(locations);
    }
}
