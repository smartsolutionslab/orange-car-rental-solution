namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

/// <summary>
/// Vehicle name value object.
/// </summary>
public sealed record VehicleName
{
    public string Value { get; init; }

    public VehicleName(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Vehicle name cannot be empty", nameof(value));
        }

        if (value.Length > 100)
        {
            throw new ArgumentException("Vehicle name cannot exceed 100 characters", nameof(value));
        }

        Value = value.Trim();
    }

    public static implicit operator string(VehicleName name) => name.Value;

    public override string ToString() => Value;
}
