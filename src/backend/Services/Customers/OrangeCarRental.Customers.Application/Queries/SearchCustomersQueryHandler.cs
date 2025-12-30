using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries;

/// <summary>
///     Handler for SearchCustomersQuery.
///     Delegates filtering and pagination to the repository for database-level performance.
/// </summary>
public sealed class SearchCustomersQueryHandler(ICustomerRepository customers)
    : IQueryHandler<SearchCustomersQuery, PagedResult<CustomerDto>>
{
    /// <summary>
    ///     Handles the search customers query.
    /// </summary>
    /// <param name="query">The search query with filters and pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Search result with matching customers and pagination metadata.</returns>
    public async Task<PagedResult<CustomerDto>> HandleAsync(
        SearchCustomersQuery query,
        CancellationToken cancellationToken = default)
    {
        var searchParameters = query.ToSearchParameters();
        searchParameters.Validate();

        var pagedResult = await customers.SearchAsync(searchParameters, cancellationToken);

        return pagedResult.Map(customer => customer.ToDto());
    }
}
