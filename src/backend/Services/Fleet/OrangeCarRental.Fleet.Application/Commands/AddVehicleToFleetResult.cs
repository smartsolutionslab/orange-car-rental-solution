namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands;

/// <summary>
///     Result of adding a vehicle to the fleet.
/// </summary>
public sealed record AddVehicleToFleetResult(
    Guid VehicleId,
    string Name,
    string Category,
    string Status,
    string LocationCode,
    decimal DailyRateNet,
    decimal DailyRateVat,
    decimal DailyRateGross
);
