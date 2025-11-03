namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
/// Represents the manufacturer/brand of a vehicle (e.g., BMW, Volkswagen, Mercedes-Benz).
/// </summary>
public readonly record struct Manufacturer
{
    public string Value { get; }

    private Manufacturer(string value) => Value = value;

    public static Manufacturer Of(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        var trimmed = value.Trim();
        if (trimmed.Length > 100)
            throw new ArgumentException("Manufacturer name cannot exceed 100 characters", nameof(value));

        return new Manufacturer(trimmed);
    }

    public static implicit operator string(Manufacturer manufacturer) => manufacturer.Value;

    public override string ToString() => Value;
}
