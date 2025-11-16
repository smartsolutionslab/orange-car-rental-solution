using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Represents the model of a vehicle (e.g., Golf, 3er, E-Klasse).
/// </summary>
/// <param name="Value">The vehicle model value.</param>
public readonly record struct VehicleModel(string Value) : IValueObject
{
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
