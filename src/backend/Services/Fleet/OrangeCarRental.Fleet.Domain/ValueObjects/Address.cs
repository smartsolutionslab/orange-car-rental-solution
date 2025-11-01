namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.ValueObjects;

/// <summary>
/// Address value object.
/// Represents a physical address with street, city, and postal code.
/// </summary>
public readonly record struct Address
{
    public Street Street { get; }
    public City City { get; }
    public PostalCode PostalCode { get; }

    private Address(Street street, City city, PostalCode postalCode)
    {
        Street = street;
        City = city;
        PostalCode = postalCode;
    }

    public static Address Of(string street, string city, string postalCode)
    {
        return new Address(
            ValueObjects.Street.Of(street),
            ValueObjects.City.Of(city),
            ValueObjects.PostalCode.Of(postalCode)
        );
    }

    /// <summary>
    /// Creates an empty address (for cases where address is not available)
    /// </summary>
    public static Address Empty => new(
        ValueObjects.Street.Empty,
        ValueObjects.City.Of("Unknown"),
        ValueObjects.PostalCode.Empty
    );

    /// <summary>
    /// Gets the full formatted address
    /// </summary>
    public string FullAddress
    {
        get
        {
            var parts = new List<string>();

            if (!Street.IsEmpty)
            {
                parts.Add(Street.Value);
            }

            var cityPostal = new List<string>();
            if (!PostalCode.IsEmpty)
            {
                cityPostal.Add(PostalCode.Value);
            }
            cityPostal.Add(City.Value);

            parts.Add(string.Join(" ", cityPostal));

            return string.Join(", ", parts);
        }
    }

    public override string ToString() => FullAddress;
}
