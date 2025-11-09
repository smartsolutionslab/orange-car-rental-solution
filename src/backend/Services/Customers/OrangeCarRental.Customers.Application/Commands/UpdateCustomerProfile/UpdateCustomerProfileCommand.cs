using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateCustomerProfile;

/// <summary>
///     Command to update a customer's profile information.
///     Uses value objects for type safety and early validation.
///     Does not include email or driver's license (use separate commands for those).
/// </summary>
public sealed record UpdateCustomerProfileCommand : ICommand<UpdateCustomerProfileResult>
{
    /// <summary>
    ///     The unique identifier of the customer to update.
    /// </summary>
    public required Guid CustomerIdentifier { get; init; }

    /// <summary>
    ///     Customer's updated name (includes first name, last name, and optional salutation).
    /// </summary>
    public required CustomerName Name { get; init; }

    /// <summary>
    ///     Customer's updated phone number (value object with German format validation).
    /// </summary>
    public required PhoneNumber PhoneNumber { get; init; }

    /// <summary>
    ///     Customer's updated address (value object containing street, city, postal code, and country).
    /// </summary>
    public required Address Address { get; init; }
}
