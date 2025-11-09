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
    int? MinAge,
    int? MaxAge,
    int? LicenseExpiringWithinDays,
    DateTime? RegisteredFrom,
    DateTime? RegisteredTo,
    string? SortBy,
    bool SortDescending,
    int PageNumber,
    int PageSize)
    : SearchParameters(PageNumber, PageSize, SortBy, SortDescending)
{
    /// <summary>
    ///     Validates the search parameters.
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
