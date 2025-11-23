namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Contracts;

/// <summary>
///     Request DTO for updating customer profile.
///     Accepts primitives from HTTP requests and maps to UpdateCustomerProfileCommand with value objects.
/// </summary>
public sealed record UpdateCustomerProfileRequest(
    CustomerProfileDto Profile,
    AddressUpdateDto Address);

/// <summary>
///     Customer profile information for updates.
/// </summary>
public sealed record CustomerProfileDto(
    string FirstName,
    string LastName,
    string PhoneNumber);

/// <summary>
///     Address information for updates.
/// </summary>
public sealed record AddressUpdateDto(
    string Street,
    string City,
    string PostalCode,
    string Country = "Germany");
