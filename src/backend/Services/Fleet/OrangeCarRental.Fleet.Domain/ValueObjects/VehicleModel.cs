namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

/// <summary>
/// Represents the model of a vehicle (e.g., Golf, 3er, E-Klasse).
/// </summary>
public readonly record struct VehicleModel
{
    public string Value { get; }

    private VehicleModel(string value) => Value = value;

    public static VehicleModel Of(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException("Model cannot be empty", nameof(value));
        }

        if (value.Length > 100)
        {
            throw new ArgumentException("Model name cannot exceed 100 characters", nameof(value));
        }

        return new VehicleModel(value.Trim());
    }

    public static implicit operator string(VehicleModel model) => model.Value;

    public override string ToString() => Value;
}
