using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;
using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleLocation;

/// <summary>
///     Command to move a vehicle to a different location.
/// </summary>
public sealed record UpdateVehicleLocationCommand(
    VehicleIdentifier VehicleId,
    Location NewLocation
);
