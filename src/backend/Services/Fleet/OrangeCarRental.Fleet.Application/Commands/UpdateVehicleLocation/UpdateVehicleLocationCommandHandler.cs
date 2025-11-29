using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleLocation;

/// <summary>
///     Handler for UpdateVehicleLocationCommand.
///     Moves a vehicle to a different rental location.
/// </summary>
public sealed class UpdateVehicleLocationCommandHandler(IFleetUnitOfWork unitOfWork)
    : ICommandHandler<UpdateVehicleLocationCommand, UpdateVehicleLocationResult>
{
    /// <summary>
    ///     Handles the update vehicle location command.
    /// </summary>
    /// <param name="command">The command with vehicle ID and new location code.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with old and new location codes.</returns>
    /// <exception cref="BuildingBlocks.Domain.Exceptions.EntityNotFoundException">Thrown when vehicle is not found.</exception>
    /// <exception cref="InvalidOperationException">Thrown when vehicle cannot be moved.</exception>
    public async Task<UpdateVehicleLocationResult> HandleAsync(
        UpdateVehicleLocationCommand command,
        CancellationToken cancellationToken = default)
    {
        var (vehicleId, newLocationCode) = command;

        var vehicles = unitOfWork.Vehicles;
        var vehicle = await vehicles.GetByIdAsync(vehicleId, cancellationToken);
        var oldLocationCode = vehicle.CurrentLocationCode;

        vehicle = vehicle.MoveToLocation(newLocationCode);
        await vehicles.UpdateAsync(vehicle, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateVehicleLocationResult(
            vehicle.Id.Value,
            oldLocationCode.Value,
            string.Empty, // Location name will be fetched by client if needed
            vehicle.CurrentLocationCode.Value,
            string.Empty, // Location name will be fetched by client if needed
            $"Vehicle moved from {oldLocationCode.Value} to {vehicle.CurrentLocationCode.Value}"
        );
    }
}
