using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Parameters for searching customers with filtering, sorting, and pagination.
/// </summary>
public sealed record CustomerSearchParameters(
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
    SortingInfo Sorting) : SearchParameters(Paging, Sorting);
