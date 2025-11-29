using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

/// <summary>
///     Opening hours for a rental location.
///     Stored as simple string for flexibility (e.g., "Mon-Fri: 08:00-18:00, Sat: 09:00-14:00").
/// </summary>
public readonly record struct OpeningHours(string Value) : IValueObject
{
    public static OpeningHours From(string value)
    {
        Ensure.That(value, nameof(value))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(500);

        return new OpeningHours(value.Trim());
    }

    /// <summary>
    ///     Standard German business hours.
    /// </summary>
    public static OpeningHours Standard => From("Mo-Fr: 08:00-18:00, Sa: 09:00-14:00");

    /// <summary>
    ///     24/7 operation.
    /// </summary>
    public static OpeningHours TwentyFourSeven => From("24/7");

    public override string ToString() => Value;
}
