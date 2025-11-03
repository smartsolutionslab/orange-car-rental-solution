namespace SmartSolutionsLab.OrangeCarRental.Reservations.Application.Services;

/// <summary>
/// DTO for registering a new customer via the Customers API.
/// </summary>
public sealed record RegisterCustomerDto(
    string FirstName,
    string LastName,
    string Email,
    string PhoneNumber,
    DateOnly DateOfBirth,
    string Street,
    string City,
    string PostalCode,
    string Country,
    string LicenseNumber,
    string LicenseIssueCountry,
    DateOnly LicenseIssueDate,
    DateOnly LicenseExpiryDate
);
