namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Strongly-typed identifier for Vehicle aggregate.
/// </summary>
public readonly record struct VehicleIdentifier
{
    private VehicleIdentifier(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static VehicleIdentifier New() => new(Guid.CreateVersion7());

    public static VehicleIdentifier From(Guid value)
    {
        ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty, nameof(value));
        return new VehicleIdentifier(value);
    }

    public static VehicleIdentifier From(string value)
    {
        if (!Guid.TryParse(value, out var guid))
            throw new ArgumentException($"Invalid vehicle ID format: {value}", nameof(value));

        return From(guid);
    }

    public override string ToString() => Value.ToString();
}
