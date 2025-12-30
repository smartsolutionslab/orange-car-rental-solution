namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Shared;

/// <summary>
///     Address details.
/// </summary>
public sealed record AddressDetailsDto(
    string Street,
    string City,
    string PostalCode,
    string Country = "Germany");
