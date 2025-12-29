using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.ChangeCustomerStatus;

/// <summary>
///     Handler for ChangeCustomerStatusCommand.
///     Loads the customer from database, changes account status, and persists changes.
/// </summary>
public sealed class ChangeCustomerStatusCommandHandler(
    ICustomerRepository repository)
    : ICommandHandler<ChangeCustomerStatusCommand, ChangeCustomerStatusResult>
{
    /// <summary>
    ///     Handles the change customer status command.
    /// </summary>
    /// <param name="command">The command with new status and reason.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status change result with old and new status details.</returns>
    /// <exception cref="InvalidOperationException">Thrown when customer is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when status is invalid or validation fails.</exception>
    public async Task<ChangeCustomerStatusResult> HandleAsync(
        ChangeCustomerStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var (customerId, newStatusValue, reason) = command;
        var newStatus = newStatusValue.ParseCustomerStatus();

        // Load customer from database
        var customer = await repository.GetByIdAsync(customerId, cancellationToken);
        var oldStatus = customer.Status;

        // Execute domain logic (returns new instance - immutable pattern)
        var updatedCustomer = customer.ChangeStatus(newStatus, reason);

        // Persist changes to database
        await repository.UpdateAsync(updatedCustomer, cancellationToken);

        return new ChangeCustomerStatusResult(
            updatedCustomer.Id,
            oldStatus.ToString(),
            updatedCustomer.Status.ToString(),
            true,
            $"Customer status changed from {oldStatus} to {updatedCustomer.Status}",
            updatedCustomer.UpdatedAtUtc
        );
    }
}
