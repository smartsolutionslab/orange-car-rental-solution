namespace SmartSolutionsLab.OrangeCarRental.Fleet.Api.Contracts;

/// <summary>
///     Basic vehicle information.
/// </summary>
public sealed record VehicleBasicInfoDto(
    string Name,
    string? Manufacturer,
    string? Model,
    int? Year,
    string? ImageUrl);
