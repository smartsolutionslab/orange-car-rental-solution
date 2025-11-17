using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

public readonly record struct PaymentIdentifier(Guid Value) : IValueObject
{
    public static PaymentIdentifier New() => new(Guid.CreateVersion7());

    public static PaymentIdentifier From(Guid value)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty, nameof(value));
        return new PaymentIdentifier(value);
    }

    public static implicit operator Guid(PaymentIdentifier id) => id.Value;

    public override string ToString() => Value.ToString();
}
