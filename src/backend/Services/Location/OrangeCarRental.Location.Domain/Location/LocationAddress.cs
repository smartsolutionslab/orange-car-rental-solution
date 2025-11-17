using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Location.Domain.Location;

/// <summary>
///     Physical address of a rental location.
/// </summary>
public readonly record struct LocationAddress(
    string Street,
    string City,
    string PostalCode,
    string Country) : IValueObject
{
    public static LocationAddress Of(string street, string city, string postalCode, string country = "Deutschland")
    {
        Ensure.That(street, nameof(street))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(200);

        Ensure.That(city, nameof(city))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        Ensure.That(postalCode, nameof(postalCode))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(10);

        Ensure.That(country, nameof(country))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        return new LocationAddress(
            street.Trim(),
            city.Trim(),
            postalCode.Trim(),
            country.Trim());
    }

    public override string ToString() => $"{Street}, {PostalCode} {City}, {Country}";
}
