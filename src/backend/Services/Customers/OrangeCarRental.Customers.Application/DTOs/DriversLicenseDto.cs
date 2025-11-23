namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;

/// <summary>
///     Data transfer object for driver's license information.
///     Maps from DriversLicense value object.
/// </summary>
public sealed record DriversLicenseDto(
    string LicenseNumber,
    string IssueCountry,
    DateOnly IssueDate,
    DateOnly ExpiryDate,
    bool IsValid,
    bool IsEuLicense,
    int DaysUntilExpiry);
