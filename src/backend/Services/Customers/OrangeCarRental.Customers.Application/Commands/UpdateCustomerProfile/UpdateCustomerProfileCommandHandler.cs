using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateCustomerProfile;

/// <summary>
///     Handler for UpdateCustomerProfileCommand.
///     Loads the customer, updates profile information, and persists via repository.
/// </summary>
public sealed class UpdateCustomerProfileCommandHandler(
    ICustomerRepository repository,
    ICustomersUnitOfWork unitOfWork)
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

        // Load customer from repository
        var customer = await repository.GetByIdAsync(customerId, cancellationToken)
            ?? throw new InvalidOperationException($"Customer with ID '{customerId.Value}' not found.");

        // Execute domain logic
        customer.UpdateProfile(name, phoneNumber, address);

        // Persist changes
        await repository.UpdateAsync(customer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateCustomerProfileResult(
            customer.Id.Value,
            true,
            "Customer profile updated successfully",
            customer.UpdatedAtUtc);
    }
}
