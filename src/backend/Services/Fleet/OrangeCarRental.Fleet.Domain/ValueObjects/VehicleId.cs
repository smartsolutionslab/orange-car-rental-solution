namespace SmartSolutionsLab.Fleet.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for Vehicle aggregate.
/// </summary>
public sealed record VehicleIdentifier(Guid Value)
{
    public VehicleIdentifier() : this(Guid.Empty) { }

    public static VehicleIdentifier New() => new(Guid.NewGuid());

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
