namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;

/// <summary>
/// Data transfer object for customer address information.
/// Maps from Address value object.
/// </summary>
public sealed record AddressDto
{
    /// <summary>
    /// Street name and number (e.g., "Hauptstraße 123").
    /// </summary>
    public required string Street { get; init; }

    /// <summary>
    /// City name (e.g., "Berlin").
    /// </summary>
    public required string City { get; init; }

    /// <summary>
    /// German postal code (5 digits, e.g., "10115").
    /// </summary>
    public required string PostalCode { get; init; }

    /// <summary>
    /// Country name (e.g., "Germany").
    /// </summary>
    public required string Country { get; init; }

    /// <summary>
    /// Full formatted address.
    /// Example: "Hauptstraße 123, 10115 Berlin, Germany"
    /// </summary>
    public string FullAddress => $"{Street}, {PostalCode} {City}, {Country}";
}
