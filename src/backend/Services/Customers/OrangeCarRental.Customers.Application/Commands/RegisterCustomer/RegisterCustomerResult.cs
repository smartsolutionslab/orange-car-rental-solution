namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Commands.RegisterCustomer;

/// <summary>
///     Result of customer registration operation.
///     Contains the newly created customer's identifier and confirmation details.
/// </summary>
public sealed record RegisterCustomerResult(
    Guid CustomerIdentifier,
    string Email,
    string Status,
    DateTime RegisteredAtUtc);
