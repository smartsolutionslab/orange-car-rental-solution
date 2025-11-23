using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Application.Queries.CalculatePrice;

/// <summary>
///     Query to calculate the rental price for a vehicle category and period.
/// </summary>
public sealed record CalculatePriceQuery(
    CategoryCode CategoryCode,
    DateOnly PickupDate,
    DateOnly ReturnDate,
    LocationCode? LocationCode) : IQuery<PriceCalculationResult>;
