using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.CrossBorder;

/// <summary>
///     Travel rule for a specific country.
/// </summary>
public sealed record CountryTravelRule
{
    /// <summary>
    ///     Gets the destination country.
    /// </summary>
    public CountryCode Country { get; }

    /// <summary>
    ///     Gets the restriction level.
    /// </summary>
    public TravelRestriction Restriction { get; }

    /// <summary>
    ///     Gets the daily surcharge for this destination (if any).
    /// </summary>
    public Money? DailySurcharge { get; }

    /// <summary>
    ///     Gets whether additional insurance is required.
    /// </summary>
    public bool RequiresAdditionalInsurance { get; }

    /// <summary>
    ///     Gets the reason for any restrictions.
    /// </summary>
    public string? RestrictionReason { get; }

    private CountryTravelRule(
        CountryCode country,
        TravelRestriction restriction,
        Money? dailySurcharge,
        bool requiresAdditionalInsurance,
        string? restrictionReason)
    {
        Country = country;
        Restriction = restriction;
        DailySurcharge = dailySurcharge;
        RequiresAdditionalInsurance = requiresAdditionalInsurance;
        RestrictionReason = restrictionReason;
    }

    /// <summary>
    ///     Creates a rule for an allowed destination.
    /// </summary>
    public static CountryTravelRule Allowed(CountryCode country, Money? dailySurcharge = null) =>
        new(country, TravelRestriction.Allowed, dailySurcharge, false, null);

    /// <summary>
    ///     Creates a rule requiring a permit.
    /// </summary>
    public static CountryTravelRule RequiresPermit(
        CountryCode country,
        Money dailySurcharge,
        bool requiresAdditionalInsurance = false) =>
        new(country, TravelRestriction.PermitRequired, dailySurcharge, requiresAdditionalInsurance, null);

    /// <summary>
    ///     Creates a rule for a restricted destination.
    /// </summary>
    public static CountryTravelRule Restricted(
        CountryCode country,
        string reason,
        bool requiresAdditionalInsurance = true) =>
        new(country, TravelRestriction.Restricted, null, requiresAdditionalInsurance, reason);

    /// <summary>
    ///     Creates a rule for a prohibited destination.
    /// </summary>
    public static CountryTravelRule Prohibited(CountryCode country, string reason) =>
        new(country, TravelRestriction.Prohibited, null, false, reason);

    /// <summary>
    ///     Gets whether this destination is allowed (with or without permit).
    /// </summary>
    public bool IsAllowed => Restriction is TravelRestriction.Allowed or TravelRestriction.PermitRequired;

    /// <summary>
    ///     Gets whether this destination requires advance booking/permit.
    /// </summary>
    public bool RequiresAdvanceBooking => Restriction == TravelRestriction.PermitRequired;

    /// <summary>
    ///     Gets the German description of this rule.
    /// </summary>
    public string GetGermanDescription() => Restriction switch
    {
        TravelRestriction.Allowed when DailySurcharge is null =>
            $"Reisen nach {Country.GetGermanName()} erlaubt",
        TravelRestriction.Allowed =>
            $"Reisen nach {Country.GetGermanName()} erlaubt ({DailySurcharge}/Tag)",
        TravelRestriction.PermitRequired =>
            $"Reisen nach {Country.GetGermanName()} mit Grenzübertrittserlaubnis ({DailySurcharge}/Tag)",
        TravelRestriction.Restricted =>
            $"Reisen nach {Country.GetGermanName()} nur mit Sondergenehmigung: {RestrictionReason}",
        TravelRestriction.Prohibited =>
            $"Reisen nach {Country.GetGermanName()} nicht gestattet: {RestrictionReason}",
        _ => Country.GetGermanName()
    };

    /// <summary>
    ///     Gets the English description of this rule.
    /// </summary>
    public string GetEnglishDescription() => Restriction switch
    {
        TravelRestriction.Allowed when DailySurcharge is null =>
            $"Travel to {Country.GetEnglishName()} allowed",
        TravelRestriction.Allowed =>
            $"Travel to {Country.GetEnglishName()} allowed ({DailySurcharge}/day)",
        TravelRestriction.PermitRequired =>
            $"Travel to {Country.GetEnglishName()} requires cross-border permit ({DailySurcharge}/day)",
        TravelRestriction.Restricted =>
            $"Travel to {Country.GetEnglishName()} requires special approval: {RestrictionReason}",
        TravelRestriction.Prohibited =>
            $"Travel to {Country.GetEnglishName()} not permitted: {RestrictionReason}",
        _ => Country.GetEnglishName()
    };
}
