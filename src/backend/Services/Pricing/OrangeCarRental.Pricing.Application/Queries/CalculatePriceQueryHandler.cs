using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Application.Queries;

/// <summary>
///     Handler for CalculatePriceQuery.
///     Calculates the rental price for a vehicle category and period using the active pricing policy.
/// </summary>
public sealed class CalculatePriceQueryHandler(IPricingPolicyRepository pricingPolicies)
    : IQueryHandler<CalculatePriceQuery, PriceCalculationResult>
{
    /// <summary>
    ///     Handles the price calculation query for a vehicle rental.
    /// </summary>
    /// <param name="query">The query containing category, dates, and optional location.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Price calculation result with net, VAT, and gross amounts.</returns>
    /// <exception cref="InvalidOperationException">Thrown when no active pricing policy is found for the category.</exception>
    public async Task<PriceCalculationResult> HandleAsync(
        CalculatePriceQuery query,
        CancellationToken cancellationToken = default)
    {
        var rentalPeriod = RentalPeriod.Of(query.PickupDate, query.ReturnDate);

        PricingPolicy? pricingPolicy = null;

        // Try to get location-specific pricing first, then fall back to general pricing
        if (query.LocationCode.HasValue)
        {
            try
            {
                pricingPolicy = await pricingPolicies.GetActivePolicyByCategoryAndLocationAsync(
                    query.CategoryCode,
                    query.LocationCode.Value,
                    cancellationToken);
            }
            catch (BuildingBlocks.Domain.Exceptions.EntityNotFoundException)
            {
                // Location-specific pricing not found, will fall back to general pricing
            }
        }

        // Fall back to general pricing if location-specific pricing not found
        if (pricingPolicy is null)
        {
            try
            {
                pricingPolicy = await pricingPolicies.GetActivePolicyByCategoryAsync(query.CategoryCode, cancellationToken);
            }
            catch (BuildingBlocks.Domain.Exceptions.EntityNotFoundException)
            {
                throw new InvalidOperationException(
                    $"No active pricing policy found for category '{query.CategoryCode.Value}'");
            }
        }

        var totalPrice = pricingPolicy.CalculatePrice(rentalPeriod);

        return new PriceCalculationResult(
            query.CategoryCode.Value,
            rentalPeriod.TotalDays,
            pricingPolicy.DailyRate.NetAmount,
            pricingPolicy.DailyRate.GrossAmount,
            totalPrice.NetAmount,
            totalPrice.GrossAmount,
            totalPrice.VatAmount,
            totalPrice.VatRate,
            totalPrice.Currency.Code,
            query.PickupDate,
            query.ReturnDate);
    }
}
