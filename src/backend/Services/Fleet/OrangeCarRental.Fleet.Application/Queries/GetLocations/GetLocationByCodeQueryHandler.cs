using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;

/// <summary>
///     Handler for GetLocationByCodeQuery.
///     Returns a specific location by its code.
/// </summary>
public sealed class GetLocationByCodeQueryHandler: IQueryHandler<GetLocationByCodeQuery, LocationDto>
{
    /// <summary>
    ///     Handles the query to retrieve a specific location by code.
    /// </summary>
    /// <param name="query">The query containing the location code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The location DTO.</returns>
    public Task<LocationDto> HandleAsync(GetLocationByCodeQuery query, CancellationToken cancellationToken = default)
    {
        var location = Location.FromCode(query.Code);

        var dto = new LocationDto(
            location.Code.Value,
            location.Name.Value,
            location.Address.Street.Value,
            location.Address.City.Value,
            location.Address.PostalCode.Value,
            location.Address.FullAddress
            );

        return Task.FromResult(dto);
    }
}
