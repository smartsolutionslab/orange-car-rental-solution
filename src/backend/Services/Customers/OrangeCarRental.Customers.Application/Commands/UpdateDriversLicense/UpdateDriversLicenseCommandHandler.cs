using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Services;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateDriversLicense;

/// <summary>
///     Handler for UpdateDriversLicenseCommand.
///     Loads the customer, updates driver's license information, and persists via event sourcing.
/// </summary>
public sealed class UpdateDriversLicenseCommandHandler(ICustomerCommandService commandService)
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

        // Update driver's license via event-sourced command service
        var customer = await commandService.UpdateDriversLicenseAsync(
            customerId,
            driversLicense,
            cancellationToken);

        return new UpdateDriversLicenseResult(
            customer.Id.Value,
            true,
            "Driver's license updated successfully",
            customer.UpdatedAtUtc);
    }
}
