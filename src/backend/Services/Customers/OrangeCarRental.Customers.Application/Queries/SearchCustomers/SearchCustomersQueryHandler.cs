using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.SearchCustomers;

/// <summary>
/// Handler for SearchCustomersQuery.
/// Delegates filtering and pagination to the repository for database-level performance.
/// </summary>
public sealed class SearchCustomersQueryHandler(ICustomerRepository customers)
{
    /// <summary>
    /// Handles the search customers query.
    /// </summary>
    /// <param name="query">The search query with filters and pagination.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Search result with matching customers and pagination metadata.</returns>
    public async Task<SearchCustomersResult> HandleAsync(
        SearchCustomersQuery query,
        CancellationToken cancellationToken = default)
    {
        // Convert query to domain search parameters
        var searchParameters = query.ToSearchParameters();

        // Validate parameters
        searchParameters.Validate();

        // Execute search in repository
        var pagedResult = await customers.SearchAsync(searchParameters, cancellationToken);

        // Map to DTOs and return result
        return pagedResult.ToDto();
    }
}
