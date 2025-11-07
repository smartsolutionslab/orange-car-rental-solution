using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleDailyRate;

/// <summary>
///     Command to update a vehicle's daily rental rate.
/// </summary>
public sealed record UpdateVehicleDailyRateCommand(
    VehicleIdentifier VehicleId,
    Money NewDailyRate
);
