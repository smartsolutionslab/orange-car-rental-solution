namespace SmartSolutionsLab.OrangeCarRental.Pricing.Application.Queries.CalculatePrice;

/// <summary>
///     Result of a price calculation query.
/// </summary>
public sealed record PriceCalculationResult
{
    /// <summary>
    ///     Vehicle category code.
    /// </summary>
    public required string CategoryCode { get; init; }

    /// <summary>
    ///     Number of rental days.
    /// </summary>
    public required int TotalDays { get; init; }

    /// <summary>
    ///     Daily rate (net amount in EUR).
    /// </summary>
    public required decimal DailyRateNet { get; init; }

    /// <summary>
    ///     Daily rate (gross amount in EUR, including VAT).
    /// </summary>
    public required decimal DailyRateGross { get; init; }

    /// <summary>
    ///     Total price (net amount in EUR).
    /// </summary>
    public required decimal TotalPriceNet { get; init; }

    /// <summary>
    ///     Total price (gross amount in EUR, including VAT).
    /// </summary>
    public required decimal TotalPriceGross { get; init; }

    /// <summary>
    ///     VAT amount in EUR.
    /// </summary>
    public required decimal VatAmount { get; init; }

    /// <summary>
    ///     VAT rate (e.g., 0.19 for 19%).
    /// </summary>
    public required decimal VatRate { get; init; }

    /// <summary>
    ///     Currency code (ISO 4217).
    /// </summary>
    public required string Currency { get; init; }

    /// <summary>
    ///     Pickup date.
    /// </summary>
    public required DateTime PickupDate { get; init; }

    /// <summary>
    ///     Return date.
    /// </summary>
    public required DateTime ReturnDate { get; init; }
}
