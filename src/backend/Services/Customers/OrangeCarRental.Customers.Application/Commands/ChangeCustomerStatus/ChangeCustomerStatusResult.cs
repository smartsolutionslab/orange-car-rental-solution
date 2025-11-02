namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.ChangeCustomerStatus;

/// <summary>
/// Result of customer status change operation.
/// </summary>
public sealed record ChangeCustomerStatusResult
{
    /// <summary>
    /// The unique identifier of the updated customer.
    /// </summary>
    public required Guid CustomerId { get; init; }

    /// <summary>
    /// The previous status of the customer.
    /// </summary>
    public required string OldStatus { get; init; }

    /// <summary>
    /// The new status of the customer.
    /// </summary>
    public required string NewStatus { get; init; }

    /// <summary>
    /// Indicates if the status was successfully changed.
    /// </summary>
    public required bool Success { get; init; }

    /// <summary>
    /// Status message describing the change result.
    /// </summary>
    public required string Message { get; init; }

    /// <summary>
    /// Date and time when the status was changed (UTC).
    /// </summary>
    public required DateTime UpdatedAtUtc { get; init; }
}
