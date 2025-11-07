using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleStatus;

/// <summary>
///     Handler for UpdateVehicleStatusCommand.
///     Updates a vehicle's operational status.
/// </summary>
public sealed class UpdateVehicleStatusCommandHandler(IVehicleRepository vehicles)
{
    /// <summary>
    ///     Handles the update vehicle status command.
    /// </summary>
    /// <param name="command">The command with vehicle ID and new status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with old and new status.</returns>
    /// <exception cref="InvalidOperationException">Thrown when vehicle is not found.</exception>
    public async Task<UpdateVehicleStatusResult> HandleAsync(
        UpdateVehicleStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        // Load vehicle
        var vehicle = await vehicles.GetByIdAsync(command.VehicleId, cancellationToken)
            ?? throw new InvalidOperationException($"Vehicle with ID '{command.VehicleId}' not found.");

        var oldStatus = vehicle.Status;

        // Update status (domain method returns new instance)
        vehicle = vehicle.ChangeStatus(command.NewStatus);

        // Persist changes
        await vehicles.UpdateAsync(vehicle, cancellationToken);
        await vehicles.SaveChangesAsync(cancellationToken);

        // Return result
        return new UpdateVehicleStatusResult(
            vehicle.Id.Value,
            oldStatus.ToString(),
            vehicle.Status.ToString(),
            $"Vehicle status changed from {oldStatus} to {vehicle.Status}"
        );
    }
}
