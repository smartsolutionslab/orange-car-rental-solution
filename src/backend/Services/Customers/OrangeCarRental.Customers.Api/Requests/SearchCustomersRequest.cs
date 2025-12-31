namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Requests;

/// <summary>
///     Request DTO for searching customers with filtering, sorting, and pagination.
///     Accepts primitives from query string and maps to SearchCustomersQuery with value objects.
/// </summary>
public sealed record SearchCustomersRequest(
    string? SearchTerm,
    string? Email,
    string? PhoneNumber,
    string? Status,
    string? City,
    string? PostalCode,
    int? MinAge,
    int? MaxAge,
    int? LicenseExpiringWithinDays,
    DateOnly? RegisteredFrom,
    DateOnly? RegisteredTo,
    string? SortBy,
    bool SortDescending = false,
    int? PageNumber = null,
    int? PageSize = null);
