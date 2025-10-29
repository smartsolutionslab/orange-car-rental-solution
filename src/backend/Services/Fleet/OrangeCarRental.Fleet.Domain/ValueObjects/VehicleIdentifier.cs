namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for Vehicle aggregate.
/// </summary>
public readonly record struct VehicleIdentifier
{
    public Guid Value { get; }

    private VehicleIdentifier(Guid value) => Value = value;

    public static VehicleIdentifier New() => new(Guid.CreateVersion7());

    public static VehicleIdentifier From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Vehicle ID cannot be empty", nameof(value));
        }
        return new VehicleIdentifier(value);
    }

    public static VehicleIdentifier From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException($"Invalid vehicle ID format: {value}", nameof(value));
        }

        return From(guid);
    }

    public override string ToString() => Value.ToString();
}
