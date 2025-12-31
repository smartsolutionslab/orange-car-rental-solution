using SmartSolutionsLab.OrangeCarRental.Customers.Api.Shared;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Requests;

/// <summary>
///     Request DTO for updating customer profile.
///     Accepts primitives from HTTP requests and maps to UpdateCustomerProfileCommand with value objects.
/// </summary>
public sealed record UpdateCustomerProfileRequest(
    CustomerProfileDto Profile,
    AddressUpdateDto Address);
