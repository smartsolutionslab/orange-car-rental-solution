namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Contracts;

/// <summary>
///     Address details.
/// </summary>
public sealed record AddressDetailsDto(
    string Street,
    string City,
    string PostalCode,
    string Country = "Germany");