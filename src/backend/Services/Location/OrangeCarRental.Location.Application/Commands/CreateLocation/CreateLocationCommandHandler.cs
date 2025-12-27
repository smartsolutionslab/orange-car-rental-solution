using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Location.Domain;
using SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Location.Application.Commands.CreateLocation;

public sealed class CreateLocationCommandHandler(
    ILocationsUnitOfWork unitOfWork)
    : ICommandHandler<CreateLocationCommand, CreateLocationResult>
{
    public async Task<CreateLocationResult> HandleAsync(
        CreateLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Create value objects
        var code = LocationCode.From(command.Code);
        var name = LocationName.From(command.Name);
        var address = LocationAddress.Of(command.Street, command.City, command.PostalCode);
        var openingHours = OpeningHours.From(command.OpeningHours);
        var contact = ContactInfo.Of(command.Phone, command.Email);

        GeoCoordinates? coordinates = null;
        if (command.Latitude.HasValue && command.Longitude.HasValue)
        {
            coordinates = GeoCoordinates.Of(command.Latitude.Value, command.Longitude.Value);
        }

        var locations = unitOfWork.Locations;

        // Check if location with same code already exists
        var existingLocation = await locations.FindByCodeAsync(code, cancellationToken);
        if (existingLocation != null)
        {
            throw new InvalidOperationException($"A location with code '{code.Value}' already exists.");
        }

        // Create location aggregate
        var location = Domain.Location.Location.Create(
            code, name, address, openingHours, contact, coordinates);

        // Persist
        await locations.AddAsync(location, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new CreateLocationResult
        {
            Code = location.Code.Value,
            Name = location.Name.Value
        };
    }
}
