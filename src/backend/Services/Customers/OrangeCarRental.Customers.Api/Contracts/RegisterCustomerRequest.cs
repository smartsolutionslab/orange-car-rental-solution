namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Contracts;

/// <summary>
///     Request DTO for registering a new customer.
///     Accepts primitives from HTTP requests and maps to RegisterCustomerCommand with value objects.
/// </summary>
public sealed record RegisterCustomerRequest
{
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Email { get; init; }
    public required string PhoneNumber { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    public required string Street { get; init; }
    public required string City { get; init; }
    public required string PostalCode { get; init; }
    public string Country { get; init; } = "Germany";
    public required string LicenseNumber { get; init; }
    public required string LicenseIssueCountry { get; init; }
    public required DateOnly LicenseIssueDate { get; init; }
    public required DateOnly LicenseExpiryDate { get; init; }
}
