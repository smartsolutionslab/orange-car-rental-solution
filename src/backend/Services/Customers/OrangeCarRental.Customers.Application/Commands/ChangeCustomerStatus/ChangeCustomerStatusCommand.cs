using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.ChangeCustomerStatus;

/// <summary>
///     Command to change a customer's account status.
///     Used to activate, suspend, or block customer accounts.
/// </summary>
public sealed record ChangeCustomerStatusCommand
{
    /// <summary>
    ///     The unique identifier of the customer to update.
    /// </summary>
    public required CustomerIdentifier CustomerIdentifier { get; init; }

    /// <summary>
    ///     The new status for the customer account (Active, Suspended, Blocked).
    /// </summary>
    public required string NewStatus { get; init; }

    /// <summary>
    ///     Reason for the status change (required for audit trail).
    /// </summary>
    public required string Reason { get; init; }
}
