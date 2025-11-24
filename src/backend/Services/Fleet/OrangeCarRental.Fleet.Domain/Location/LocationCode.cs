using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

/// <summary>
///     Location code value object.
///     Represents a unique identifier for a rental location (e.g., "BER-HBF", "MUC-FLG").
/// </summary>
/// <param name="Value">The location code value.</param>
public readonly record struct LocationCode(string Value) : IValueObject
{
    public static LocationCode Of(string code)
    {
        var trimmed = code?.Trim().ToUpperInvariant() ?? string.Empty;

        Ensure.That(trimmed, nameof(code))
            .IsNotNullOrWhiteSpace()
            .AndHasMinLength(3);

        return new LocationCode(trimmed);
    }

    public static LocationCode? TryParse(string? value)
    {
        if(string.IsNullOrWhiteSpace(value)) return null;

        return Of(value);
    }

    public static implicit operator string(LocationCode code) => code.Value;

    public override string ToString() => Value;
}
