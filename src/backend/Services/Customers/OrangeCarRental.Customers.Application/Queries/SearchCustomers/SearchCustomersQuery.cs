using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.SearchCustomers;

/// <summary>
///     Query to search customers with filtering, sorting, and pagination.
///     Wraps CustomerSearchParameters from the domain layer.
/// </summary>
public sealed record SearchCustomersQuery(
    string? SearchTerm,
    string? Email,
    string? PhoneNumber,
    string? Status,
    string? City,
    string? PostalCode,
    int? MinAge,
    int? MaxAge,
    int? LicenseExpiringWithinDays,
    DateTime? RegisteredFrom,
    DateTime? RegisteredTo,
    string? SortBy,
    bool SortDescending,
    int? PageNumber,
    int? PageSize
) : IQuery<SearchCustomersResult>;
