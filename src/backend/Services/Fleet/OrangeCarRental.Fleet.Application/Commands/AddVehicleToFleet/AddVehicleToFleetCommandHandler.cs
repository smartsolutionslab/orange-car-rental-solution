using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.AddVehicleToFleet;

/// <summary>
///     Handler for AddVehicleToFleetCommand.
///     Creates a new vehicle and adds it to the fleet.
/// </summary>
public sealed class AddVehicleToFleetCommandHandler(IFleetUnitOfWork unitOfWork)
    : ICommandHandler<AddVehicleToFleetCommand, AddVehicleToFleetResult>
{
    private IVehicleRepository Vehicles => unitOfWork.Vehicles;

    /// <summary>
    ///     Handles the add vehicle to fleet command.
    /// </summary>
    /// <param name="command">The command with vehicle data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Result with the new vehicle ID and details.</returns>
    public async Task<AddVehicleToFleetResult> HandleAsync(
        AddVehicleToFleetCommand command,
        CancellationToken cancellationToken = default)
    {
        var (name, category, currentLocation, dailyRate, seats, fuelType, transmissionType, licensePlate, manufacturer,
            vehicleModel, year, imageUrl) = command;

        var vehicle = Vehicle.From(
            name,
            category,
            currentLocation,
            dailyRate,
            seats,
            fuelType,
            transmissionType
        );
        // Set optional details if provided
        if (manufacturer is not null || vehicleModel is not null || year is not null || imageUrl is not null)
        {
            vehicle = vehicle.SetDetails(manufacturer, vehicleModel, year, imageUrl);
        }

        if (licensePlate.HasValue)
        {
            vehicle = vehicle.SetLicensePlate(licensePlate.Value);
        }
        await Vehicles.AddAsync(vehicle, cancellationToken);

        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new AddVehicleToFleetResult(
            vehicle.Id.Value,
            vehicle.Name.Value,
            vehicle.Category.Code,
            vehicle.Status.ToString(),
            vehicle.CurrentLocationCode.Value,
            vehicle.DailyRate.NetAmount,
            vehicle.DailyRate.VatAmount,
            vehicle.DailyRate.GrossAmount
        );
    }
}
