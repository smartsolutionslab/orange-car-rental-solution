namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateCustomerProfile;

/// <summary>
///     Result of customer profile update operation.
/// </summary>
public sealed record UpdateCustomerProfileResult
{
    /// <summary>
    ///     The unique identifier of the updated customer.
    /// </summary>
    public required Guid CustomerIdentifier { get; init; }

    /// <summary>
    ///     Indicates if the profile was successfully updated.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    ///     Status message describing the update result.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    ///     Date and time when the profile was updated (UTC).
    /// </summary>
    public required DateTime UpdatedAtUtc { get; init; }
}
