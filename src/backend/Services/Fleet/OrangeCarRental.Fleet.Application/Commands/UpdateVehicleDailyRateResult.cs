namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Commands;

/// <summary>
///     Result of updating a vehicle's daily rental rate.
/// </summary>
public sealed record UpdateVehicleDailyRateResult(
    Guid VehicleId,
    decimal OldDailyRateNet,
    decimal OldDailyRateGross,
    decimal NewDailyRateNet,
    decimal NewDailyRateGross,
    string Message
);
