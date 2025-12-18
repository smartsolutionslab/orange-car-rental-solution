using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

/// <summary>
///     Display name for a location (e.g., "Berlin Hauptbahnhof").
/// </summary>
public readonly record struct LocationName(string Value) : IValueObject
{
    public static LocationName From(string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        return new LocationName(value.Trim());
    }

    public override string ToString() => Value;
}
