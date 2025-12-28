using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateDriversLicense;

/// <summary>
///     Handler for UpdateDriversLicenseCommand.
///     Loads the customer, updates driver's license information, and persists via repository.
/// </summary>
public sealed class UpdateDriversLicenseCommandHandler(
    ICustomerRepository repository,
    ICustomersUnitOfWork unitOfWork)
    : ICommandHandler<UpdateDriversLicenseCommand, UpdateDriversLicenseResult>
{
    /// <summary>
    ///     Handles the update driver's license command.
    /// </summary>
    /// <param name="command">The update command with new license data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Update result with success status and details.</returns>
    /// <exception cref="InvalidOperationException">Thrown when customer is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public async Task<UpdateDriversLicenseResult> HandleAsync(
        UpdateDriversLicenseCommand command,
        CancellationToken cancellationToken = default)
    {
        var (customerId, driversLicense) = command;

        // Load customer from repository
        var customer = await repository.GetByIdAsync(customerId, cancellationToken)
            ?? throw new InvalidOperationException($"Customer with ID '{customerId.Value}' not found.");

        // Execute domain logic
        customer.UpdateDriversLicense(driversLicense);

        // Persist changes
        await repository.UpdateAsync(customer, cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);

        return new UpdateDriversLicenseResult(
            customer.Id.Value,
            true,
            "Driver's license updated successfully",
            customer.UpdatedAtUtc);
    }
}
