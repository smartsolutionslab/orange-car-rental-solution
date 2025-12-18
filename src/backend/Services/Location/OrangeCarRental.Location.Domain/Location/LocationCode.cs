using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

/// <summary>
///     Location code identifier (e.g., "BER-HBF" for Berlin Hauptbahnhof).
/// </summary>
public readonly record struct LocationCode(string Value) : IValueObject
{
    public static LocationCode From(string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasLengthBetween(3, 20)
            .AndSatisfies(
                v => v.All(c => char.IsLetterOrDigit(c) || c == '-'),
                "Location code must contain only letters, digits, and hyphens");

        return new LocationCode(value.ToUpperInvariant().Trim());
    }

    public override string ToString() => Value;
}
