using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Value-added tax (VAT) rate for German market compliance.
///     German standard VAT rate is 19%, reduced rate is 7%.
/// </summary>
public readonly record struct VatRate : IValueObject
{
    /// <summary>
    ///     Gets the VAT rate as a decimal (e.g., 0.19 for 19%).
    /// </summary>
    public decimal Value { get; }

    private VatRate(decimal value)
    {
        Value = value;
    }

    /// <summary>
    ///     German standard VAT rate (19%).
    /// </summary>
    public static readonly VatRate GermanStandard = new(0.19m);

    /// <summary>
    ///     German reduced VAT rate (7%) for food, books, etc.
    /// </summary>
    public static readonly VatRate GermanReduced = new(0.07m);

    /// <summary>
    ///     Zero VAT rate for VAT-exempt services.
    /// </summary>
    public static readonly VatRate Zero = new(0m);

    /// <summary>
    ///     Creates a VAT rate from a decimal value.
    /// </summary>
    /// <param name="value">The VAT rate as a decimal (e.g., 0.19 for 19%).</param>
    /// <exception cref="ArgumentException">Thrown when the rate is negative or greater than 1.</exception>
    public static VatRate Of(decimal value)
    {
        Ensure.That(value, nameof(value))
            .ThrowIf(value < 0, "VAT rate cannot be negative")
            .ThrowIf(value > 1, "VAT rate cannot be greater than 100%");

        return new VatRate(Math.Round(value, 4));
    }

    /// <summary>
    ///     Creates a VAT rate from a percentage value.
    /// </summary>
    /// <param name="percentage">The VAT rate as a percentage (e.g., 19 for 19%).</param>
    public static VatRate FromPercentage(decimal percentage)
    {
        return Of(percentage / 100m);
    }

    /// <summary>
    ///     Gets the VAT rate as a percentage (e.g., 19 for 19%).
    /// </summary>
    public decimal AsPercentage => Value * 100m;

    /// <summary>
    ///     Calculates the VAT amount from a net amount.
    /// </summary>
    public decimal CalculateVatAmount(decimal netAmount) => Math.Round(netAmount * Value, 2);

    /// <summary>
    ///     Calculates the gross amount from a net amount.
    /// </summary>
    public decimal CalculateGrossAmount(decimal netAmount) => netAmount + CalculateVatAmount(netAmount);

    /// <summary>
    ///     Calculates the net amount from a gross amount.
    /// </summary>
    public decimal CalculateNetAmount(decimal grossAmount) => Math.Round(grossAmount / (1 + Value), 2);

    /// <summary>
    ///     Gets the German display string (e.g., "19 %").
    /// </summary>
    public string ToGermanString() => $"{AsPercentage:0.##} %";

    public override string ToString() => $"{AsPercentage:0.##}%";
}
