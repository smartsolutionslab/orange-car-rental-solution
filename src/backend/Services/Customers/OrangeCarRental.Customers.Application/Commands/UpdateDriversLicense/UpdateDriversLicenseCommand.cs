namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateDriversLicense;

/// <summary>
/// Command to update a customer's driver's license information.
/// Used when a customer renews their license or provides updated license details.
/// </summary>
public sealed record UpdateDriversLicenseCommand
{
    /// <summary>
    /// The unique identifier of the customer to update.
    /// </summary>
    public required Guid CustomerId { get; init; }

    /// <summary>
    /// Updated driver's license number (alphanumeric).
    /// </summary>
    public required string LicenseNumber { get; init; }

    /// <summary>
    /// Updated country that issued the driver's license (e.g., "Germany").
    /// </summary>
    public required string IssueCountry { get; init; }

    /// <summary>
    /// Updated date the driver's license was issued.
    /// </summary>
    public required DateOnly IssueDate { get; init; }

    /// <summary>
    /// Updated date the driver's license expires.
    /// Must be valid for at least 30 days.
    /// </summary>
    public required DateOnly ExpiryDate { get; init; }
}
