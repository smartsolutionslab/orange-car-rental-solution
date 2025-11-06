namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Contracts;

/// <summary>
///     Request DTO for updating driver's license.
///     Accepts primitives from HTTP requests and maps to UpdateDriversLicenseCommand with value objects.
/// </summary>
public sealed record UpdateDriversLicenseRequest
{
    public required string LicenseNumber { get; init; }
    public required string IssueCountry { get; init; }
    public required DateOnly IssueDate { get; init; }
    public required DateOnly ExpiryDate { get; init; }
}
