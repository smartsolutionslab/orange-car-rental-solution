using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Enums;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.ChangeCustomerStatus;

/// <summary>
/// Handler for ChangeCustomerStatusCommand.
/// Loads the customer, changes account status, and persists changes.
/// </summary>
public sealed class ChangeCustomerStatusCommandHandler(ICustomerRepository customers)
{
    /// <summary>
    /// Handles the change customer status command.
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
        // Load customer
        var customerId = CustomerId.From(command.CustomerId);
        var customer = await customers.GetByIdAsync(customerId, cancellationToken);

        if (customer is null)
        {
            throw new InvalidOperationException(
                $"Customer with ID '{command.CustomerId}' not found.");
        }

        // Parse and validate new status
        if (!Enum.TryParse<CustomerStatus>(command.NewStatus, ignoreCase: true, out var newStatus))
        {
            throw new ArgumentException(
                $"Invalid customer status: '{command.NewStatus}'. Valid values are: Active, Suspended, Blocked.",
                nameof(command.NewStatus));
        }

        // Store old status before change
        var oldStatus = customer.Status;

        // Change status (domain method handles validation)
        customer.ChangeStatus(newStatus, command.Reason);

        // Persist changes
        await customers.UpdateAsync(customer, cancellationToken);
        await customers.SaveChangesAsync(cancellationToken);

        // Return result
        return new ChangeCustomerStatusResult
        {
            CustomerId = customer.Id.Value,
            OldStatus = oldStatus.ToString(),
            NewStatus = customer.Status.ToString(),
            Success = true,
            Message = $"Customer status changed from {oldStatus} to {customer.Status}",
            UpdatedAtUtc = customer.UpdatedAtUtc
        };
    }
}
