using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Vehicle;

/// <summary>
///     Vehicle name value object.
/// </summary>
/// <param name="Value">The vehicle name value.</param>
public readonly record struct VehicleName(string Value) : IValueObject
{
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
