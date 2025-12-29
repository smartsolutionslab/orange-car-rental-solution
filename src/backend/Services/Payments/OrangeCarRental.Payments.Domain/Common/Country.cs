using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Domain.Common;

/// <summary>
///     Country name value object for invoice addresses.
/// </summary>
public readonly record struct Country(string Value) : IValueObject
{
    public static Country From(string country)
    {
        var trimmed = country?.Trim() ?? string.Empty;

        Ensure.That(trimmed, nameof(country))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        return new Country(trimmed);
    }

    /// <summary>
    ///     Germany (Deutschland).
    /// </summary>
    public static Country Germany => new("Deutschland");

    public static implicit operator string(Country country) => country.Value;

    public override string ToString() => Value;
}
