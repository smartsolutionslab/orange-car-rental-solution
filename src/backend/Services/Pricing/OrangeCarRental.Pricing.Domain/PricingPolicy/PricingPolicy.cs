using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy.Events;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

/// <summary>
///     Pricing policy aggregate root.
///     Defines daily rates for vehicle categories with German market pricing (19% VAT).
///     IMMUTABLE: Properties can only be set during construction. Methods return new instances.
/// </summary>
public sealed class PricingPolicy : AggregateRoot<PricingPolicyIdentifier>
{
    // For EF Core
    private PricingPolicy()
    {
        CategoryCode = default!;
        DailyRate = default!;
    }

    private PricingPolicy(
        PricingPolicyIdentifier id,
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

    public CategoryCode CategoryCode { get; init; }
    public Money DailyRate { get; init; }
    public bool IsActive { get; init; }
    public DateTime EffectiveFrom { get; init; }
    public DateTime? EffectiveUntil { get; init; }

    // Optional: Location-specific pricing (for future enhancement)
    public LocationCode? LocationCode { get; init; }

    /// <summary>
    ///     Creates a new pricing policy for a vehicle category.
    /// </summary>
    public static PricingPolicy Create(
        CategoryCode categoryCode,
        Money dailyRate,
        DateTime? effectiveFrom = null,
        DateTime? effectiveUntil = null,
        LocationCode? locationCode = null)
    {
        return new PricingPolicy(
            PricingPolicyIdentifier.New(),
            categoryCode,
            dailyRate,
            effectiveFrom ?? DateTime.UtcNow,
            effectiveUntil,
            locationCode);
    }

    /// <summary>
    ///     Updates the daily rate for this pricing policy.
    ///     Returns a new instance with the updated rate (immutable pattern).
    /// </summary>
    public PricingPolicy UpdateDailyRate(Money newDailyRate)
    {
        if (newDailyRate.GrossAmount == DailyRate.GrossAmount)
            return this;

        var oldRate = DailyRate;
        var updated = CreateMutatedCopy(
            CategoryCode,
            newDailyRate,
            IsActive,
            EffectiveFrom,
            EffectiveUntil,
            LocationCode);

        updated.AddDomainEvent(new PricingPolicyUpdated(Id, CategoryCode, oldRate, newDailyRate));

        return updated;
    }

    /// <summary>
    ///     Deactivates this pricing policy.
    ///     Returns a new instance with deactivated state (immutable pattern).
    /// </summary>
    public PricingPolicy Deactivate()
    {
        if (!IsActive)
            return this;

        var now = DateTime.UtcNow;
        var updated = CreateMutatedCopy(
            CategoryCode,
            DailyRate,
            false,
            EffectiveFrom,
            now,
            LocationCode);

        updated.AddDomainEvent(new PricingPolicyDeactivated(Id, CategoryCode));

        return updated;
    }

    /// <summary>
    ///     Creates a copy of this instance with modified values (used internally for immutable updates).
    ///     Does not raise domain events - caller is responsible for that.
    /// </summary>
    private PricingPolicy CreateMutatedCopy(
        CategoryCode categoryCode,
        Money dailyRate,
        bool isActive,
        DateTime effectiveFrom,
        DateTime? effectiveUntil,
        LocationCode? locationCode)
    {
        return new PricingPolicy
        {
            Id = Id, // Copy Id from original instance
            CategoryCode = categoryCode,
            DailyRate = dailyRate,
            IsActive = isActive,
            EffectiveFrom = effectiveFrom,
            EffectiveUntil = effectiveUntil,
            LocationCode = locationCode
        };
    }

    /// <summary>
    ///     Checks if this policy is valid for the given date.
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
    ///     Calculates the total price for a rental period.
    /// </summary>
    public Money CalculatePrice(RentalPeriod period)
    {
        if (!IsValidFor(period.PickupDate.ToDateTime(TimeOnly.MinValue)))
            throw new InvalidOperationException("Pricing policy is not valid for the requested pickup date");

        return DailyRate * period.TotalDays;
    }
}
