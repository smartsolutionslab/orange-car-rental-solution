using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Payment;

public readonly record struct PaymentIdentifier(Guid Value) : IValueObject
{
    public static PaymentIdentifier New() => new(Guid.CreateVersion7());

    public static PaymentIdentifier From(Guid value)
    {
        Ensure.That(value, nameof(value)).IsNotEmpty();
        return new PaymentIdentifier(value);
    }

    public static implicit operator Guid(PaymentIdentifier id) => id.Value;

    public override string ToString() => Value.ToString();
}
