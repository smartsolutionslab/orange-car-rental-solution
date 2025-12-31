namespace SmartSolutionsLab.OrangeCarRental.Pricing.Api.Requests;

/// <summary>
///     Request DTO for calculating rental price.
/// </summary>
public sealed record CalculatePriceRequest(
    string CategoryCode,
    DateOnly PickupDate,
    DateOnly ReturnDate,
    string? LocationCode);
