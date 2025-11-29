using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
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

    public static SeatingCapacity From(int value)
    {
        Ensure.That(value, nameof(value))
            .IsGreaterThanOrEqual(2)
            .AndIsLessThanOrEqual(9);

        return new SeatingCapacity(value);
    }

    public static SeatingCapacity? From(int? value)
    {
        if (value == null) return null;

        return From(value.Value);
    }

    public static implicit operator int(SeatingCapacity capacity) => capacity.Value;

    // Comparison operators for filtering and sorting
    public static bool operator <(SeatingCapacity left, SeatingCapacity right) => left.Value < right.Value;
    public static bool operator <=(SeatingCapacity left, SeatingCapacity right) => left.Value <= right.Value;
    public static bool operator >(SeatingCapacity left, SeatingCapacity right) => left.Value > right.Value;
    public static bool operator >=(SeatingCapacity left, SeatingCapacity right) => left.Value >= right.Value;

    public override string ToString() => Value.ToString();
}
