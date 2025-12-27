using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleStatus;

/// <summary>
///     Handler for UpdateVehicleStatusCommand.
///     Updates a vehicle's operational status.
/// </summary>
public sealed class UpdateVehicleStatusCommandHandler(IFleetUnitOfWork unitOfWork)
    : ICommandHandler<UpdateVehicleStatusCommand, UpdateVehicleStatusResult>
{
    private IVehicleRepository Vehicles => unitOfWork.Vehicles;

    /// <summary>
    ///     Handles the update vehicle status command.
    /// </summary>
    /// <param name="command">The command with vehicle ID and new status.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with old and new status.</returns>
    /// <exception cref="BuildingBlocks.Domain.Exceptions.EntityNotFoundException">Thrown when vehicle is not found.</exception>
    public async Task<UpdateVehicleStatusResult> HandleAsync(
        UpdateVehicleStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var (vehicleId, newStatus) = command;

        var vehicle = await Vehicles.GetByIdAsync(vehicleId, cancellationToken);
        var oldStatus = vehicle.Status;

        vehicle = vehicle.ChangeStatus(newStatus);
        await Vehicles.UpdateAsync(vehicle, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateVehicleStatusResult(
            vehicle.Id.Value,
            oldStatus.ToString(),
            vehicle.Status.ToString(),
            $"Vehicle status changed from {oldStatus} to {vehicle.Status}"
        );
    }
}
