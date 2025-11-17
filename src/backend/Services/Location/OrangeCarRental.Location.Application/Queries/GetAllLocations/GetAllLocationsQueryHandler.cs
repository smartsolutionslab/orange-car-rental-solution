using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Location.Application.Queries.GetAllLocations;

public sealed class GetAllLocationsQueryHandler(ILocationRepository locations)
    : IQueryHandler<GetAllLocationsQuery, GetAllLocationsResult>
{
    public async Task<GetAllLocationsResult> HandleAsync(
        GetAllLocationsQuery query,
        CancellationToken cancellationToken = default)
    {
        var allLocations = await locations.GetAllActiveAsync(cancellationToken);

        var dtos = allLocations.Select(l => new LocationDto
        {
            Id = l.Id.Value,
            Code = l.Code.Value,
            Name = l.Name.Value,
            Address = l.Address.ToString(),
            OpeningHours = l.OpeningHours.Value,
            Phone = l.Contact.Phone,
            Email = l.Contact.Email,
            Status = l.Status.ToString()
        }).ToList();

        return new GetAllLocationsResult { Locations = dtos };
    }
}
