using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Sepa;

/// <summary>
///     Strongly-typed identifier for SEPA mandates.
/// </summary>
public readonly record struct SepaMandateIdentifier(Guid Value) : IValueObject
{
    public static SepaMandateIdentifier New() => new(Guid.CreateVersion7());

    public static SepaMandateIdentifier From(Guid value)
    {
        Ensure.That(value, nameof(value)).IsNotEmpty();
        return new SepaMandateIdentifier(value);
    }

    public static implicit operator Guid(SepaMandateIdentifier id) => id.Value;

    public override string ToString() => Value.ToString();
}
