using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;

/// <summary>
///     Strongly-typed identifier for a Reservation (reference to Reservations bounded context).
/// </summary>
public readonly record struct ReservationIdentifier(Guid Value) : IValueObject
{
    public static ReservationIdentifier From(Guid value)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty, nameof(value));
        return new ReservationIdentifier(value);
    }

    public static implicit operator Guid(ReservationIdentifier id) => id.Value;

    public override string ToString() => Value.ToString();
}
