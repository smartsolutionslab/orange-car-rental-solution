using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.CrossBorder;

/// <summary>
///     Cross-border travel policy for German car rentals.
///     Defines which countries are allowed and any applicable surcharges.
/// </summary>
public sealed class CrossBorderPolicy : IValueObject
{
    private readonly Dictionary<string, CountryTravelRule> _countryRules;

    private CrossBorderPolicy(IEnumerable<CountryTravelRule> rules)
    {
        _countryRules = rules.ToDictionary(r => r.Country.Value, r => r);
    }

    /// <summary>
    ///     Gets the rule for a specific country.
    /// </summary>
    public CountryTravelRule? GetRule(CountryCode country)
    {
        return _countryRules.GetValueOrDefault(country.Value);
    }

    /// <summary>
    ///     Checks if travel to a country is allowed.
    /// </summary>
    public bool IsAllowed(CountryCode country)
    {
        var rule = GetRule(country);
        return rule?.IsAllowed ?? false;
    }

    /// <summary>
    ///     Checks if travel to a country requires a permit.
    /// </summary>
    public bool RequiresPermit(CountryCode country)
    {
        var rule = GetRule(country);
        return rule?.RequiresAdvanceBooking ?? true;
    }

    /// <summary>
    ///     Gets the daily surcharge for a country.
    /// </summary>
    public Money? GetDailySurcharge(CountryCode country)
    {
        var rule = GetRule(country);
        return rule?.DailySurcharge;
    }

    /// <summary>
    ///     Calculates the total cross-border surcharge for a trip.
    /// </summary>
    /// <param name="countries">Countries to be visited.</param>
    /// <param name="rentalDays">Number of rental days.</param>
    /// <returns>Total surcharge for all cross-border travel.</returns>
    public Money CalculateTotalSurcharge(IEnumerable<CountryCode> countries, int rentalDays)
    {
        var total = Money.Zero(Currency.EUR);

        foreach (var country in countries.Distinct())
        {
            var surcharge = GetDailySurcharge(country);
            if (surcharge.HasValue)
            {
                total += surcharge.Value * rentalDays;
            }
        }

        return total;
    }

    /// <summary>
    ///     Validates a list of destination countries.
    /// </summary>
    /// <param name="countries">Countries to validate.</param>
    /// <returns>Validation result with any issues.</returns>
    public CrossBorderValidationResult Validate(IEnumerable<CountryCode> countries)
    {
        var issues = new List<string>();
        var requiresPermit = false;
        var requiresAdditionalInsurance = false;

        foreach (var country in countries.Distinct())
        {
            var rule = GetRule(country);

            if (rule is null)
            {
                issues.Add($"Travel to {country.GetEnglishName()} is not covered by this policy");
                continue;
            }

            switch (rule.Restriction)
            {
                case TravelRestriction.Prohibited:
                    issues.Add($"Travel to {country.GetEnglishName()} is not permitted: {rule.RestrictionReason}");
                    break;
                case TravelRestriction.Restricted:
                    issues.Add($"Travel to {country.GetEnglishName()} requires special approval: {rule.RestrictionReason}");
                    break;
                case TravelRestriction.PermitRequired:
                    requiresPermit = true;
                    break;
            }

            if (rule.RequiresAdditionalInsurance)
            {
                requiresAdditionalInsurance = true;
            }
        }

        return new CrossBorderValidationResult(
            issues.Count == 0,
            issues,
            requiresPermit,
            requiresAdditionalInsurance);
    }

    /// <summary>
    ///     Gets all allowed countries.
    /// </summary>
    public IReadOnlyList<CountryCode> GetAllowedCountries()
    {
        return _countryRules.Values
            .Where(r => r.IsAllowed)
            .Select(r => r.Country)
            .ToList();
    }

    /// <summary>
    ///     Gets all countries requiring a permit.
    /// </summary>
    public IReadOnlyList<CountryCode> GetPermitRequiredCountries()
    {
        return _countryRules.Values
            .Where(r => r.Restriction == TravelRestriction.PermitRequired)
            .Select(r => r.Country)
            .ToList();
    }

    /// <summary>
    ///     Gets all prohibited countries.
    /// </summary>
    public IReadOnlyList<CountryCode> GetProhibitedCountries()
    {
        return _countryRules.Values
            .Where(r => r.Restriction == TravelRestriction.Prohibited)
            .Select(r => r.Country)
            .ToList();
    }

    /// <summary>
    ///     Standard German car rental cross-border policy.
    ///     Based on typical German rental company policies.
    /// </summary>
    public static CrossBorderPolicy StandardGermanPolicy => new(
    [
        // Home country - always allowed, no surcharge
        CountryTravelRule.Allowed(CountryCode.Germany),

        // Western Europe - allowed without permit
        CountryTravelRule.Allowed(CountryCode.Austria),
        CountryTravelRule.Allowed(CountryCode.Switzerland, Money.EuroGross(5.00m)),
        CountryTravelRule.Allowed(CountryCode.France),
        CountryTravelRule.Allowed(CountryCode.Netherlands),
        CountryTravelRule.Allowed(CountryCode.Belgium),
        CountryTravelRule.Allowed(CountryCode.Luxembourg),
        CountryTravelRule.Allowed(CountryCode.Denmark),

        // Southern Europe - allowed without permit
        CountryTravelRule.Allowed(CountryCode.Italy),
        CountryTravelRule.Allowed(CountryCode.Spain),
        CountryTravelRule.Allowed(CountryCode.Create("PT")), // Portugal

        // Northern Europe - allowed without permit
        CountryTravelRule.Allowed(CountryCode.Create("SE")), // Sweden
        CountryTravelRule.Allowed(CountryCode.Create("NO"), Money.EuroGross(5.00m)), // Norway (non-EU)
        CountryTravelRule.Allowed(CountryCode.Create("FI")), // Finland

        // Central/Eastern Europe - permit required
        CountryTravelRule.RequiresPermit(CountryCode.Poland, Money.EuroGross(15.00m)),
        CountryTravelRule.RequiresPermit(CountryCode.CzechRepublic, Money.EuroGross(15.00m)),
        CountryTravelRule.RequiresPermit(CountryCode.Create("SK"), Money.EuroGross(15.00m)), // Slovakia
        CountryTravelRule.RequiresPermit(CountryCode.Create("HU"), Money.EuroGross(15.00m)), // Hungary
        CountryTravelRule.RequiresPermit(CountryCode.Create("SI"), Money.EuroGross(15.00m)), // Slovenia
        CountryTravelRule.RequiresPermit(CountryCode.Create("HR"), Money.EuroGross(20.00m)), // Croatia

        // Restricted - special approval needed
        CountryTravelRule.Restricted(CountryCode.Create("RO"), "Higher theft risk"),
        CountryTravelRule.Restricted(CountryCode.Create("BG"), "Higher theft risk"),
        CountryTravelRule.Restricted(CountryCode.Create("GR"), "Ferry required"),

        // Prohibited
        CountryTravelRule.Prohibited(CountryCode.Create("UA"), "Active conflict zone"),
        CountryTravelRule.Prohibited(CountryCode.Create("RU"), "Sanctions and insurance void"),
        CountryTravelRule.Prohibited(CountryCode.Create("BY"), "Sanctions and insurance void"),
        CountryTravelRule.Prohibited(CountryCode.Create("TR"), "Insurance restrictions")
    ]);

    /// <summary>
    ///     Premium policy with more destinations allowed.
    /// </summary>
    public static CrossBorderPolicy PremiumGermanPolicy => new(
    [
        // All standard allowed countries
        CountryTravelRule.Allowed(CountryCode.Germany),
        CountryTravelRule.Allowed(CountryCode.Austria),
        CountryTravelRule.Allowed(CountryCode.Switzerland),
        CountryTravelRule.Allowed(CountryCode.France),
        CountryTravelRule.Allowed(CountryCode.Netherlands),
        CountryTravelRule.Allowed(CountryCode.Belgium),
        CountryTravelRule.Allowed(CountryCode.Luxembourg),
        CountryTravelRule.Allowed(CountryCode.Denmark),
        CountryTravelRule.Allowed(CountryCode.Italy),
        CountryTravelRule.Allowed(CountryCode.Spain),
        CountryTravelRule.Allowed(CountryCode.Create("PT")),
        CountryTravelRule.Allowed(CountryCode.Create("SE")),
        CountryTravelRule.Allowed(CountryCode.Create("NO")),
        CountryTravelRule.Allowed(CountryCode.Create("FI")),

        // Eastern Europe - allowed with surcharge (no permit needed)
        CountryTravelRule.Allowed(CountryCode.Poland, Money.EuroGross(10.00m)),
        CountryTravelRule.Allowed(CountryCode.CzechRepublic, Money.EuroGross(10.00m)),
        CountryTravelRule.Allowed(CountryCode.Create("SK"), Money.EuroGross(10.00m)),
        CountryTravelRule.Allowed(CountryCode.Create("HU"), Money.EuroGross(10.00m)),
        CountryTravelRule.Allowed(CountryCode.Create("SI"), Money.EuroGross(10.00m)),
        CountryTravelRule.Allowed(CountryCode.Create("HR"), Money.EuroGross(15.00m)),

        // Balkans - permit required
        CountryTravelRule.RequiresPermit(CountryCode.Create("RO"), Money.EuroGross(25.00m), true),
        CountryTravelRule.RequiresPermit(CountryCode.Create("BG"), Money.EuroGross(25.00m), true),
        CountryTravelRule.RequiresPermit(CountryCode.Create("GR"), Money.EuroGross(20.00m)),

        // Still prohibited
        CountryTravelRule.Prohibited(CountryCode.Create("UA"), "Active conflict zone"),
        CountryTravelRule.Prohibited(CountryCode.Create("RU"), "Sanctions and insurance void"),
        CountryTravelRule.Prohibited(CountryCode.Create("BY"), "Sanctions and insurance void"),
        CountryTravelRule.Prohibited(CountryCode.Create("TR"), "Insurance restrictions")
    ]);

    /// <summary>
    ///     Creates a custom policy from a list of rules.
    /// </summary>
    public static CrossBorderPolicy Create(IEnumerable<CountryTravelRule> rules) => new(rules);
}

/// <summary>
///     Result of cross-border travel validation.
/// </summary>
/// <param name="IsValid">Whether the travel is allowed.</param>
/// <param name="Issues">List of validation issues.</param>
/// <param name="RequiresPermit">Whether a cross-border permit is required.</param>
/// <param name="RequiresAdditionalInsurance">Whether additional insurance is required.</param>
public sealed record CrossBorderValidationResult(
    bool IsValid,
    IReadOnlyList<string> Issues,
    bool RequiresPermit,
    bool RequiresAdditionalInsurance)
{
    public static CrossBorderValidationResult Valid(bool requiresPermit = false, bool requiresAdditionalInsurance = false) =>
        new(true, [], requiresPermit, requiresAdditionalInsurance);
}
