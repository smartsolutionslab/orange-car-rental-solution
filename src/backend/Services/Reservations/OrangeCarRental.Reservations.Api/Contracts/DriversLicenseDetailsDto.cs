namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Contracts;

/// <summary>
///     Driver's license details.
/// </summary>
public sealed record DriversLicenseDetailsDto(
    string LicenseNumber,
    string LicenseIssueCountry,
    DateOnly LicenseIssueDate,
    DateOnly LicenseExpiryDate);