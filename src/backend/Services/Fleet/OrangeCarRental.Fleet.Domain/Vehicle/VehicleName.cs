using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
/// Vehicle name value object.
/// </summary>
public readonly record struct VehicleName
{
    public string Value { get; }

    private VehicleName(string value) => Value = value;

    public static VehicleName Of(string value)
    {
        var trimmed = value?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        return new VehicleName(trimmed);
    }

    public static implicit operator string(VehicleName name) => name.Value;

    public override string ToString() => Value;
}
