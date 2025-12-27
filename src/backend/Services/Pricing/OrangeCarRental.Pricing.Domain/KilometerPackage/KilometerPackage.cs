using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.KilometerPackage;

/// <summary>
///     Kilometer package for German car rentals.
///     Defines daily kilometer limits and additional kilometer rates.
/// </summary>
public sealed record KilometerPackage : IValueObject
{
    /// <summary>
    ///     Standard package: 100 km/day included, 0.20€ per additional km.
    /// </summary>
    public static readonly KilometerPackage Limited100 = new(
        KilometerPackageType.Limited100,
        100,
        Money.EuroGross(0.20m));

    /// <summary>
    ///     Extended package: 200 km/day included, 0.20€ per additional km.
    /// </summary>
    public static readonly KilometerPackage Limited200 = new(
        KilometerPackageType.Limited200,
        200,
        Money.EuroGross(0.20m));

    /// <summary>
    ///     Unlimited kilometers - no additional charges.
    /// </summary>
    public static readonly KilometerPackage Unlimited = new(
        KilometerPackageType.Unlimited,
        null,
        null);

    private KilometerPackage(
        KilometerPackageType type,
        int? dailyLimitKm,
        Money? additionalKmRate)
    {
        Type = type;
        DailyLimitKm = dailyLimitKm;
        AdditionalKmRate = additionalKmRate;
    }

    /// <summary>
    ///     Gets the package type.
    /// </summary>
    public KilometerPackageType Type { get; }

    /// <summary>
    ///     Gets the daily kilometer limit (null for unlimited).
    /// </summary>
    public int? DailyLimitKm { get; }

    /// <summary>
    ///     Gets the rate per additional kilometer (null for unlimited).
    /// </summary>
    public Money? AdditionalKmRate { get; }

    /// <summary>
    ///     Gets whether this package has unlimited kilometers.
    /// </summary>
    public bool IsUnlimited => Type == KilometerPackageType.Unlimited;

    /// <summary>
    ///     Gets the total kilometer allowance for a rental period.
    /// </summary>
    /// <param name="days">Number of rental days.</param>
    /// <returns>Total kilometers allowed, or null for unlimited.</returns>
    public int? GetTotalAllowance(int days)
    {
        if (IsUnlimited || !DailyLimitKm.HasValue)
            return null;

        return DailyLimitKm.Value * days;
    }

    /// <summary>
    ///     Calculates the additional kilometer charge.
    /// </summary>
    /// <param name="totalDays">Number of rental days.</param>
    /// <param name="kilometersDriven">Total kilometers driven.</param>
    /// <returns>Additional charge for excess kilometers, or zero if within limit.</returns>
    public Money CalculateAdditionalCharge(int totalDays, int kilometersDriven)
    {
        if (IsUnlimited || !DailyLimitKm.HasValue || !AdditionalKmRate.HasValue)
            return Money.Zero(Currency.EUR);

        var allowance = GetTotalAllowance(totalDays) ?? 0;
        var excessKm = Math.Max(0, kilometersDriven - allowance);

        if (excessKm == 0)
            return Money.Zero(Currency.EUR);

        return AdditionalKmRate.Value * excessKm;
    }

    /// <summary>
    ///     Gets a package by type.
    /// </summary>
    public static KilometerPackage FromType(KilometerPackageType type) => type switch
    {
        KilometerPackageType.Limited100 => Limited100,
        KilometerPackageType.Limited200 => Limited200,
        KilometerPackageType.Unlimited => Unlimited,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown kilometer package type")
    };

    /// <summary>
    ///     Gets the German display name for this package.
    /// </summary>
    public string GetGermanDisplayName() => Type switch
    {
        KilometerPackageType.Limited100 => "100 km/Tag inklusive",
        KilometerPackageType.Limited200 => "200 km/Tag inklusive",
        KilometerPackageType.Unlimited => "Unbegrenzte Kilometer",
        _ => Type.ToString()
    };

    /// <summary>
    ///     Gets the English display name for this package.
    /// </summary>
    public string GetEnglishDisplayName() => Type switch
    {
        KilometerPackageType.Limited100 => "100 km/day included",
        KilometerPackageType.Limited200 => "200 km/day included",
        KilometerPackageType.Unlimited => "Unlimited kilometers",
        _ => Type.ToString()
    };

    public override string ToString() => GetEnglishDisplayName();
}
