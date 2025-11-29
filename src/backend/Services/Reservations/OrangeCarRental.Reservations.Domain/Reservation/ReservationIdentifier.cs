using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.Reservation;

/// <summary>
///     Strongly-typed identifier for a reservation using time-ordered UUIDs.
/// </summary>
public readonly record struct ReservationIdentifier(Guid Value) : IValueObject
{
    public static ReservationIdentifier New() => new(Guid.CreateVersion7());

    public static ReservationIdentifier From(Guid value)
    {
        Ensure.That(value, nameof(value)).IsNotEmpty();
        return new ReservationIdentifier(value);
    }

    public static ReservationIdentifier From(string value)
    {
        Ensure.That(value, nameof(value))
            .ThrowIf(!Guid.TryParse(value, out var guid), $"Invalid reservation ID format: {value}");
        return From(Guid.Parse(value));
    }

    public override string ToString() => Value.ToString();
}
