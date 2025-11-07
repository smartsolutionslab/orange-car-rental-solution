using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleLocation;

/// <summary>
///     Handler for UpdateVehicleLocationCommand.
///     Moves a vehicle to a different rental location.
/// </summary>
public sealed class UpdateVehicleLocationCommandHandler(IVehicleRepository vehicles)
{
    /// <summary>
    ///     Handles the update vehicle location command.
    /// </summary>
    /// <param name="command">The command with vehicle ID and new location.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with old and new location details.</returns>
    /// <exception cref="InvalidOperationException">Thrown when vehicle is not found or cannot be moved.</exception>
    public async Task<UpdateVehicleLocationResult> HandleAsync(
        UpdateVehicleLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        // Load vehicle
        var vehicle = await vehicles.GetByIdAsync(command.VehicleId, cancellationToken)
            ?? throw new InvalidOperationException($"Vehicle with ID '{command.VehicleId}' not found.");

        var oldLocation = vehicle.CurrentLocation;

        // Move to new location (domain method validates and returns new instance)
        vehicle = vehicle.MoveToLocation(command.NewLocation);

        // Persist changes
        await vehicles.UpdateAsync(vehicle, cancellationToken);
        await vehicles.SaveChangesAsync(cancellationToken);

        // Return result
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
