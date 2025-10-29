namespace OrangeCarRental.Fleet.Domain.ValueObjects;

/// <summary>
/// Strongly-typed identifier for Vehicle aggregate.
/// </summary>
public sealed record VehicleId(Guid Value)
{
    public VehicleId() : this(Guid.Empty) { }

    public static VehicleId New() => new(Guid.NewGuid());

    public static VehicleId From(Guid value)
    {
        if (value == Guid.Empty)
        {
            throw new ArgumentException("Vehicle ID cannot be empty", nameof(value));
        }
        return new VehicleId(value);
    }

    public static VehicleId From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
        {
            throw new ArgumentException($"Invalid vehicle ID format: {value}", nameof(value));
        }

        return From(guid);
    }

    public override string ToString() => Value.ToString();
}
