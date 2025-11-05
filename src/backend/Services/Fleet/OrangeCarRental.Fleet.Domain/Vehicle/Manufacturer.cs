using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

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
        var trimmed = value?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        return new Manufacturer(trimmed);
    }

    public static implicit operator string(Manufacturer manufacturer) => manufacturer.Value;

    public override string ToString() => Value;
}
