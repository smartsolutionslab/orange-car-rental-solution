using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Application.Queries.CalculatePrice;

/// <summary>
///     Handler for CalculatePriceQuery.
///     Calculates the rental price for a vehicle category and period using the active pricing policy.
/// </summary>
public sealed class CalculatePriceQueryHandler(IPricingPolicyRepository pricingPolicies)
    : IQueryHandler<CalculatePriceQuery, PriceCalculationResult>
{
    public async Task<PriceCalculationResult> HandleAsync(
        CalculatePriceQuery query,
        CancellationToken cancellationToken = default)
    {
        var rentalPeriod = RentalPeriod.Of(query.PickupDate, query.ReturnDate);

        // Try to get location-specific pricing first, then fall back to general pricing
        var pricingPolicy = query.LocationCode.HasValue
            ? await pricingPolicies.GetActivePolicyByCategoryAndLocationAsync(
                query.CategoryCode,
                query.LocationCode.Value,
                cancellationToken)
            : null;

        // Fall back to general pricing if location-specific pricing not found
        pricingPolicy ??= await pricingPolicies.GetActivePolicyByCategoryAsync(query.CategoryCode, cancellationToken);

        if (pricingPolicy is null)
        {
            throw new InvalidOperationException(
                $"No active pricing policy found for category '{query.CategoryCode.Value}'");
        }

        var totalPrice = pricingPolicy.CalculatePrice(rentalPeriod);

        return new PriceCalculationResult
        {
            CategoryCode = query.CategoryCode.Value,
            TotalDays = rentalPeriod.TotalDays,
            DailyRateNet = pricingPolicy.DailyRate.NetAmount,
            DailyRateGross = pricingPolicy.DailyRate.GrossAmount,
            TotalPriceNet = totalPrice.NetAmount,
            TotalPriceGross = totalPrice.GrossAmount,
            VatAmount = totalPrice.VatAmount,
            VatRate = totalPrice.VatRate,
            Currency = totalPrice.Currency.Code,
            PickupDate = query.PickupDate,
            ReturnDate = query.ReturnDate
        };
    }
}
