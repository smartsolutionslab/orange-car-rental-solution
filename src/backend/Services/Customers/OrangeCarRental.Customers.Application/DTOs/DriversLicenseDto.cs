namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;

/// <summary>
/// Data transfer object for driver's license information.
/// Maps from DriversLicense value object.
/// </summary>
public sealed record DriversLicenseDto
{
    /// <summary>
    /// The license number (alphanumeric, normalized to uppercase).
    /// </summary>
    public required string LicenseNumber { get; init; }

    /// <summary>
    /// The country that issued the license (e.g., "Germany").
    /// </summary>
    public required string IssueCountry { get; init; }

    /// <summary>
    /// The date the license was issued.
    /// </summary>
    public required DateOnly IssueDate { get; init; }

    /// <summary>
    /// The date the license expires.
    /// </summary>
    public required DateOnly ExpiryDate { get; init; }

    /// <summary>
    /// Indicates if the license is currently valid (not expired).
    /// </summary>
    public required bool IsValid { get; init; }

    /// <summary>
    /// Indicates if the license is from an EU member state.
    /// </summary>
    public required bool IsEuLicense { get; init; }

    /// <summary>
    /// Number of days until the license expires.
    /// Negative if already expired.
    /// </summary>
    public required int DaysUntilExpiry { get; init; }
}
