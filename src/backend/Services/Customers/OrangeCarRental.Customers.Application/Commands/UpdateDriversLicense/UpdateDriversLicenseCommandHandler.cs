using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateDriversLicense;

/// <summary>
///     Handler for UpdateDriversLicenseCommand.
///     Loads the customer, updates driver's license information, and persists changes.
/// </summary>
public sealed class UpdateDriversLicenseCommandHandler(ICustomerRepository customers)
    : ICommandHandler<UpdateDriversLicenseCommand, UpdateDriversLicenseResult>
{
    /// <summary>
    ///     Handles the update driver's license command.
    /// </summary>
    /// <param name="command">The update command with new license data.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Update result with success status and details.</returns>
    /// <exception cref="BuildingBlocks.Domain.Exceptions.EntityNotFoundException">Thrown when customer is not found.</exception>
    /// <exception cref="ArgumentException">Thrown when validation fails.</exception>
    public async Task<UpdateDriversLicenseResult> HandleAsync(
        UpdateDriversLicenseCommand command,
        CancellationToken cancellationToken = default)
    {
        // Load customer (throws EntityNotFoundException if not found)
        var customer = await customers.GetByIdAsync(command.CustomerIdentifier, cancellationToken);

        // Update driver's license (domain method handles validation and returns new instance)
        customer = customer.UpdateDriversLicense(command.DriversLicense);

        // Persist changes (repository updates with the new immutable instance)
        await customers.UpdateAsync(customer, cancellationToken);
        await customers.SaveChangesAsync(cancellationToken);

        // Return result
        return new UpdateDriversLicenseResult
        {
            CustomerIdentifier = customer.Id.Value,
            Success = true,
            Message = "Driver's license updated successfully",
            UpdatedAtUtc = customer.UpdatedAtUtc
        };
    }
}
