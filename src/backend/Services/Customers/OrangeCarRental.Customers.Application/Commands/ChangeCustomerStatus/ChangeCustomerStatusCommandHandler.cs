using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Services;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.ChangeCustomerStatus;

/// <summary>
///     Handler for ChangeCustomerStatusCommand.
///     Loads the customer, changes account status, and persists via event sourcing.
/// </summary>
public sealed class ChangeCustomerStatusCommandHandler(
    ICustomerCommandService commandService,
    ICustomersUnitOfWork unitOfWork)
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

        // Get current status from read model for the response
        var existingCustomer = await unitOfWork.Customers.GetByIdAsync(customerId, cancellationToken);
        var oldStatus = existingCustomer.Status;

        // Change status via event-sourced command service
        var customer = await commandService.ChangeStatusAsync(
            customerId,
            newStatus,
            reason,
            cancellationToken);

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
