using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Application.Queries.CalculatePrice;

/// <summary>
/// Handler for CalculatePriceQuery.
/// Calculates the rental price for a vehicle category and period using the active pricing policy.
/// </summary>
public sealed class CalculatePriceQueryHandler(IPricingPolicyRepository repository)
{
    public async Task<PriceCalculationResult> HandleAsync(
        CalculatePriceQuery query,
        CancellationToken cancellationToken = default)
    {
        var categoryCode = CategoryCode.Of(query.CategoryCode);
        var rentalPeriod = RentalPeriod.Of(query.PickupDate, query.ReturnDate);

        // Try to get location-specific pricing first, then fall back to general pricing
        var pricingPolicy = query.LocationCode != null
            ? await repository.GetActivePolicyByCategoryAndLocationAsync(
                categoryCode,
                LocationCode.Of(query.LocationCode),
                cancellationToken)
            : null;

        // Fall back to general pricing if location-specific pricing not found
        pricingPolicy ??= await repository.GetActivePolicyByCategoryAsync(categoryCode, cancellationToken);

        if (pricingPolicy is null)
        {
            throw new InvalidOperationException(
                $"No active pricing policy found for category '{query.CategoryCode}'");
        }

        var totalPrice = pricingPolicy.CalculatePrice(rentalPeriod);

        return new PriceCalculationResult
        {
            CategoryCode = query.CategoryCode,
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
