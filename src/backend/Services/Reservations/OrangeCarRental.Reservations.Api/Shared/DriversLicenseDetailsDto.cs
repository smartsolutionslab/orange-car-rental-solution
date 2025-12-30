namespace SmartSolutionsLab.OrangeCarRental.Reservations.Api.Shared;

/// <summary>
///     Driver's license details.
/// </summary>
public sealed record DriversLicenseDetailsDto(
    string LicenseNumber,
    string LicenseIssueCountry,
    DateOnly LicenseIssueDate,
    DateOnly LicenseExpiryDate);
