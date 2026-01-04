using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands;

/// <summary>
///     Handler for UpdateCustomerProfileCommand.
///     Loads the customer from database, updates profile information, and persists changes.
/// </summary>
public sealed class UpdateCustomerProfileCommandHandler(
    ICustomerRepository repository,
    IUnitOfWork unitOfWork)
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

        // Load customer from database
        var customer = await repository.GetByIdAsync(customerId, cancellationToken);

        // Execute domain logic (returns new instance - immutable pattern)
        var updatedCustomer = customer.UpdateProfile(name, phoneNumber, address);

        // Persist changes to database
        await repository.UpdateAsync(updatedCustomer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateCustomerProfileResult(
            updatedCustomer.Id.Value,
            true,
            "Customer profile updated successfully",
            updatedCustomer.UpdatedAtUtc);
    }
}
