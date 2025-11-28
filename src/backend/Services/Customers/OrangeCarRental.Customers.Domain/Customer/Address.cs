using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Address value object for German addresses.
///     Represents a physical address with street, city, postal code, and country.
/// </summary>
/// <param name="Street">Street name and number.</param>
/// <param name="City">City name (value object).</param>
/// <param name="PostalCode">Postal code (value object).</param>
/// <param name="Country">Country name.</param>
public readonly record struct Address(string Street, City City, PostalCode PostalCode, string Country) : IValueObject
{
    /// <summary>
    ///     Gets the full formatted address.
    ///     Example: "Hauptstraße 123, 10115 Berlin, Germany"
    /// </summary>
    public string FullAddress => $"{Street}, {PostalCode} {City}, {Country}";

    /// <summary>
    ///     Creates an address value object with all components.
    /// </summary>
    /// <param name="street">Street name and number (e.g., "Hauptstraße 123").</param>
    /// <param name="city">City name (e.g., "Berlin").</param>
    /// <param name="postalCode">German postal code (5 digits, e.g., "10115").</param>
    /// <param name="country">Country name (defaults to "Germany").</param>
    /// <exception cref="ArgumentException">Thrown when any required field is invalid.</exception>
    public static Address Of(string street, string city, string postalCode, string country = "Germany")
    {
        Ensure.That(street, nameof(street))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(200);

        var normalizedStreet = street.Trim();
        var normalizedCountry = (country ?? "Germany").Trim();

        Ensure.That(normalizedCountry, nameof(country))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        // Create value objects (they handle their own validation)
        var cityValueObject = City.From(city);
        var postalCodeValueObject = PostalCode.From(postalCode);

        return new Address(normalizedStreet, cityValueObject, postalCodeValueObject, normalizedCountry);
    }

    /// <summary>
    ///     Creates an anonymized address for GDPR compliance.
    /// </summary>
    public static Address Anonymized()
    {
        return new Address(
            "Anonymized Street",
            new City("Anonymized City"),
            new PostalCode("00000"),
            "Germany");
    }

    public override string ToString() => FullAddress;
}
