namespace SmartSolutionsLab.OrangeCarRental.Fleet.Api.Contracts;

/// <summary>
///     Request DTO for adding a new vehicle to the fleet.
///     Accepts primitives from HTTP requests and maps to AddVehicleToFleetCommand with value objects.
/// </summary>
public sealed record AddVehicleToFleetRequest(
    VehicleBasicInfoDto BasicInfo,
    VehicleSpecificationsDto Specifications,
    VehicleLocationAndPricingDto LocationAndPricing,
    VehicleRegistrationDto? Registration);

/// <summary>
///     Basic vehicle information.
/// </summary>
public sealed record VehicleBasicInfoDto(
    string Name,
    string? Manufacturer,
    string? Model,
    int? Year,
    string? ImageUrl);

/// <summary>
///     Vehicle technical specifications.
/// </summary>
public sealed record VehicleSpecificationsDto(
    string Category,
    int Seats,
    string FuelType,
    string TransmissionType);

/// <summary>
///     Vehicle location and pricing information.
/// </summary>
public sealed record VehicleLocationAndPricingDto(
    string LocationCode,
    decimal DailyRateNet);

/// <summary>
///     Vehicle registration information.
/// </summary>
public sealed record VehicleRegistrationDto(
    string? LicensePlate);
