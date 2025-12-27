namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;

/// <summary>
///     Data transfer object for customer address information.
///     Maps from Address value object.
/// </summary>
public sealed record AddressDto(
    string Street,
    string City,
    string PostalCode,
    string Country)
{
    /// <summary>
    ///     Full formatted address.
    ///     Example: "Hauptstraße 123, 10115 Berlin, Germany"
    /// </summary>
    public string FullAddress => $"{Street}, {PostalCode} {City}, {Country}";
}
