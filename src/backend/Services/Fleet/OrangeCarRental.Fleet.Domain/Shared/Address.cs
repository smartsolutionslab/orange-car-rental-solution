using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Shared;

/// <summary>
///     Address value object.
///     Represents a physical address with street, city, and postal code.
/// </summary>
public readonly record struct Address(Street Street, City City, PostalCode PostalCode) : IValueObject
{
    /// <summary>
    ///     Creates an empty address (for cases where address is not available)
    /// </summary>
    public static Address Empty => new(
        Street.Empty,
        City.Of("Unknown"),
        PostalCode.Empty
    );

    /// <summary>
    ///     Gets the full formatted address
    /// </summary>
    public string FullAddress
    {
        get
        {
            var parts = new List<string>();

            if (!Street.IsEmpty) parts.Add(Street.Value);

            var cityPostal = new List<string>();
            if (!PostalCode.IsEmpty) cityPostal.Add(PostalCode.Value);
            cityPostal.Add(City.Value);

            parts.Add(string.Join(" ", cityPostal));

            return string.Join(", ", parts);
        }
    }

    public static Address Of(Street street, City city, PostalCode postalCode) => new(street, city, postalCode);

    public static Address Of(string street, string city, string postalCode)
    {
        return new Address(
            Street.Of(street),
            City.Of(city),
            PostalCode.Of(postalCode)
        );
    }

    public override string ToString() => FullAddress;
}
