using System.Globalization;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.InsurancePackage;

/// <summary>
///     Insurance package for German car rentals.
///     Defines coverage type, deductible (Selbstbeteiligung), and daily surcharge.
/// </summary>
public sealed record InsurancePackage : IValueObject
{
    private InsurancePackage(
        InsuranceType type,
        Money deductible,
        Money dailySurcharge,
        bool includesTheftProtection,
        bool includesGlassAndTires,
        bool includesPersonalAccident)
    {
        Type = type;
        Deductible = deductible;
        DailySurcharge = dailySurcharge;
        IncludesTheftProtection = includesTheftProtection;
        IncludesGlassAndTires = includesGlassAndTires;
        IncludesPersonalAccident = includesPersonalAccident;
    }

    /// <summary>
    ///     Gets the insurance type.
    /// </summary>
    public InsuranceType Type { get; }

    /// <summary>
    ///     Gets the deductible amount (Selbstbeteiligung).
    ///     This is the maximum out-of-pocket cost for the renter in case of damage.
    /// </summary>
    public Money Deductible { get; }

    /// <summary>
    ///     Gets the daily surcharge for this insurance package.
    /// </summary>
    public Money DailySurcharge { get; }

    /// <summary>
    ///     Gets whether theft protection (Diebstahlschutz) is included.
    /// </summary>
    public bool IncludesTheftProtection { get; }

    /// <summary>
    ///     Gets whether glass and tire damage coverage is included.
    /// </summary>
    public bool IncludesGlassAndTires { get; }

    /// <summary>
    ///     Gets whether personal accident insurance (Insassenunfallversicherung) is included.
    /// </summary>
    public bool IncludesPersonalAccident { get; }

    /// <summary>
    ///     Gets whether this package has zero deductible.
    /// </summary>
    public bool IsZeroDeductible => Deductible.GrossAmount == 0;

    /// <summary>
    ///     Calculates the total insurance cost for a rental period.
    /// </summary>
    /// <param name="rentalDays">Number of rental days.</param>
    /// <returns>Total insurance surcharge.</returns>
    public Money CalculateCost(int rentalDays)
    {
        if (rentalDays <= 0)
            return Money.Zero(Currency.EUR);

        return DailySurcharge * rentalDays;
    }

    private static readonly CultureInfo GermanCulture = new("de-DE");

    /// <summary>
    ///     Gets the German display name for this package.
    /// </summary>
    public string GetGermanDisplayName() => Type switch
    {
        InsuranceType.Haftpflicht => "Haftpflichtversicherung (Basis)",
        InsuranceType.Teilkasko => $"Teilkasko (SB {Deductible.GrossAmount.ToString("N0", GermanCulture)}€)",
        InsuranceType.Vollkasko => $"Vollkasko (SB {Deductible.GrossAmount.ToString("N0", GermanCulture)}€)",
        InsuranceType.VollkaskoZeroDeductible => "Vollkasko ohne Selbstbeteiligung",
        _ => Type.ToString()
    };

    /// <summary>
    ///     Gets the English display name for this package.
    /// </summary>
    public string GetEnglishDisplayName() => Type switch
    {
        InsuranceType.Haftpflicht => "Liability Only (Basic)",
        InsuranceType.Teilkasko => $"Partial Coverage (€{Deductible.GrossAmount:N0} excess)",
        InsuranceType.Vollkasko => $"Comprehensive (€{Deductible.GrossAmount:N0} excess)",
        InsuranceType.VollkaskoZeroDeductible => "Comprehensive Zero Excess",
        _ => Type.ToString()
    };

    /// <summary>
    ///     Gets a description of what this package covers.
    /// </summary>
    public string GetCoverageDescription() => Type switch
    {
        InsuranceType.Haftpflicht =>
            "Covers damage to third parties only. You are liable for damage to the rental vehicle.",
        InsuranceType.Teilkasko =>
            "Covers theft, fire, glass damage, natural disasters, and animal collisions. Does not cover own-fault accidents.",
        InsuranceType.Vollkasko =>
            "Full coverage including own-fault accidents, theft, fire, and glass damage.",
        InsuranceType.VollkaskoZeroDeductible =>
            "Complete protection with no out-of-pocket costs in case of damage.",
        _ => string.Empty
    };

    /// <summary>
    ///     Gets the German coverage description.
    /// </summary>
    public string GetGermanCoverageDescription() => Type switch
    {
        InsuranceType.Haftpflicht =>
            "Deckt nur Schäden an Dritten. Sie haften für Schäden am Mietfahrzeug.",
        InsuranceType.Teilkasko =>
            "Deckt Diebstahl, Brand, Glasschäden, Naturereignisse und Wildunfälle. Keine Deckung bei selbstverschuldeten Unfällen.",
        InsuranceType.Vollkasko =>
            "Vollständige Deckung einschließlich selbstverschuldeter Unfälle, Diebstahl, Brand und Glasschäden.",
        InsuranceType.VollkaskoZeroDeductible =>
            "Rundum-Schutz ohne Zuzahlung im Schadensfall.",
        _ => string.Empty
    };

    public override string ToString() => $"{GetEnglishDisplayName()} ({DailySurcharge}/day)";

    // Predefined German market insurance packages

    /// <summary>
    ///     Basic liability only - included in base rental price.
    ///     Renter is fully liable for damage to the rental vehicle.
    /// </summary>
    public static readonly InsurancePackage Basic = new(
        InsuranceType.Haftpflicht,
        Money.EuroGross(2500.00m), // High liability for vehicle damage
        Money.Zero(Currency.EUR), // Included in base price
        includesTheftProtection: false,
        includesGlassAndTires: false,
        includesPersonalAccident: false);

    /// <summary>
    ///     Standard partial coverage (Teilkasko) with 1000€ deductible.
    ///     Common default for German rentals.
    /// </summary>
    public static readonly InsurancePackage Standard = new(
        InsuranceType.Teilkasko,
        Money.EuroGross(1000.00m),
        Money.EuroGross(8.00m),
        includesTheftProtection: true,
        includesGlassAndTires: false,
        includesPersonalAccident: false);

    /// <summary>
    ///     Comprehensive coverage (Vollkasko) with 500€ deductible.
    ///     Good balance of protection and cost.
    /// </summary>
    public static readonly InsurancePackage Comfort = new(
        InsuranceType.Vollkasko,
        Money.EuroGross(500.00m),
        Money.EuroGross(15.00m),
        includesTheftProtection: true,
        includesGlassAndTires: true,
        includesPersonalAccident: false);

    /// <summary>
    ///     Premium comprehensive coverage (Vollkasko) with zero deductible.
    ///     Maximum protection with no out-of-pocket costs.
    /// </summary>
    public static readonly InsurancePackage Premium = new(
        InsuranceType.VollkaskoZeroDeductible,
        Money.Zero(Currency.EUR),
        Money.EuroGross(25.00m),
        includesTheftProtection: true,
        includesGlassAndTires: true,
        includesPersonalAccident: true);

    /// <summary>
    ///     Gets all available insurance packages.
    /// </summary>
    public static IReadOnlyList<InsurancePackage> GetAllPackages() =>
    [
        Basic,
        Standard,
        Comfort,
        Premium
    ];

    /// <summary>
    ///     Gets an insurance package by type.
    /// </summary>
    public static InsurancePackage FromType(InsuranceType type) => type switch
    {
        InsuranceType.Haftpflicht => Basic,
        InsuranceType.Teilkasko => Standard,
        InsuranceType.Vollkasko => Comfort,
        InsuranceType.VollkaskoZeroDeductible => Premium,
        _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown insurance type")
    };

    /// <summary>
    ///     Creates a custom insurance package with specific deductible.
    /// </summary>
    /// <param name="type">The insurance type.</param>
    /// <param name="deductible">The deductible amount.</param>
    /// <param name="dailySurcharge">The daily surcharge.</param>
    /// <returns>A custom insurance package.</returns>
    public static InsurancePackage CreateCustom(
        InsuranceType type,
        Money deductible,
        Money dailySurcharge)
    {
        var basePackage = FromType(type);
        return new InsurancePackage(
            type,
            deductible,
            dailySurcharge,
            basePackage.IncludesTheftProtection,
            basePackage.IncludesGlassAndTires,
            basePackage.IncludesPersonalAccident);
    }

    /// <summary>
    ///     Compares two packages and returns which offers better protection.
    /// </summary>
    public int CompareCoverageTo(InsurancePackage other)
    {
        // Higher type = better coverage
        return Type.CompareTo(other.Type);
    }
}
