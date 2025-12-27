namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleStatus;

/// <summary>
///     Result of updating a vehicle's status.
/// </summary>
public sealed record UpdateVehicleStatusResult(
    Guid VehicleId,
    string OldStatus,
    string NewStatus,
    string Message
);
