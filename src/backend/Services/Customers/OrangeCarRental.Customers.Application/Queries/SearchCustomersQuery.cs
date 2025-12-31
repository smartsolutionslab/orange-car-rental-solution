using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.CQRS;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries;

/// <summary>
///     Query to search customers with filtering, sorting, and pagination.
///     Uses value objects for type-safe filtering.
/// </summary>
public sealed record SearchCustomersQuery(
    SearchTerm? SearchTerm,
    Email? Email,
    PhoneNumber? PhoneNumber,
    CustomerStatus? Status,
    City? City,
    PostalCode? PostalCode,
    IntRange? AgeRange,
    IntRange? LicenseExpiringDays,
    DateRange? RegisteredDateRange,
    PagingInfo Paging,
    SortingInfo Sorting
) : IQuery<PagedResult<CustomerDto>>;
