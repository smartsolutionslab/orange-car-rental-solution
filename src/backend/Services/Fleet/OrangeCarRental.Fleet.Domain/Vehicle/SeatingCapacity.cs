using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Represents the seating capacity of a vehicle.
///     Typical rental vehicles have 2-9 seats.
/// </summary>
public readonly record struct SeatingCapacity : IValueObject
{
    private SeatingCapacity(int value)
    {
        Value = value;
    }

    public int Value { get; }

    public static SeatingCapacity Of(int value)
    {
        if (value < 2)
            throw new ArgumentException("Seating capacity must be at least 2", nameof(value));

        if (value > 9)
            throw new ArgumentException("Seating capacity cannot exceed 9 for rental vehicles", nameof(value));

        return new SeatingCapacity(value);
    }

    public static implicit operator int(SeatingCapacity capacity) => capacity.Value;

    // Comparison operators for filtering and sorting
    public static bool operator <(SeatingCapacity left, SeatingCapacity right) => left.Value < right.Value;
    public static bool operator <=(SeatingCapacity left, SeatingCapacity right) => left.Value <= right.Value;
    public static bool operator >(SeatingCapacity left, SeatingCapacity right) => left.Value > right.Value;
    public static bool operator >=(SeatingCapacity left, SeatingCapacity right) => left.Value >= right.Value;

    public override string ToString() => Value.ToString();
}
