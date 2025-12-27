namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Contracts;

/// <summary>
///     Request DTO for registering a new customer.
///     Accepts primitives from HTTP requests and maps to RegisterCustomerCommand with value objects.
/// </summary>
public sealed record RegisterCustomerRequest(
    CustomerInfoDto Customer,
    AddressInfoDto Address,
    DriversLicenseInfoDto DriversLicense);
