namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.DTOs;

/// <summary>
///     DTO for price calculation results from the Pricing API.
/// </summary>
public sealed record PriceCalculationDto(
    string CategoryCode,
    int TotalDays,
    decimal DailyRateNet,
    decimal DailyRateGross,
    decimal TotalPriceNet,
    decimal TotalPriceGross,
    decimal VatAmount,
    decimal VatRate,
    string Currency);
