namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;

/// <summary>
///     DTO for price calculation results from the Pricing API.
/// </summary>
public sealed record PriceCalculationDto
{
    public required string CategoryCode { get; init; }
    public required int TotalDays { get; init; }
    public required decimal DailyRateNet { get; init; }
    public required decimal DailyRateGross { get; init; }
    public required decimal TotalPriceNet { get; init; }
    public required decimal TotalPriceGross { get; init; }
    public required decimal VatAmount { get; init; }
    public required decimal VatRate { get; init; }
    public required string Currency { get; init; }
}
