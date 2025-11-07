using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.GetCustomer;

/// <summary>
///     Query to retrieve a customer by their unique identifier.
/// </summary>
public sealed record GetCustomerQuery
{
    /// <summary>
    ///     The unique identifier of the customer to retrieve.
    /// </summary>
    public required CustomerIdentifier CustomerIdentifier { get; init; }
}
