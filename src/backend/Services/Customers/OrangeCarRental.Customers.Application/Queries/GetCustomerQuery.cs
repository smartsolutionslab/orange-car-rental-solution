using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries;

/// <summary>
///     Query to retrieve a customer by their unique identifier.
/// </summary>
public sealed record GetCustomerQuery(
    CustomerIdentifier CustomerIdentifier
) : IQuery<CustomerDto>;
