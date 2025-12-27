using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

/// <summary>
///     Location code for pricing (e.g., MUC-HBF, BER-AIRPORT).
///     Maps to Location from Fleet service.
/// </summary>
/// <param name="Value">The location code value.</param>
public readonly record struct LocationCode(string Value) : IValueObject
{
    public static LocationCode From(string value)
    {
        var trimmed = value?.Trim().ToUpperInvariant() ?? string.Empty;

        Ensure.That(trimmed, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasLengthBetween(3, 20);

        return new LocationCode(trimmed);
    }

    public static implicit operator string(LocationCode code) => code.Value;

    public override string ToString() => Value;
}
