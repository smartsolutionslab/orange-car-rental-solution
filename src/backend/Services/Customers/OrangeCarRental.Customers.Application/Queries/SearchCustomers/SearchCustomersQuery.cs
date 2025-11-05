namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.SearchCustomers;

/// <summary>
///     Query to search customers with filtering, sorting, and pagination.
///     Wraps CustomerSearchParameters from the domain layer.
/// </summary>
public sealed record SearchCustomersQuery
{
    /// <summary>
    ///     Search by customer name (first or last name).
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    ///     Filter by email address.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    ///     Filter by phone number.
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    ///     Filter by customer status (Active, Suspended, Blocked).
    /// </summary>
    public string? Status { get; init; }

    /// <summary>
    ///     Filter by city.
    /// </summary>
    public string? City { get; init; }

    /// <summary>
    ///     Filter by postal code.
    /// </summary>
    public string? PostalCode { get; init; }

    /// <summary>
    ///     Filter by minimum age.
    /// </summary>
    public int? MinAge { get; init; }

    /// <summary>
    ///     Filter by maximum age.
    /// </summary>
    public int? MaxAge { get; init; }

    /// <summary>
    ///     Filter customers with expiring licenses (within N days).
    /// </summary>
    public int? LicenseExpiringWithinDays { get; init; }

    /// <summary>
    ///     Filter by registration date range (from).
    /// </summary>
    public DateTime? RegisteredFrom { get; init; }

    /// <summary>
    ///     Filter by registration date range (to).
    /// </summary>
    public DateTime? RegisteredTo { get; init; }

    /// <summary>
    ///     Sort field name (e.g., "LastName", "RegisteredAtUtc").
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    ///     Sort in descending order.
    /// </summary>
    public bool SortDescending { get; init; }

    /// <summary>
    ///     Page number (1-based). Defaults to 1.
    /// </summary>
    public int? PageNumber { get; init; }

    /// <summary>
    ///     Number of items per page. Defaults to 20.
    /// </summary>
    public int? PageSize { get; init; }
}
