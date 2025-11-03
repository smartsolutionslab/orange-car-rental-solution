
namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
/// Parameters for searching customers with filtering, sorting, and pagination.
/// </summary>
public sealed class CustomerSearchParameters
{
    /// <summary>
    /// Search by customer name (first or last name).
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filter by email address.
    /// </summary>
    public string? Email { get; init; }

    /// <summary>
    /// Filter by phone number.
    /// </summary>
    public string? PhoneNumber { get; init; }

    /// <summary>
    /// Filter by customer status.
    /// </summary>
    public CustomerStatus? Status { get; init; }

    /// <summary>
    /// Filter by city.
    /// </summary>
    public string? City { get; init; }

    /// <summary>
    /// Filter by postal code.
    /// </summary>
    public string? PostalCode { get; init; }

    /// <summary>
    /// Filter by minimum age.
    /// </summary>
    public int? MinAge { get; init; }

    /// <summary>
    /// Filter by maximum age.
    /// </summary>
    public int? MaxAge { get; init; }

    /// <summary>
    /// Filter customers with expiring licenses (within N days).
    /// </summary>
    public int? LicenseExpiringWithinDays { get; init; }

    /// <summary>
    /// Filter by registration date range (from).
    /// </summary>
    public DateTime? RegisteredFrom { get; init; }

    /// <summary>
    /// Filter by registration date range (to).
    /// </summary>
    public DateTime? RegisteredTo { get; init; }

    /// <summary>
    /// Sort field name.
    /// </summary>
    public string? SortBy { get; init; }

    /// <summary>
    /// Sort in descending order.
    /// </summary>
    public bool SortDescending { get; init; }

    /// <summary>
    /// Page number (1-based).
    /// </summary>
    public int PageNumber { get; init; } = 1;

    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int PageSize { get; init; } = 20;

    /// <summary>
    /// Validates the search parameters.
    /// </summary>
    public void Validate()
    {
        if (PageNumber < 1)
            throw new ArgumentException("Page number must be at least 1", nameof(PageNumber));

        if (PageSize < 1 || PageSize > 100)
            throw new ArgumentException("Page size must be between 1 and 100", nameof(PageSize));

        if (MinAge.HasValue && MinAge < 0)
            throw new ArgumentException("Minimum age cannot be negative", nameof(MinAge));

        if (MaxAge.HasValue && MaxAge < 0)
            throw new ArgumentException("Maximum age cannot be negative", nameof(MaxAge));

        if (MinAge.HasValue && MaxAge.HasValue && MinAge > MaxAge)
            throw new ArgumentException("Minimum age cannot be greater than maximum age");

        if (RegisteredFrom.HasValue && RegisteredTo.HasValue && RegisteredFrom > RegisteredTo)
            throw new ArgumentException("RegisteredFrom cannot be after RegisteredTo");
    }
}
