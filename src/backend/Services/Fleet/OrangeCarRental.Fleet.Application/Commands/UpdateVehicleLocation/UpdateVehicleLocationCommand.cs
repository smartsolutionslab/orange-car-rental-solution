using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleLocation;

/// <summary>
///     Command to move a vehicle to a different location.
/// </summary>
public sealed record UpdateVehicleLocationCommand(
    Guid VehicleId,
    Location NewLocation
);
