using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.GetCustomerByEmail;

/// <summary>
///     Query to retrieve a customer by their email address.
/// </summary>
public sealed record GetCustomerByEmailQuery : IQuery<CustomerDto?>
{
    /// <summary>
    ///     The email address of the customer to retrieve.
    /// </summary>
    public required Email Email { get; init; }
}
