namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Contracts;

/// <summary>
///     Request DTO for registering a new customer.
///     Accepts primitives from HTTP requests and maps to RegisterCustomerCommand with value objects.
/// </summary>
public sealed record RegisterCustomerRequest
{
    public required CustomerInfoDto Customer { get; init; }
    public required AddressInfoDto Address { get; init; }
    public required DriversLicenseInfoDto DriversLicense { get; init; }
}

/// <summary>
///     Customer personal information.
/// </summary>
public sealed record CustomerInfoDto
{
    /// <summary>First name</summary>
    public required string FirstName { get; init; }

    /// <summary>Last name</summary>
    public required string LastName { get; init; }

    /// <summary>Email address</summary>
    public required string Email { get; init; }

    /// <summary>Phone number</summary>
    public required string PhoneNumber { get; init; }

    /// <summary>Date of birth</summary>
    public required DateOnly DateOfBirth { get; init; }
}

/// <summary>
///     Address information.
/// </summary>
public sealed record AddressInfoDto
{
    /// <summary>Street address</summary>
    public required string Street { get; init; }

    /// <summary>City</summary>
    public required string City { get; init; }

    /// <summary>Postal code</summary>
    public required string PostalCode { get; init; }

    /// <summary>Country (defaults to Germany)</summary>
    public string Country { get; init; } = "Germany";
}

/// <summary>
///     Driver's license information.
/// </summary>
public sealed record DriversLicenseInfoDto
{
    /// <summary>License number</summary>
    public required string LicenseNumber { get; init; }

    /// <summary>Country that issued the license</summary>
    public required string LicenseIssueCountry { get; init; }

    /// <summary>License issue date</summary>
    public required DateOnly LicenseIssueDate { get; init; }

    /// <summary>License expiry date</summary>
    public required DateOnly LicenseExpiryDate { get; init; }
}
