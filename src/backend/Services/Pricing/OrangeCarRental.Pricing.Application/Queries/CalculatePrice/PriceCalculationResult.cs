namespace SmartSolutionsLab.OrangeCarRental.Pricing.Application.Queries.CalculatePrice;

/// <summary>
///     Result of a price calculation query.
/// </summary>
public sealed record PriceCalculationResult(
    string CategoryCode,
    int TotalDays,
    decimal DailyRateNet,
    decimal DailyRateGross,
    decimal TotalPriceNet,
    decimal TotalPriceGross,
    decimal VatAmount,
    decimal VatRate,
    string Currency,
    DateOnly PickupDate,
    DateOnly ReturnDate);
