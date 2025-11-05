using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;

/// <summary>
///     Handler for GetLocationsQuery.
///     Returns all available rental locations.
/// </summary>
public sealed class GetLocationsQueryHandler
{
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
