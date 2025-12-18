namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateDriversLicense;

/// <summary>
///     Result of driver's license update operation.
/// </summary>
public sealed record UpdateDriversLicenseResult(
    Guid CustomerIdentifier,
    bool Success,
    string Message,
    DateTime UpdatedAtUtc);
