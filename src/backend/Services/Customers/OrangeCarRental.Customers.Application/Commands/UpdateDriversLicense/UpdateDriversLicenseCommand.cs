using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateDriversLicense;

/// <summary>
///     Command to update a customer's driver's license information.
///     Uses value object for type safety and early validation.
///     Used when a customer renews their license or provides updated license details.
/// </summary>
public sealed record UpdateDriversLicenseCommand
{
    /// <summary>
    ///     The unique identifier of the customer to update.
    /// </summary>
    public required CustomerIdentifier CustomerIdentifier { get; init; }

    /// <summary>
    ///     Updated driver's license (value object with validation).
    /// </summary>
    public required DriversLicense DriversLicense { get; init; }
}
