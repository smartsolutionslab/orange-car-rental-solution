using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Represents a currency using ISO 4217 codes.
/// </summary>
/// <param name="Code">Three-letter currency code (ISO 4217).</param>
public readonly record struct Currency(string Code)
{
    /// <summary>
    ///     Euro currency.
    /// </summary>
    public static readonly Currency EUR = new("EUR");

    /// <summary>
    ///     US Dollar currency.
    /// </summary>
    public static readonly Currency USD = new("USD");

    /// <summary>
    ///     British Pound currency.
    /// </summary>
    public static readonly Currency GBP = new("GBP");

    /// <summary>
    ///     Swiss Franc currency.
    /// </summary>
    public static readonly Currency CHF = new("CHF");

    public static Currency Of(string code)
    {
        Ensure.That(code, nameof(code))
            .IsNotNullOrWhiteSpace()
            .AndHasLengthBetween(3, 3);

        return new Currency(code.ToUpperInvariant());
    }

    public override string ToString() => Code;
}
