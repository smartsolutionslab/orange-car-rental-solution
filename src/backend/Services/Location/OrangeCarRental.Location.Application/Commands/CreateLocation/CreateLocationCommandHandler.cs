using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Location.Application.Commands.CreateLocation;

public sealed class CreateLocationCommandHandler(ILocationRepository locations)
    : ICommandHandler<CreateLocationCommand, CreateLocationResult>
{
    public async Task<CreateLocationResult> HandleAsync(
        CreateLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Create value objects
        var code = LocationCode.Of(command.Code);
        var name = LocationName.Of(command.Name);
        var address = LocationAddress.Of(command.Street, command.City, command.PostalCode);
        var openingHours = OpeningHours.Of(command.OpeningHours);
        var contact = ContactInfo.Of(command.Phone, command.Email);

        GeoCoordinates? coordinates = null;
        if (command.Latitude.HasValue && command.Longitude.HasValue)
        {
            coordinates = GeoCoordinates.Of(command.Latitude.Value, command.Longitude.Value);
        }

        // Create location aggregate
        var location = Domain.Location.Location.Create(
            code, name, address, openingHours, contact, coordinates);

        // Persist
        await locations.AddAsync(location, cancellationToken);
        await locations.SaveChangesAsync(cancellationToken);

        return new CreateLocationResult
        {
            LocationId = location.Id.Value,
            Code = location.Code.Value,
            Name = location.Name.Value
        };
    }
}
