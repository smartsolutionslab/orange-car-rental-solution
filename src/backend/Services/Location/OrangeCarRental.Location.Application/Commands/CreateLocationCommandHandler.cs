using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Location.Domain;
using SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Location.Application.Commands;

public sealed class CreateLocationCommandHandler(
    ILocationsUnitOfWork unitOfWork)
    : ICommandHandler<CreateLocationCommand, CreateLocationResult>
{
    public async Task<CreateLocationResult> HandleAsync(
        CreateLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var locations = unitOfWork.Locations;

        // Check if location with same code already exists
        var existingLocation = await locations.FindByCodeAsync(command.Code, cancellationToken);
        if (existingLocation != null) throw new InvalidOperationException($"A location with code '{command.Code.Value}' already exists.");

        // Create location aggregate
        var location = Domain.Location.Location.Create(
            command.Code,
            command.Name,
            command.Address,
            command.OpeningHours,
            command.Contact,
            command.Coordinates);

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
