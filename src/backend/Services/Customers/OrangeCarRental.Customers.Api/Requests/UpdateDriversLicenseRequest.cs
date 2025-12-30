namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Requests;

/// <summary>
///     Request DTO for updating driver's license.
///     Accepts primitives from HTTP requests and maps to UpdateDriversLicenseCommand with value objects.
/// </summary>
public sealed record UpdateDriversLicenseRequest(
    string LicenseNumber,
    string IssueCountry,
    DateOnly IssueDate,
    DateOnly ExpiryDate);
