using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.AddLocation;

/// <summary>
///     Handler for AddLocationCommand.
///     Creates a new rental location.
/// </summary>
public sealed class AddLocationCommandHandler(ILocationRepository locations)
    : ICommandHandler<AddLocationCommand, AddLocationResult>
{
    /// <summary>
    ///     Handles the add location command.
    /// </summary>
    /// <param name="command">The command with location data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with the new location code and details.</returns>
    public async Task<AddLocationResult> HandleAsync(
        AddLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var (code, name, address) = command;

        // Check if location code already exists
        var existingLocation = await locations.FindByCodeAsync(code, cancellationToken);
        if (existingLocation != null)
        {
            throw new InvalidOperationException($"A location with code '{code.Value}' already exists.");
        }

        // Create new location
        var location = Location.Create(code, name, address);

        // Persist location
        await locations.AddAsync(location, cancellationToken);
        await locations.SaveChangesAsync(cancellationToken);

        return new AddLocationResult(
            location.Code.Value,
            location.Name.Value,
            location.Address.FullAddress,
            location.Status.ToString()
        );
    }
}
