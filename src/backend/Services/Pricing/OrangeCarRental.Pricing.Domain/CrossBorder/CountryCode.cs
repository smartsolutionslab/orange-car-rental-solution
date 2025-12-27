using System.Text.RegularExpressions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.CrossBorder;

/// <summary>
///     ISO 3166-1 alpha-2 country code.
/// </summary>
public readonly partial record struct CountryCode : IValueObject
{
    /// <summary>
    ///     Gets the two-letter country code.
    /// </summary>
    public string Value { get; }

    private CountryCode(string value)
    {
        Value = value;
    }

    /// <summary>
    ///     Creates a country code from a string.
    /// </summary>
    public static CountryCode Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Country code cannot be empty", nameof(value));

        var normalized = value.Trim().ToUpperInvariant();

        if (!CountryCodeRegex().IsMatch(normalized))
            throw new ArgumentException(
                "Country code must be exactly 2 letters (ISO 3166-1 alpha-2)",
                nameof(value));

        return new CountryCode(normalized);
    }

    /// <summary>
    ///     Germany.
    /// </summary>
    public static readonly CountryCode Germany = new("DE");

    /// <summary>
    ///     Austria.
    /// </summary>
    public static readonly CountryCode Austria = new("AT");

    /// <summary>
    ///     Switzerland.
    /// </summary>
    public static readonly CountryCode Switzerland = new("CH");

    /// <summary>
    ///     France.
    /// </summary>
    public static readonly CountryCode France = new("FR");

    /// <summary>
    ///     Netherlands.
    /// </summary>
    public static readonly CountryCode Netherlands = new("NL");

    /// <summary>
    ///     Belgium.
    /// </summary>
    public static readonly CountryCode Belgium = new("BE");

    /// <summary>
    ///     Luxembourg.
    /// </summary>
    public static readonly CountryCode Luxembourg = new("LU");

    /// <summary>
    ///     Poland.
    /// </summary>
    public static readonly CountryCode Poland = new("PL");

    /// <summary>
    ///     Czech Republic.
    /// </summary>
    public static readonly CountryCode CzechRepublic = new("CZ");

    /// <summary>
    ///     Denmark.
    /// </summary>
    public static readonly CountryCode Denmark = new("DK");

    /// <summary>
    ///     Italy.
    /// </summary>
    public static readonly CountryCode Italy = new("IT");

    /// <summary>
    ///     Spain.
    /// </summary>
    public static readonly CountryCode Spain = new("ES");

    /// <summary>
    ///     Gets whether this is an EU member state.
    /// </summary>
    public bool IsEuMember => EuMemberStates.Contains(Value);

    /// <summary>
    ///     Gets whether this is a Schengen area country.
    /// </summary>
    public bool IsSchengenArea => SchengenCountries.Contains(Value);

    /// <summary>
    ///     Gets the country name in German.
    /// </summary>
    public string GetGermanName() => Value switch
    {
        "DE" => "Deutschland",
        "AT" => "Österreich",
        "CH" => "Schweiz",
        "FR" => "Frankreich",
        "NL" => "Niederlande",
        "BE" => "Belgien",
        "LU" => "Luxemburg",
        "PL" => "Polen",
        "CZ" => "Tschechien",
        "DK" => "Dänemark",
        "IT" => "Italien",
        "ES" => "Spanien",
        "PT" => "Portugal",
        "GB" => "Großbritannien",
        "SE" => "Schweden",
        "NO" => "Norwegen",
        "FI" => "Finnland",
        "HU" => "Ungarn",
        "SK" => "Slowakei",
        "SI" => "Slowenien",
        "HR" => "Kroatien",
        "RO" => "Rumänien",
        "BG" => "Bulgarien",
        "GR" => "Griechenland",
        "UA" => "Ukraine",
        "RU" => "Russland",
        "BY" => "Belarus",
        "TR" => "Türkei",
        _ => Value
    };

    /// <summary>
    ///     Gets the country name in English.
    /// </summary>
    public string GetEnglishName() => Value switch
    {
        "DE" => "Germany",
        "AT" => "Austria",
        "CH" => "Switzerland",
        "FR" => "France",
        "NL" => "Netherlands",
        "BE" => "Belgium",
        "LU" => "Luxembourg",
        "PL" => "Poland",
        "CZ" => "Czech Republic",
        "DK" => "Denmark",
        "IT" => "Italy",
        "ES" => "Spain",
        "PT" => "Portugal",
        "GB" => "United Kingdom",
        "SE" => "Sweden",
        "NO" => "Norway",
        "FI" => "Finland",
        "HU" => "Hungary",
        "SK" => "Slovakia",
        "SI" => "Slovenia",
        "HR" => "Croatia",
        "RO" => "Romania",
        "BG" => "Bulgaria",
        "GR" => "Greece",
        "UA" => "Ukraine",
        "RU" => "Russia",
        "BY" => "Belarus",
        "TR" => "Turkey",
        _ => Value
    };

    public override string ToString() => Value;

    private static readonly HashSet<string> EuMemberStates =
    [
        "AT", "BE", "BG", "HR", "CY", "CZ", "DK", "EE", "FI", "FR",
        "DE", "GR", "HU", "IE", "IT", "LV", "LT", "LU", "MT", "NL",
        "PL", "PT", "RO", "SK", "SI", "ES", "SE"
    ];

    private static readonly HashSet<string> SchengenCountries =
    [
        "AT", "BE", "HR", "CZ", "DK", "EE", "FI", "FR", "DE", "GR",
        "HU", "IS", "IT", "LV", "LI", "LT", "LU", "MT", "NL", "NO",
        "PL", "PT", "SK", "SI", "ES", "SE", "CH"
    ];

    [GeneratedRegex(@"^[A-Z]{2}$")]
    private static partial Regex CountryCodeRegex();
}
