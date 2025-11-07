using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Application.Queries.CalculatePrice;

/// <summary>
///     Query to calculate the rental price for a vehicle category and period.
/// </summary>
public sealed record CalculatePriceQuery : IQuery<PriceCalculationResult>
{
    /// <summary>
    ///     Vehicle category code (e.g., "KLEIN", "MITTEL", "SUV").
    /// </summary>
    public required CategoryCode CategoryCode { get; init; }

    /// <summary>
    ///     Pickup date.
    /// </summary>
    public required DateTime PickupDate { get; init; }

    /// <summary>
    ///     Return date.
    /// </summary>
    public required DateTime ReturnDate { get; init; }

    /// <summary>
    ///     Optional location code for location-specific pricing.
    /// </summary>
    public LocationCode? LocationCode { get; init; }
}
