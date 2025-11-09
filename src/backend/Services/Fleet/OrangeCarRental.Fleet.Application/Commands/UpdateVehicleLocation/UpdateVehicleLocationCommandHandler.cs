using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleLocation;

/// <summary>
///     Handler for UpdateVehicleLocationCommand.
///     Moves a vehicle to a different rental location.
/// </summary>
public sealed class UpdateVehicleLocationCommandHandler(IVehicleRepository vehicles)
    : ICommandHandler<UpdateVehicleLocationCommand, UpdateVehicleLocationResult>
{
    /// <summary>
    ///     Handles the update vehicle location command.
    /// </summary>
    /// <param name="command">The command with vehicle ID and new location.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with old and new location details.</returns>
    /// <exception cref="InvalidOperationException">Thrown when vehicle is not found or cannot be moved.</exception>
    public async Task<UpdateVehicleLocationResult> HandleAsync(UpdateVehicleLocationCommand command, CancellationToken cancellationToken = default)
    {
        var (vehicleId, newLocation) = command;

        // Load vehicle
        var vehicle = await vehicles.GetByIdAsync(vehicleId, cancellationToken)
            ?? throw new InvalidOperationException($"Vehicle with ID '{vehicleId}' not found.");

        var oldLocation = vehicle.CurrentLocation;

        // Move to new location (domain method validates and returns new instance)
        vehicle = vehicle.MoveToLocation(newLocation);

        // Persist changes
        await vehicles.UpdateAsync(vehicle, cancellationToken);
        await vehicles.SaveChangesAsync(cancellationToken);

        return new UpdateVehicleLocationResult(
            vehicle.Id.Value,
            oldLocation.Code.Value,
            oldLocation.Name.Value,
            vehicle.CurrentLocation.Code.Value,
            vehicle.CurrentLocation.Name.Value,
            $"Vehicle moved from {oldLocation.Name.Value} to {vehicle.CurrentLocation.Name.Value}"
        );
    }
}
