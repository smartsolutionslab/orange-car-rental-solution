namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
/// Represents the model of a vehicle (e.g., Golf, 3er, E-Klasse).
/// </summary>
public readonly record struct VehicleModel
{
    public string Value { get; }

    private VehicleModel(string value) => Value = value;

    public static VehicleModel Of(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, nameof(value));

        var trimmed = value.Trim();
        if (trimmed.Length > 100)
            throw new ArgumentException("Model name cannot exceed 100 characters", nameof(value));

        return new VehicleModel(trimmed);
    }

    public static implicit operator string(VehicleModel model) => model.Value;

    public override string ToString() => Value;
}
