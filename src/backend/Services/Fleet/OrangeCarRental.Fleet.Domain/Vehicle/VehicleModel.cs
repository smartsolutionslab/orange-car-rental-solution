using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

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
        var trimmed = value?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        return new VehicleModel(trimmed);
    }

    public static implicit operator string(VehicleModel model) => model.Value;

    public override string ToString() => Value;
}
