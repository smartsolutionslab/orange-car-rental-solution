using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleStatus;

/// <summary>
///     Command to update a vehicle's status.
/// </summary>
public sealed record UpdateVehicleStatusCommand(
    Guid VehicleId,
    VehicleStatus NewStatus
);
