namespace SmartSolutionsLab.OrangeCarRental.Reservations.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for a reservation using time-ordered UUIDs.
/// </summary>
public readonly record struct ReservationIdentifier
{
    public Guid Value { get; }

    private ReservationIdentifier(Guid value) => Value = value;

    public static ReservationIdentifier New() => new(Guid.CreateVersion7());

    public static ReservationIdentifier From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Reservation ID cannot be empty", nameof(value));
        }
        return new ReservationIdentifier(value);
    }

    public static ReservationIdentifier From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException($"Invalid reservation ID format: {value}", nameof(value));
        }
        return From(guid);
    }

    public override string ToString() => Value.ToString();
}
