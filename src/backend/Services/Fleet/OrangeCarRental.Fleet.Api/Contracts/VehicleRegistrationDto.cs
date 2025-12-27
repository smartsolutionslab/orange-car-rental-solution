namespace SmartSolutionsLab.OrangeCarRental.Fleet.Api.Contracts;

/// <summary>
///     Vehicle registration information.
/// </summary>
public sealed record VehicleRegistrationDto(
    string? LicensePlate);
