using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

/// <summary>
///     Represents a quantity value with a unit of measure.
///     Ensures non-negative quantities with explicit units for domain clarity.
/// </summary>
public readonly record struct Quantity : IValueObject
{
    /// <summary>
    ///     Gets the quantity value.
    /// </summary>
    public int Value { get; }

    /// <summary>
    ///     Gets the unit of measure.
    /// </summary>
    public string Unit { get; }

    private Quantity(int value, string unit)
    {
        Value = value;
        Unit = unit;
    }

    /// <summary>
    ///     Creates a quantity with the specified value and unit.
    /// </summary>
    /// <param name="value">The quantity value (must be non-negative).</param>
    /// <param name="unit">The unit of measure.</param>
    /// <exception cref="ArgumentException">Thrown when value is negative or unit is empty.</exception>
    public static Quantity Of(int value, string unit)
    {
        Ensure.That(value, nameof(value))
            .ThrowIf(value < 0, "Quantity cannot be negative");
        Ensure.That(unit, nameof(unit)).IsNotNullOrWhiteSpace();

        return new Quantity(value, unit.Trim());
    }

    /// <summary>
    ///     Creates a quantity representing days (German: Tage).
    /// </summary>
    public static Quantity Days(int count) => new(count, count == 1 ? "Tag" : "Tage");

    /// <summary>
    ///     Creates a quantity representing pieces (German: Stück).
    /// </summary>
    public static Quantity Pieces(int count) => new(count, "Stück");

    /// <summary>
    ///     Creates a quantity representing kilometers.
    /// </summary>
    public static Quantity Kilometers(int count) => new(count, "km");

    /// <summary>
    ///     Creates a quantity representing hours (German: Stunden).
    /// </summary>
    public static Quantity Hours(int count) => new(count, count == 1 ? "Stunde" : "Stunden");

    /// <summary>
    ///     Creates a quantity for a flat/one-time item (German: pauschal).
    /// </summary>
    public static Quantity Flat() => new(1, "pauschal");

    /// <summary>
    ///     Gets whether this is zero quantity.
    /// </summary>
    public bool IsZero => Value == 0;

    /// <summary>
    ///     Formats the quantity for display (e.g., "5 Tage").
    /// </summary>
    public string ToDisplayString() => $"{Value} {Unit}";

    public override string ToString() => ToDisplayString();
}
