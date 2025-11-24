using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;

/// <summary>
///     Handler for GetLocationByCodeQuery.
///     Returns a specific location by its code.
/// </summary>
public sealed class GetLocationByCodeQueryHandler(ILocationRepository locations)
    : IQueryHandler<GetLocationByCodeQuery, LocationDto>
{
    /// <summary>
    ///     Handles the query to retrieve a specific location by code.
    /// </summary>
    /// <param name="query">The query containing the location code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The location DTO.</returns>
    public async Task<LocationDto> HandleAsync(GetLocationByCodeQuery query, CancellationToken cancellationToken = default)
    {
        var location = await locations.GetByCodeAsync(query.Code, cancellationToken);

        if (location == null)
        {
            throw new ArgumentException($"Location with code '{query.Code.Value}' not found.", nameof(query.Code));
        }

        var dto = new LocationDto(
            location.Code.Value,
            location.Name.Value,
            location.Address.Street.Value,
            location.Address.City.Value,
            location.Address.PostalCode.Value,
            location.Address.FullAddress
            );

        return dto;
    }
}
