using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
/// Represents a currency using ISO 4217 codes.
/// </summary>
public readonly record struct Currency
{
    /// <summary>
    /// Three-letter currency code (ISO 4217).
    /// </summary>
    public string Code { get; }

    private Currency(string code) => Code = code;

    public static Currency Of(string code)
    {
        Ensure.That(code, nameof(code))
            .IsNotNullOrWhiteSpace()
            .AndHasLengthBetween(3, 3);

        return new Currency(code.ToUpperInvariant());
    }

    /// <summary>
    /// Euro currency.
    /// </summary>
    public static readonly Currency EUR = new("EUR");

    /// <summary>
    /// US Dollar currency.
    /// </summary>
    public static readonly Currency USD = new("USD");

    /// <summary>
    /// British Pound currency.
    /// </summary>
    public static readonly Currency GBP = new("GBP");

    /// <summary>
    /// Swiss Franc currency.
    /// </summary>
    public static readonly Currency CHF = new("CHF");

    public override string ToString() => Code;
}
