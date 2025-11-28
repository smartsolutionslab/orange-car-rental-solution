using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Tests.Builders;

/// <summary>
/// Test data builder for PricingPolicy aggregates.
/// Uses fluent API with sensible defaults.
/// </summary>
public class PricingPolicyBuilder
{
    private CategoryCode _categoryCode = CategoryCode.From("KLEIN");
    private Money _dailyRate = Money.Euro(49.99m);
    private DateTime? _effectiveFrom;
    private DateTime? _effectiveUntil;
    private LocationCode? _locationCode;

    /// <summary>
    /// Sets the category code.
    /// </summary>
    public PricingPolicyBuilder WithCategory(string categoryCode)
    {
        _categoryCode = CategoryCode.From(categoryCode);
        return this;
    }

    /// <summary>
    /// Sets the category as Kleinwagen (compact).
    /// </summary>
    public PricingPolicyBuilder AsCompact()
    {
        _categoryCode = CategoryCode.From("KLEIN");
        _dailyRate = Money.Euro(40.00m);
        return this;
    }

    /// <summary>
    /// Sets the category as Kompaktklasse.
    /// </summary>
    public PricingPolicyBuilder AsCompactClass()
    {
        _categoryCode = CategoryCode.From("KOMPAKT");
        _dailyRate = Money.Euro(50.00m);
        return this;
    }

    /// <summary>
    /// Sets the category as Mittelklasse (mid-size).
    /// </summary>
    public PricingPolicyBuilder AsMidSize()
    {
        _categoryCode = CategoryCode.From("MITTEL");
        _dailyRate = Money.Euro(60.00m);
        return this;
    }

    /// <summary>
    /// Sets the category as SUV.
    /// </summary>
    public PricingPolicyBuilder AsSuv()
    {
        _categoryCode = CategoryCode.From("SUV");
        _dailyRate = Money.Euro(90.00m);
        return this;
    }

    /// <summary>
    /// Sets the category as Oberklasse (upper class).
    /// </summary>
    public PricingPolicyBuilder AsUpperClass()
    {
        _categoryCode = CategoryCode.From("OBER");
        _dailyRate = Money.Euro(80.00m);
        return this;
    }

    /// <summary>
    /// Sets the category as Luxusklasse (luxury).
    /// </summary>
    public PricingPolicyBuilder AsLuxury()
    {
        _categoryCode = CategoryCode.From("LUXUS");
        _dailyRate = Money.Euro(150.00m);
        return this;
    }

    /// <summary>
    /// Sets the daily rate.
    /// </summary>
    public PricingPolicyBuilder WithDailyRate(decimal amount)
    {
        _dailyRate = Money.Euro(amount);
        return this;
    }

    /// <summary>
    /// Sets the effective from date.
    /// </summary>
    public PricingPolicyBuilder EffectiveFrom(DateTime effectiveFrom)
    {
        _effectiveFrom = effectiveFrom;
        return this;
    }

    /// <summary>
    /// Sets the effective until date.
    /// </summary>
    public PricingPolicyBuilder EffectiveUntil(DateTime effectiveUntil)
    {
        _effectiveUntil = effectiveUntil;
        return this;
    }

    /// <summary>
    /// Sets the effective date range.
    /// </summary>
    public PricingPolicyBuilder EffectiveBetween(DateTime from, DateTime until)
    {
        _effectiveFrom = from;
        _effectiveUntil = until;
        return this;
    }

    /// <summary>
    /// Sets the policy to be effective starting from 10 days ago.
    /// </summary>
    public PricingPolicyBuilder CurrentlyActive()
    {
        _effectiveFrom = DateTime.UtcNow.AddDays(-10);
        _effectiveUntil = DateTime.UtcNow.AddDays(10);
        return this;
    }

    /// <summary>
    /// Sets the policy to be effective in the future.
    /// </summary>
    public PricingPolicyBuilder FuturePolicy()
    {
        _effectiveFrom = DateTime.UtcNow.AddDays(10);
        _effectiveUntil = DateTime.UtcNow.AddDays(30);
        return this;
    }

    /// <summary>
    /// Sets the policy to be expired.
    /// </summary>
    public PricingPolicyBuilder ExpiredPolicy()
    {
        _effectiveFrom = DateTime.UtcNow.AddDays(-30);
        _effectiveUntil = DateTime.UtcNow.AddDays(-10);
        return this;
    }

    /// <summary>
    /// Sets the location code for location-specific pricing.
    /// </summary>
    public PricingPolicyBuilder ForLocation(string locationCode)
    {
        _locationCode = LocationCode.From(locationCode);
        return this;
    }

    /// <summary>
    /// Builds the pricing policy in Active status.
    /// </summary>
    public PricingPolicy Build()
    {
        return PricingPolicy.Create(
            _categoryCode,
            _dailyRate,
            _effectiveFrom ?? DateTime.UtcNow,
            _effectiveUntil,
            _locationCode);
    }

    /// <summary>
    /// Builds the pricing policy and immediately deactivates it.
    /// </summary>
    public PricingPolicy BuildDeactivated()
    {
        var policy = Build();
        return policy.Deactivate();
    }

    /// <summary>
    /// Creates a new PricingPolicyBuilder with default values.
    /// </summary>
    public static PricingPolicyBuilder Default() => new();

    /// <summary>
    /// Creates a budget-friendly policy (Kleinwagen at low rate).
    /// </summary>
    public static PricingPolicyBuilder Budget() => new PricingPolicyBuilder()
        .AsCompact()
        .WithDailyRate(35.00m);

    /// <summary>
    /// Creates a premium policy (Luxus at high rate).
    /// </summary>
    public static PricingPolicyBuilder Premium() => new PricingPolicyBuilder()
        .AsLuxury()
        .WithDailyRate(200.00m);

    /// <summary>
    /// Creates a family-friendly policy (Mittelklasse or SUV).
    /// </summary>
    public static PricingPolicyBuilder Family() => new PricingPolicyBuilder()
        .AsMidSize()
        .WithDailyRate(65.00m);
}
