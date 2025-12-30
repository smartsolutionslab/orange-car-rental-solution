namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Shared;

/// <summary>
///     Address information.
/// </summary>
public sealed record AddressInfoDto(
    string Street,
    string City,
    string PostalCode,
    string Country = "Germany");
