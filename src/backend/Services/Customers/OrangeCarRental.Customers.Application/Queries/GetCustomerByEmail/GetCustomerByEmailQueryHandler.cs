using SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.SearchCustomers;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.GetCustomerByEmail;

/// <summary>
/// Handler for GetCustomerByEmailQuery.
/// Retrieves a customer by email address and maps to DTO.
/// </summary>
public sealed class GetCustomerByEmailQueryHandler(ICustomerRepository customers)
{
    /// <summary>
    /// Handles the get customer by email query.
    /// </summary>
    /// <param name="query">The query with email address.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Customer DTO if found, otherwise null.</returns>
    public async Task<CustomerDto?> HandleAsync(
        GetCustomerByEmailQuery query,
        CancellationToken cancellationToken = default)
    {
        var email = Email.Of(query.Email);
        var customer = await customers.GetByEmailAsync(email, cancellationToken);

        return customer?.ToDto();
    }
}
