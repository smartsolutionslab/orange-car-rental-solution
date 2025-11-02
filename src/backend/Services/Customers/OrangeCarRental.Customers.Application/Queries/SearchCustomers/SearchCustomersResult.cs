using SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.SearchCustomers;

/// <summary>
/// Result of customer search operation with pagination.
/// Contains matching customers and pagination metadata.
/// </summary>
public sealed record SearchCustomersResult
{
    /// <summary>
    /// List of customers matching the search criteria.
    /// </summary>
    public required List<CustomerDto> Customers { get; init; }

    /// <summary>
    /// Total number of customers matching the search criteria (across all pages).
    /// </summary>
    public required int TotalCount { get; init; }

    /// <summary>
    /// Current page number (1-based).
    /// </summary>
    public required int PageNumber { get; init; }

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    /// Total number of pages.
    /// </summary>
    public required int TotalPages { get; init; }

    /// <summary>
    /// Indicates if there is a previous page.
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;

    /// <summary>
    /// Indicates if there is a next page.
    /// </summary>
    public bool HasNextPage => PageNumber < TotalPages;
}
