using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.GetLocations;

/// <summary>
/// Handler for GetLocationByCodeQuery.
/// Returns a specific location by its code.
/// </summary>
public sealed class GetLocationByCodeQueryHandler
{
    public Task<LocationDto?> HandleAsync(
        GetLocationByCodeQuery query,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var locationCode = LocationCode.Of(query.Code);
            var location = Location.FromCode(locationCode);

            var dto = new LocationDto
            {
                Code = location.Code.Value,
                Name = location.Name.Value,
                Street = location.Address.Street.Value,
                City = location.Address.City.Value,
                PostalCode = location.Address.PostalCode.Value,
                FullAddress = location.Address.FullAddress
            };

            return Task.FromResult<LocationDto?>(dto);
        }
        catch (ArgumentException)
        {
            // Location code not found
            return Task.FromResult<LocationDto?>(null);
        }
    }
}
