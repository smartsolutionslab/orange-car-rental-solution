namespace SmartSolutionsLab.OrangeCarRental.Fleet.Api.Shared;

/// <summary>
///     Vehicle location and pricing information.
/// </summary>
public sealed record VehicleLocationAndPricingDto(
    string LocationCode,
    decimal DailyRateNet);
