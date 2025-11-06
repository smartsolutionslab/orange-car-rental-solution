using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.AddVehicleToFleet;

/// <summary>
///     Handler for AddVehicleToFleetCommand.
///     Creates a new vehicle and adds it to the fleet.
/// </summary>
public sealed class AddVehicleToFleetCommandHandler(IVehicleRepository vehicles)
{
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
        // Create new vehicle from command (value objects already validated)
        var vehicle = Vehicle.From(
            command.Name,
            command.Category,
            command.CurrentLocation,
            command.DailyRate,
            command.Seats,
            command.FuelType,
            command.TransmissionType
        );

        // Set optional details if provided
        if (command.LicensePlate is not null || command.Manufacturer is not null ||
            command.Model is not null || command.Year is not null || command.ImageUrl is not null)
        {
            vehicle = vehicle.SetDetails(
                command.Manufacturer,
                command.Model,
                command.Year,
                command.ImageUrl
            );
        }

        if (command.LicensePlate is not null)
        {
            vehicle = vehicle.SetLicensePlate(command.LicensePlate);
        }

        // Persist vehicle
        await vehicles.AddAsync(vehicle, cancellationToken);
        await vehicles.SaveChangesAsync(cancellationToken);

        // Return result
        return new AddVehicleToFleetResult(
            vehicle.Id.Value,
            vehicle.Name.Value,
            vehicle.Category.Code,
            vehicle.Status.ToString(),
            vehicle.CurrentLocation.Code.Value,
            vehicle.DailyRate.NetAmount,
            vehicle.DailyRate.VatAmount,
            vehicle.DailyRate.GrossAmount
        );
    }
}
