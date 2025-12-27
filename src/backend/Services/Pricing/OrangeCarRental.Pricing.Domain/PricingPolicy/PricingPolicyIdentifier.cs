using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Domain.PricingPolicy;

/// <summary>
///     Strongly-typed identifier for PricingPolicy aggregate.
///     Uses GUID v7 for time-ordered identifiers with better database performance.
/// </summary>
public readonly record struct PricingPolicyIdentifier(Guid Value) : IValueObject
{
    /// <summary>
    ///     Creates a new unique pricing policy identifier using GUID v7.
    /// </summary>
    public static PricingPolicyIdentifier New() => new(Guid.CreateVersion7());

    /// <summary>
    ///     Creates a pricing policy identifier from an existing GUID.
    /// </summary>
    /// <param name="value">The GUID value.</param>
    /// <exception cref="ArgumentException">Thrown when the GUID is empty.</exception>
    public static PricingPolicyIdentifier From(Guid value)
    {
        Ensure.That(value, nameof(value)).IsNotEmpty();
        return new PricingPolicyIdentifier(value);
    }

    /// <summary>
    ///     Creates a pricing policy identifier from a string representation of a GUID.
    /// </summary>
    /// <param name="value">The string representation of the GUID.</param>
    /// <exception cref="ArgumentException">Thrown when the string is not a valid GUID.</exception>
    public static PricingPolicyIdentifier From(string value)
    {
        Ensure.That(value, nameof(value))
            .ThrowIf(!Guid.TryParse(value, out var guid), $"Invalid pricing policy ID format: {value}");
        return From(Guid.Parse(value));
    }

    /// <summary>
    ///     Implicit conversion to Guid for database mapping and serialization.
    /// </summary>
    public static implicit operator Guid(PricingPolicyIdentifier id) => id.Value;

    public override string ToString() => Value.ToString();
}
