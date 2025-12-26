using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;

/// <summary>
///     Strongly-typed identifier for a Customer (reference to Customers bounded context).
/// </summary>
public readonly record struct CustomerId(Guid Value) : IValueObject
{
    public static CustomerId From(Guid value)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty, nameof(value));
        return new CustomerId(value);
    }

    public static implicit operator Guid(CustomerId id) => id.Value;

    public override string ToString() => Value.ToString();
}
