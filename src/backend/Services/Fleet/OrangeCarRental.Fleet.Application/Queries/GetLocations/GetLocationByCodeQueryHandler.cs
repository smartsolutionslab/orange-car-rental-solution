using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;

/// <summary>
///     Handler for GetLocationByCodeQuery.
///     Returns a specific location by its code.
/// </summary>
public sealed class GetLocationByCodeQueryHandler
    : IQueryHandler<GetLocationByCodeQuery, LocationDto>
{
    /// <summary>
    ///     Handles the query to retrieve a specific location by code.
    /// </summary>
    /// <param name="query">The query containing the location code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The location DTO.</returns>
    public Task<LocationDto> HandleAsync(
        GetLocationByCodeQuery query,
        CancellationToken cancellationToken = default)
    {
        var location = Location.FromCode(query.Code);

        var dto = new LocationDto
        {
            Code = location.Code.Value,
            Name = location.Name.Value,
            Street = location.Address.Street.Value,
            City = location.Address.City.Value,
            PostalCode = location.Address.PostalCode.Value,
            FullAddress = location.Address.FullAddress
        };

        return Task.FromResult(dto);
    }
}
