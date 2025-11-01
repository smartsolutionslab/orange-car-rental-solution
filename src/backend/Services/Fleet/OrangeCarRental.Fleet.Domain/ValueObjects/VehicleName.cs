namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

/// <summary>
/// Vehicle name value object.
/// </summary>
public readonly record struct VehicleName
{
    public string Value { get; }

    private VehicleName(string value) => Value = value;

    public static VehicleName Of(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        if (value.Length > 100) throw new ArgumentException("Vehicle name cannot exceed 100 characters", nameof(value));

        return new VehicleName(value.Trim());
    }

    public static implicit operator string(VehicleName name) => name.Value;

    public override string ToString() => Value;
}
