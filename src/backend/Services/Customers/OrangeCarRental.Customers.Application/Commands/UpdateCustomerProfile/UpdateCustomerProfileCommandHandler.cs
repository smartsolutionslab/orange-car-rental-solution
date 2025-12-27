using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Services;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateCustomerProfile;

/// <summary>
///     Handler for UpdateCustomerProfileCommand.
///     Loads the customer, updates profile information, and persists via event sourcing.
/// </summary>
public sealed class UpdateCustomerProfileCommandHandler(ICustomerCommandService commandService)
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

        // Update profile via event-sourced command service
        var customer = await commandService.UpdateProfileAsync(
            customerId,
            name,
            phoneNumber,
            address,
            cancellationToken);

        return new UpdateCustomerProfileResult(
            customer.Id.Value,
            true,
            "Customer profile updated successfully",
            customer.UpdatedAtUtc);
    }
}
