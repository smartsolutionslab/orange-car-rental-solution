namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Contracts;

/// <summary>
///     Request DTO for registering a new customer.
///     Accepts primitives from HTTP requests and maps to RegisterCustomerCommand with value objects.
/// </summary>
public sealed record RegisterCustomerRequest(
    CustomerInfoDto Customer,
    AddressInfoDto Address,
    DriversLicenseInfoDto DriversLicense);

/// <summary>
///     Customer personal information.
/// </summary>
public sealed record CustomerInfoDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateOnly DateOfBirth);

/// <summary>
///     Address information.
/// </summary>
public sealed record AddressInfoDto(
    string Street,
    string City,
    string PostalCode,
    string Country = "Germany");

/// <summary>
///     Driver's license information.
/// </summary>
public sealed record DriversLicenseInfoDto(
    string LicenseNumber,
    string LicenseIssueCountry,
    DateOnly LicenseIssueDate,
    DateOnly LicenseExpiryDate);
