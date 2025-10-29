namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

/// <summary>
/// Represents the manufacturing year of a vehicle.
/// Typical rental vehicles are from recent years (e.g., 1990-current year).
/// </summary>
public readonly record struct ManufacturingYear
{
    public int Value { get; }

    private ManufacturingYear(int value) => Value = value;

    public static ManufacturingYear Of(int value)
    {
        if (value < 1990)
        {
            throw new ArgumentException("Manufacturing year must be 1990 or later for rental vehicles", nameof(value));
        }

        var currentYear = DateTime.UtcNow.Year;
        if (value > currentYear + 1)
        {
            throw new ArgumentException($"Manufacturing year cannot be later than {currentYear + 1}", nameof(value));
        }

        return new ManufacturingYear(value);
    }

    public static implicit operator int(ManufacturingYear year) => year.Value;

    public override string ToString() => Value.ToString();
}
