using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.SearchCustomers;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.GetCustomer;

/// <summary>
///     Handler for GetCustomerQuery.
///     Retrieves a customer by ID and maps to DTO.
/// </summary>
public sealed class GetCustomerQueryHandler(ICustomerRepository customers)
    : IQueryHandler<GetCustomerQuery, CustomerDto?>
{
    /// <summary>
    ///     Handles the get customer query.
    /// </summary>
    /// <param name="query">The query with customer ID.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Customer DTO if found, otherwise null.</returns>
    public async Task<CustomerDto?> HandleAsync(
        GetCustomerQuery query,
        CancellationToken cancellationToken = default)
    {
        var customer = await customers.GetByIdAsync(query.CustomerIdentifier, cancellationToken);

        return customer?.ToDto();
    }
}
