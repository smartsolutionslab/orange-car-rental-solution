using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;

/// <summary>
///     Strongly-typed identifier for a Customer (reference to Customers bounded context).
/// </summary>
public readonly record struct CustomerIdentifier(Guid Value) : IValueObject
{
    public static CustomerIdentifier From(Guid value)
    {
        Ensure.That(value, nameof(value)).IsNotEmpty();
        return new CustomerIdentifier(value);
    }

    public static implicit operator Guid(CustomerIdentifier id) => id.Value;

    public override string ToString() => Value.ToString();
}
