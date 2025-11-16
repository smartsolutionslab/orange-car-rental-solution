using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

/// <summary>
///     Vehicle category code for pricing (e.g., KLEIN, KOMPAKT, MITTEL).
///     Maps to VehicleCategory from Fleet service.
/// </summary>
/// <param name="Value">The category code value.</param>
public readonly record struct CategoryCode(string Value) : IValueObject
{
    public static CategoryCode Of(string value)
    {
        var trimmed = value?.Trim().ToUpperInvariant() ?? string.Empty;

        Ensure.That(trimmed, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(20);

        return new CategoryCode(trimmed);
    }

    public static implicit operator string(CategoryCode code) => code.Value;

    public override string ToString() => Value;
}
