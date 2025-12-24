using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Invoice;

/// <summary>
///     Strongly-typed identifier for Invoice aggregate.
/// </summary>
public readonly record struct InvoiceIdentifier(Guid Value) : IValueObject
{
    public static InvoiceIdentifier New() => new(Guid.CreateVersion7());

    public static InvoiceIdentifier From(Guid value)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty, nameof(value));
        return new InvoiceIdentifier(value);
    }

    public static implicit operator Guid(InvoiceIdentifier id) => id.Value;

    public override string ToString() => Value.ToString();
}
