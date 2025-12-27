namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;

/// <summary>
///     Data transfer object for customer information.
///     Maps from Customer aggregate with all properties for display.
/// </summary>
public sealed record CustomerDto(
    Guid Id,
    string FirstName,
    string LastName,
    string FullName,
    string Email,
    string PhoneNumber,
    string PhoneNumberFormatted,
    DateOnly DateOfBirth,
    int Age,
    AddressDto? Address,
    DriversLicenseDto? DriversLicense,
    string Status,
    bool CanMakeReservation,
    DateTime RegisteredAtUtc,
    DateTime UpdatedAtUtc);
