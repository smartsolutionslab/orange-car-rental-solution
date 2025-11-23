using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.UpdateDriversLicense;

/// <summary>
///     Command to update a customer's driver's license information.
///     Uses value object for type safety and early validation.
///     Used when a customer renews their license or provides updated license details.
/// </summary>
public sealed record UpdateDriversLicenseCommand(
    CustomerIdentifier CustomerIdentifier,
    DriversLicense DriversLicense) : ICommand<UpdateDriversLicenseResult>;
