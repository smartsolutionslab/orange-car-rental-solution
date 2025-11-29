namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Contracts;

/// <summary>
///     Address information for updates.
/// </summary>
public sealed record AddressUpdateDto(
    string Street,
    string City,
    string PostalCode,
    string Country = "Germany");