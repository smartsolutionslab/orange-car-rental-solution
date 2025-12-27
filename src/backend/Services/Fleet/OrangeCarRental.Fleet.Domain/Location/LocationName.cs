using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Location;

/// <summary>
///     Location name value object.
///     Represents the display name/title of a rental location (e.g., "Berlin Hauptbahnhof").
/// </summary>
/// <param name="Value">The location name value.</param>
public readonly record struct LocationName(string Value) : IValueObject
{
    public static LocationName From(string name)
    {
        var trimmed = name?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(name))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        return new LocationName(trimmed);
    }

    public static LocationName? FromNullable(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        return From(value);
    }

    public static implicit operator string(LocationName name) => name.Value;

    public override string ToString() => Value;
}
