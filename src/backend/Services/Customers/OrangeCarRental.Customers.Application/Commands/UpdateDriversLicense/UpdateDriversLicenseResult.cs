namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateDriversLicense;

/// <summary>
/// Result of driver's license update operation.
/// </summary>
public sealed record UpdateDriversLicenseResult
{
    /// <summary>
    /// The unique identifier of the updated customer.
    /// </summary>
    public required Guid CustomerId { get; init; }

    /// <summary>
    /// Indicates if the license was successfully updated.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Status message describing the update result.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Date and time when the license was updated (UTC).
    /// </summary>
    public required DateTime UpdatedAtUtc { get; init; }
}
