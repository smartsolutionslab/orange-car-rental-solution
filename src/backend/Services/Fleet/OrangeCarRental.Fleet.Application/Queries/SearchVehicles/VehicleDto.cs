namespace SmartSolutionsLab.OrangeCarRental.Fleet.Application.Queries.SearchVehicles;

/// <summary>
///     Data transfer object for vehicle search results.
///     Contains all information needed to display a vehicle to customers with German pricing.
/// </summary>
public sealed record VehicleDto(
    Guid Id,
    string Name,
    string CategoryCode,
    string CategoryName,
    string LocationCode,
    string City,
    int Seats,
    string FuelType,
    string TransmissionType,
    decimal DailyRateNet,
    decimal DailyRateVat,
    decimal DailyRateGross,
    string Currency,
    string Status,
    string? LicensePlate,
    string? Manufacturer,
    string? Model,
    int? Year,
    string? ImageUrl);
