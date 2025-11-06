namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Contracts;

/// <summary>
///     Request DTO for updating customer profile.
///     Accepts primitives from HTTP requests and maps to UpdateCustomerProfileCommand with value objects.
/// </summary>
public sealed record UpdateCustomerProfileRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string PhoneNumber { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public string Country { get; init; } = "Germany";
}
