using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

/// <summary>
/// Vehicle category code for pricing (e.g., KLEIN, KOMPAKT, MITTEL).
/// Maps to VehicleCategory from Fleet service.
/// </summary>
public readonly record struct CategoryCode
{
    public string Value { get; }

    private CategoryCode(string value) => Value = value;

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
