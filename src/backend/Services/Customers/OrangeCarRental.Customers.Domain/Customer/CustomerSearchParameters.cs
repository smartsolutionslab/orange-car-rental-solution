using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
/// Parameters for searching customers with filtering, sorting, and pagination.
/// </summary>
public sealed class CustomerSearchParameters : SearchParameters
{
    /// <summary>
    /// Search by customer name (first or last name).
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filter by email address (value object).
    /// </summary>
    public Email? Email { get; init; }

    /// <summary>
    /// Filter by phone number (value object).
    /// </summary>
    public PhoneNumber? PhoneNumber { get; init; }

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
    /// Validates the search parameters.
    /// </summary>
    public override void Validate()
    {
        base.Validate();

        ArgumentOutOfRangeException.ThrowIfLessThan(MinAge ?? 0, 0, nameof(MinAge));
        ArgumentOutOfRangeException.ThrowIfLessThan(MaxAge ?? 0, 0, nameof(MaxAge));

        if (MinAge.HasValue && MaxAge.HasValue && MinAge > MaxAge)
            throw new ArgumentException("Minimum age cannot be greater than maximum age");

        if (RegisteredFrom.HasValue && RegisteredTo.HasValue && RegisteredFrom > RegisteredTo)
            throw new ArgumentException("RegisteredFrom cannot be after RegisteredTo");
    }
}
