using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.ChangeCustomerStatus;

/// <summary>
///     Handler for ChangeCustomerStatusCommand.
///     Loads the customer, changes account status, and persists changes.
/// </summary>
public sealed class ChangeCustomerStatusCommandHandler(ICustomerRepository customers)
    : ICommandHandler<ChangeCustomerStatusCommand, ChangeCustomerStatusResult>
{
    /// <summary>
    ///     Handles the change customer status command.
    /// </summary>
    /// <param name="command">The command with new status and reason.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Status change result with old and new status details.</returns>
    /// <exception cref="BuildingBlocks.Domain.Exceptions.EntityNotFoundException">Thrown when customer is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when status is invalid or validation fails.</exception>
    public async Task<ChangeCustomerStatusResult> HandleAsync(
        ChangeCustomerStatusCommand command,
        CancellationToken cancellationToken = default)
    {
        var (customerId, newStatusValue, reason) = command;

        var customer = await customers.GetByIdAsync(customerId, cancellationToken);
        var newStatus = newStatusValue.Parse();

        var oldStatus = customer.Status;

        customer = customer.ChangeStatus(newStatus, reason);

        await customers.UpdateAsync(customer, cancellationToken);
        await customers.SaveChangesAsync(cancellationToken);

        return new ChangeCustomerStatusResult(
            customer.Id,
            oldStatus.ToString(),
            customer.Status.ToString(),
            true,
            $"Customer status changed from {oldStatus} to {customer.Status}",
            customer.UpdatedAtUtc
        );
    }


}
