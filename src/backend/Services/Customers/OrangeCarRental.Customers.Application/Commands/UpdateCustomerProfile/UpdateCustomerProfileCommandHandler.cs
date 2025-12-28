using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateCustomerProfile;

/// <summary>
///     Handler for UpdateCustomerProfileCommand.
///     Loads the customer from event store, updates profile information, and persists via event sourcing.
/// </summary>
public sealed class UpdateCustomerProfileCommandHandler(
    IEventSourcedCustomerRepository repository)
    : ICommandHandler<UpdateCustomerProfileCommand, UpdateCustomerProfileResult>
{
    /// <summary>
    ///     Handles the update customer profile command.
    /// </summary>
    /// <param name="command">The update command with new profile data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Update result with success status and details.</returns>
    /// <exception cref="InvalidOperationException">Thrown when customer is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public async Task<UpdateCustomerProfileResult> HandleAsync(
        UpdateCustomerProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var (customerId, name, phoneNumber, address) = command;

        // Load customer from event store
        var customer = await repository.LoadAsync(customerId, cancellationToken);
        if (!customer.State.HasBeenCreated)
        {
            throw new InvalidOperationException($"Customer with ID '{customerId.Value}' not found.");
        }

        // Execute domain logic
        customer.UpdateProfile(name, phoneNumber, address);

        // Persist events to event store
        await repository.SaveAsync(customer, cancellationToken);

        return new UpdateCustomerProfileResult(
            customer.Id.Value,
            true,
            "Customer profile updated successfully",
            customer.UpdatedAtUtc);
    }
}
