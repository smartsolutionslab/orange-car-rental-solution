using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateCustomerProfile;

/// <summary>
///     Handler for UpdateCustomerProfileCommand.
///     Loads the customer, updates profile information, and persists changes.
/// </summary>
public sealed class UpdateCustomerProfileCommandHandler(ICustomerRepository customers)
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
    public async Task<UpdateCustomerProfileResult> HandleAsync(UpdateCustomerProfileCommand command, CancellationToken cancellationToken = default)
    {
        var (customerId, name, phoneNumber, address ) = command;

        // Load customer
        var customer = await customers.GetByIdAsync(customerId, cancellationToken)
                       ?? throw new InvalidOperationException($"Customer with ID '{command.CustomerId}' not found.");

        // Update profile (domain method handles validation and returns new instance)
        customer = customer.UpdateProfile(name, phoneNumber, address);

        // Persist changes (repository updates with the new immutable instance)
        await customers.UpdateAsync(customer, cancellationToken);
        await customers.SaveChangesAsync(cancellationToken);

        // Return result
        return new UpdateCustomerProfileResult(CustomerIdentifier: customer.Id.Value, Success: true,
            Message: "Customer profile updated successfully", UpdatedAtUtc: customer.UpdatedAtUtc);
    }
}
