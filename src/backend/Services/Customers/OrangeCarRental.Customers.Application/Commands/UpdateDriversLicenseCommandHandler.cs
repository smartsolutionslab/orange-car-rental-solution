using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands;

/// <summary>
///     Handler for UpdateDriversLicenseCommand.
///     Loads the customer from database, updates driver's license information, and persists changes.
/// </summary>
public sealed class UpdateDriversLicenseCommandHandler(
    ICustomerRepository repository)
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

        // Load customer from database
        var customer = await repository.GetByIdAsync(customerId, cancellationToken);

        // Execute domain logic (returns new instance - immutable pattern)
        var updatedCustomer = customer.UpdateDriversLicense(driversLicense);

        // Persist changes to database
        await repository.UpdateAsync(updatedCustomer, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        return new UpdateDriversLicenseResult(
            updatedCustomer.Id.Value,
            true,
            "Driver's license updated successfully",
            updatedCustomer.UpdatedAtUtc);
    }
}
