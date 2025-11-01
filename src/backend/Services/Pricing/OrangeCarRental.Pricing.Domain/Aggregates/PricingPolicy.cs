using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.Events;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.Aggregates;

/// <summary>
/// Pricing policy aggregate root.
/// Defines daily rates for vehicle categories with German market pricing (19% VAT).
/// </summary>
public sealed class PricingPolicy : AggregateRoot<PricingPolicyId>
{
    public CategoryCode CategoryCode { get; private set; }
    public Money DailyRate { get; private set; }
    public bool IsActive { get; private set; }
    public DateTime EffectiveFrom { get; private set; }
    public DateTime? EffectiveUntil { get; private set; }

    // Optional: Location-specific pricing (for future enhancement)
    public LocationCode? LocationCode { get; private set; }

    // For EF Core
    private PricingPolicy()
    {
        CategoryCode = default!;
        DailyRate = default!;
    }

    private PricingPolicy(
        PricingPolicyId id,
        CategoryCode categoryCode,
        Money dailyRate,
        DateTime effectiveFrom,
        DateTime? effectiveUntil = null,
        LocationCode? locationCode = null)
        : base(id)
    {
        CategoryCode = categoryCode;
        DailyRate = dailyRate;
        IsActive = true;
        EffectiveFrom = effectiveFrom;
        EffectiveUntil = effectiveUntil;
        LocationCode = locationCode;

        AddDomainEvent(new PricingPolicyCreated(Id, CategoryCode, DailyRate, EffectiveFrom));
    }

    /// <summary>
    /// Creates a new pricing policy for a vehicle category.
    /// </summary>
    public static PricingPolicy Create(
        CategoryCode categoryCode,
        Money dailyRate,
        DateTime? effectiveFrom = null,
        DateTime? effectiveUntil = null,
        LocationCode? locationCode = null)
    {
        return new PricingPolicy(
            PricingPolicyId.New(),
            categoryCode,
            dailyRate,
            effectiveFrom ?? DateTime.UtcNow,
            effectiveUntil,
            locationCode);
    }

    /// <summary>
    /// Updates the daily rate for this pricing policy.
    /// </summary>
    public void UpdateDailyRate(Money newDailyRate)
    {
        if (newDailyRate.GrossAmount == DailyRate.GrossAmount)
            return;

        var oldRate = DailyRate;
        DailyRate = newDailyRate;

        AddDomainEvent(new PricingPolicyUpdated(Id, CategoryCode, oldRate, newDailyRate));
    }

    /// <summary>
    /// Deactivates this pricing policy.
    /// </summary>
    public void Deactivate()
    {
        if (!IsActive)
            return;

        IsActive = false;
        EffectiveUntil = DateTime.UtcNow;

        AddDomainEvent(new PricingPolicyDeactivated(Id, CategoryCode));
    }

    /// <summary>
    /// Checks if this policy is valid for the given date.
    /// </summary>
    public bool IsValidFor(DateTime date)
    {
        if (!IsActive)
            return false;

        if (date < EffectiveFrom)
            return false;

        if (EffectiveUntil.HasValue && date >= EffectiveUntil.Value)
            return false;

        return true;
    }

    /// <summary>
    /// Calculates the total price for a rental period.
    /// </summary>
    public Money CalculatePrice(RentalPeriod period)
    {
        if (!IsValidFor(period.PickupDate))
            throw new InvalidOperationException("Pricing policy is not valid for the requested pickup date");

        return DailyRate * period.TotalDays;
    }
}
