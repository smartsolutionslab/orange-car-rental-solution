namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands.UpdateVehicleLocation;

/// <summary>
///     Result of updating a vehicle's location.
/// </summary>
public sealed record UpdateVehicleLocationResult(
    Guid VehicleId,
    string OldLocationCode,
    string OldLocationName,
    string NewLocationCode,
    string NewLocationName,
    string Message
);
