using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Represents the manufacturing year of a vehicle.
///     Typical rental vehicles are from recent years (e.g., 1990-current year).
/// </summary>
public readonly record struct ManufacturingYear(int Value) : IValueObject
{
    public static ManufacturingYear From(int value)
    {
        Ensure.That(value, nameof(value))
            .IsGreaterThanOrEqual(1990);

        var currentYear = DateTime.UtcNow.Year;
        Ensure.That(value, nameof(value))
            .ThrowIf(value > currentYear + 1, $"Manufacturing year cannot be later than {currentYear + 1}");

        return new ManufacturingYear(value);
    }

    public static ManufacturingYear? FromNullable(int? value)
    {
        if (value == null) return null;

        return From(value.Value);
    }

    public static implicit operator int(ManufacturingYear year) => year.Value;

    public override string ToString() => Value.ToString();
}
