using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.ChangeCustomerStatus;

/// <summary>
///     Command to change a customer's account status.
///     Used to activate, suspend, or block customer accounts.
/// </summary>
public sealed record ChangeCustomerStatusCommand(
    CustomerIdentifier CustomerId,
    string NewStatus,
    string Reason
)
    : ICommand<ChangeCustomerStatusResult>;
