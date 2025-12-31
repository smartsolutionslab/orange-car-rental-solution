namespace SmartSolutionsLab.OrangeCarRental.Fleet.Api.Shared;

/// <summary>
///     Vehicle technical specifications.
/// </summary>
public sealed record VehicleSpecificationsDto(
    string Category,
    int Seats,
    string FuelType,
    string TransmissionType);
