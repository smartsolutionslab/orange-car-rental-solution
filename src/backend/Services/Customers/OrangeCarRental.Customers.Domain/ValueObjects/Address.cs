namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.ValueObjects;

/// <summary>
/// Address value object for German addresses.
/// Represents a physical address with street, city, postal code, and country.
/// </summary>
public readonly record struct Address
{
    public string Street { get; }
    public string City { get; }
    public string PostalCode { get; }
    public string Country { get; }

    private Address(string street, string city, string postalCode, string country)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
        Country = country;
    }

    /// <summary>
    /// Creates an address value object with all components.
    /// </summary>
    /// <param name="street">Street name and number (e.g., "Hauptstraße 123").</param>
    /// <param name="city">City name (e.g., "Berlin").</param>
    /// <param name="postalCode">German postal code (5 digits, e.g., "10115").</param>
    /// <param name="country">Country name (defaults to "Germany").</param>
    /// <exception cref="ArgumentException">Thrown when any required field is invalid.</exception>
    public static Address Of(string street, string city, string postalCode, string country = "Germany")
    {
        if (string.IsNullOrWhiteSpace(street))
            throw new ArgumentException("Street cannot be empty", nameof(street));

        if (string.IsNullOrWhiteSpace(city))
            throw new ArgumentException("City cannot be empty", nameof(city));

        if (string.IsNullOrWhiteSpace(postalCode))
            throw new ArgumentException("Postal code cannot be empty", nameof(postalCode));

        var normalizedStreet = street.Trim();
        var normalizedCity = city.Trim();
        var normalizedPostalCode = postalCode.Trim();
        var normalizedCountry = (country ?? "Germany").Trim();

        // Validate German postal code format (5 digits)
        if (normalizedCountry.Equals("Germany", StringComparison.OrdinalIgnoreCase) ||
            normalizedCountry.Equals("Deutschland", StringComparison.OrdinalIgnoreCase))
        {
            if (normalizedPostalCode.Length != 5 || !normalizedPostalCode.All(char.IsDigit))
            {
                throw new ArgumentException(
                    "German postal code must be exactly 5 digits",
                    nameof(postalCode));
            }
        }

        // Validate lengths
        if (normalizedStreet.Length > 200)
            throw new ArgumentException("Street name is too long (max 200 characters)", nameof(street));

        if (normalizedCity.Length > 100)
            throw new ArgumentException("City name is too long (max 100 characters)", nameof(city));

        if (normalizedCountry.Length > 100)
            throw new ArgumentException("Country name is too long (max 100 characters)", nameof(country));

        return new Address(normalizedStreet, normalizedCity, normalizedPostalCode, normalizedCountry);
    }

    /// <summary>
    /// Gets the full formatted address.
    /// Example: "Hauptstraße 123, 10115 Berlin, Germany"
    /// </summary>
    public string FullAddress => $"{Street}, {PostalCode} {City}, {Country}";

    /// <summary>
    /// Creates an anonymized address for GDPR compliance.
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
