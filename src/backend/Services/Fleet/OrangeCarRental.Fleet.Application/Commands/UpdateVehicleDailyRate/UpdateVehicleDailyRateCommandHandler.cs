using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleDailyRate;

/// <summary>
///     Handler for UpdateVehicleDailyRateCommand.
///     Updates a vehicle's daily rental rate with German VAT.
/// </summary>
public sealed class UpdateVehicleDailyRateCommandHandler(IFleetUnitOfWork unitOfWork)
    : ICommandHandler<UpdateVehicleDailyRateCommand, UpdateVehicleDailyRateResult>
{
    private IVehicleRepository Vehicles => unitOfWork.Vehicles;

    /// <summary>
    ///     Handles the update vehicle daily rate command.
    /// </summary>
    /// <param name="command">The command with vehicle ID and new daily rate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with old and new rate details.</returns>
    /// <exception cref="BuildingBlocks.Domain.Exceptions.EntityNotFoundException">Thrown when vehicle is not found.</exception>
    public async Task<UpdateVehicleDailyRateResult> HandleAsync(
        UpdateVehicleDailyRateCommand command,
        CancellationToken cancellationToken = default)
    {
        var (vehicleId, newDailyRate) = command;

        var vehicle = await Vehicles.GetByIdAsync(vehicleId, cancellationToken);
        var oldRate = vehicle.DailyRate;

        vehicle = vehicle.UpdateDailyRate(newDailyRate);
        await Vehicles.UpdateAsync(vehicle, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateVehicleDailyRateResult(
            vehicle.Id.Value,
            oldRate.NetAmount,
            oldRate.GrossAmount,
            vehicle.DailyRate.NetAmount,
            vehicle.DailyRate.GrossAmount,
            $"Daily rate updated from {oldRate.GrossAmount:C2} to {vehicle.DailyRate.GrossAmount:C2} (incl. 19% VAT)"
        );
    }
}
