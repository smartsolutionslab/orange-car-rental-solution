using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleDailyRate;

/// <summary>
///     Command to update a vehicle's daily rental rate.
/// </summary>
public sealed record UpdateVehicleDailyRateCommand(
    Guid VehicleId,
    Money NewDailyRate
);
