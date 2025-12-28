using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.ChangeCustomerStatus;

/// <summary>
///     Handler for ChangeCustomerStatusCommand.
///     Loads the customer from event store, changes account status, and persists via event sourcing.
/// </summary>
public sealed class ChangeCustomerStatusCommandHandler(
    IEventSourcedCustomerRepository repository)
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

        // Load customer from event store
        var customer = await repository.LoadAsync(customerId, cancellationToken);
        if (!customer.State.HasBeenCreated)
        {
            throw new InvalidOperationException($"Customer with ID '{customerId.Value}' not found.");
        }

        var oldStatus = customer.Status;

        // Execute domain logic
        customer.ChangeStatus(newStatus, reason);

        // Persist events to event store
        await repository.SaveAsync(customer, cancellationToken);

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
