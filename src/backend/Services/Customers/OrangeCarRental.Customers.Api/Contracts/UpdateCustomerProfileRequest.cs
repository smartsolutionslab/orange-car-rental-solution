namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Contracts;

/// <summary>
///     Request DTO for updating customer profile.
///     Accepts primitives from HTTP requests and maps to UpdateCustomerProfileCommand with value objects.
/// </summary>
public sealed record UpdateCustomerProfileRequest
{
    public required CustomerProfileDto Profile { get; init; }
    public required AddressUpdateDto Address { get; init; }
}

/// <summary>
///     Customer profile information for updates.
/// </summary>
public sealed record CustomerProfileDto
{
    /// <summary>First name</summary>
    public required string FirstName { get; init; }

    /// <summary>Last name</summary>
    public required string LastName { get; init; }

    /// <summary>Phone number</summary>
    public required string PhoneNumber { get; init; }
}

/// <summary>
///     Address information for updates.
/// </summary>
public sealed record AddressUpdateDto
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
