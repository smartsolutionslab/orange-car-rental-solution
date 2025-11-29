namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Contracts;

/// <summary>
///     Driver's license information.
/// </summary>
public sealed record DriversLicenseInfoDto(
    string LicenseNumber,
    string LicenseIssueCountry,
    DateOnly LicenseIssueDate,
    DateOnly LicenseExpiryDate);