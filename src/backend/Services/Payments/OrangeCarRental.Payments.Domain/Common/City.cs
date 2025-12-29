using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;

/// <summary>
///     City name value object for invoice addresses.
/// </summary>
public readonly record struct City(string Value) : IValueObject
{
    public static City From(string city)
    {
        var trimmed = city?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(city))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        return new City(trimmed);
    }

    public static implicit operator string(City city) => city.Value;

    public override string ToString() => Value;
}
