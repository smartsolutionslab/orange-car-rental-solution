using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Validation;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Address value object for German addresses.
///     Represents a physical address with street, city, postal code, and country.
/// </summary>
/// <param name="Street">Street name and number.</param>
/// <param name="City">City name.</param>
/// <param name="PostalCode">Postal code.</param>
/// <param name="Country">Country name.</param>
public readonly record struct Address(string Street, string City, string PostalCode, string Country)
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

        Ensure.That(city, nameof(city))
            .IsNotNullOrWhiteSpace()
            .AndHasMaxLength(100);

        Ensure.That(postalCode, nameof(postalCode))
            .IsNotNullOrWhiteSpace();

        var normalizedStreet = street.Trim();
        var normalizedCity = city.Trim();
        var normalizedPostalCode = postalCode.Trim();
        var normalizedCountry = (country ?? "Germany").Trim();

        Ensure.That(normalizedCountry, nameof(country))
            .AndHasMaxLength(100);

        // Validate German postal code format (5 digits)
        if (normalizedCountry.Equals("Germany", StringComparison.OrdinalIgnoreCase) ||
            normalizedCountry.Equals("Deutschland", StringComparison.OrdinalIgnoreCase))
        {
            Ensure.That(normalizedPostalCode, nameof(postalCode))
                .AndHasLengthBetween(5, 5)
                .AndSatisfies(
                    code => code.All(char.IsDigit),
                    "German postal code must be exactly 5 digits");
        }

        return new Address(normalizedStreet, normalizedCity, normalizedPostalCode, normalizedCountry);
    }

    /// <summary>
    ///     Creates an anonymized address for GDPR compliance.
    /// </summary>
    public static Address Anonymized()
    {
        return new Address(
            "Anonymized Street",
            "Anonymized City",
            "00000",
            "Germany");
    }

    public override string ToString() => FullAddress;
}
