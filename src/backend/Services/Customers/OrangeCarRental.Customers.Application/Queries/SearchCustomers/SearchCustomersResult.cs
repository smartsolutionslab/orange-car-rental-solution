using SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.SearchCustomers;

/// <summary>
///     Result of customer search operation with pagination.
///     Contains matching customers and pagination metadata.
/// </summary>
public sealed record SearchCustomersResult(
    IReadOnlyList<CustomerDto> Customers,
    int TotalCount,
    int PageNumber,
    int PageSize,
    int TotalPages)
{
    /// <summary>
    ///     Indicates if there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    ///     Indicates if there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
