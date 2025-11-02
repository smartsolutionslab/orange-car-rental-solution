using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateDriversLicense;

/// <summary>
/// Handler for UpdateDriversLicenseCommand.
/// Loads the customer, updates driver's license information, and persists changes.
/// </summary>
public sealed class UpdateDriversLicenseCommandHandler(ICustomerRepository repository)
{
    /// <summary>
    /// Handles the update driver's license command.
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
        // Load customer
        var customerId = CustomerId.From(command.CustomerId);
        var customer = await repository.GetByIdAsync(customerId, cancellationToken);

        if (customer is null)
        {
            throw new InvalidOperationException(
                $"Customer with ID '{command.CustomerId}' not found.");
        }

        // Create driver's license value object from command data
        var driversLicense = DriversLicense.Of(
            command.LicenseNumber,
            command.IssueCountry,
            command.IssueDate,
            command.ExpiryDate);

        // Update driver's license (domain method handles validation)
        customer.UpdateDriversLicense(driversLicense);

        // Persist changes
        await repository.UpdateAsync(customer, cancellationToken);
        await repository.SaveChangesAsync(cancellationToken);

        // Return result
        return new UpdateDriversLicenseResult
        {
            CustomerId = customer.Id.Value,
            Success = true,
            Message = "Driver's license updated successfully",
            UpdatedAtUtc = customer.UpdatedAtUtc
        };
    }
}
