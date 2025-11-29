using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateCustomerProfile;

/// <summary>
///     Handler for UpdateCustomerProfileCommand.
///     Loads the customer, updates profile information, and persists changes.
/// </summary>
public sealed class UpdateCustomerProfileCommandHandler(ICustomersUnitOfWork unitOfWork)
    : ICommandHandler<UpdateCustomerProfileCommand, UpdateCustomerProfileResult>
{
    /// <summary>
    ///     Handles the update customer profile command.
    /// </summary>
    /// <param name="command">The update command with new profile data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Update result with success status and details.</returns>
    /// <exception cref="BuildingBlocks.Domain.Exceptions.EntityNotFoundException">Thrown when customer is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public async Task<UpdateCustomerProfileResult> HandleAsync(
        UpdateCustomerProfileCommand command,
        CancellationToken cancellationToken = default)
    {
        var (customerId, name, phoneNumber, address) = command;

        var customers = unitOfWork.Customers;
        var customer = await customers.GetByIdAsync(customerId, cancellationToken);
        customer = customer.UpdateProfile(name, phoneNumber, address);

        // Persist changes (repository updates with the new immutable instance)
        await customers.UpdateAsync(customer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateCustomerProfileResult(
            customer.Id.Value,
            true,
            "Customer profile updated successfully",
            customer.UpdatedAtUtc);
    }
}
