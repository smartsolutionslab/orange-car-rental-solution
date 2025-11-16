using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.ChangeCustomerStatus;

/// <summary>
///     Result of customer status change operation.
/// </summary>
public sealed record ChangeCustomerStatusResult(
    CustomerIdentifier CustomerId,
    string OldStatus,
    string NewStatus,
    bool Success,
    string Message,
    DateTime UpdatedAtUtc
);
