using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

/// <summary>
///     Parameters for searching customers with filtering, sorting, and pagination.
/// </summary>
public sealed record CustomerSearchParameters : SearchParameters
{
    public SearchTerm? SearchTerm { get; init; }
    public Email? Email { get; init; }
    public PhoneNumber? PhoneNumber { get; init; }
    public CustomerStatus? Status { get; init; }
    public City? City { get; init; }
    public PostalCode? PostalCode { get; init; }
    public IntRange? AgeRange { get; init; }
    public IntRange? LicenseExpiringDays { get; init; }
    public DateRange? RegisteredDateRange { get; init; }

    public CustomerSearchParameters(PagingInfo paging) : base(paging, SortingInfo.None)
    {
    }

    public CustomerSearchParameters(PagingInfo paging, SortingInfo sorting) : base(paging, sorting)
    {
    }

    /// <summary>
    ///     Creates search parameters with default paging.
    /// </summary>
    public static CustomerSearchParameters Default() => new(PagingInfo.Default);

    /// <summary>
    ///     Creates search parameters with specific paging.
    /// </summary>
    public static CustomerSearchParameters WithPaging(int pageNumber, int pageSize) =>
        new(PagingInfo.Create(pageNumber, pageSize));

    /// <summary>
    ///     Creates search parameters with a search term.
    /// </summary>
    public static CustomerSearchParameters WithSearchTerm(SearchTerm searchTerm, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { SearchTerm = searchTerm };

    /// <summary>
    ///     Creates search parameters filtered by email.
    /// </summary>
    public static CustomerSearchParameters ForEmail(Email email, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { Email = email };

    /// <summary>
    ///     Creates search parameters filtered by status.
    /// </summary>
    public static CustomerSearchParameters ForStatus(CustomerStatus status, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { Status = status };

    /// <summary>
    ///     Creates search parameters filtered by city.
    /// </summary>
    public static CustomerSearchParameters ForCity(City city, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { City = city };

    /// <summary>
    ///     Creates search parameters filtered by age range.
    /// </summary>
    public static CustomerSearchParameters WithAgeRange(IntRange ageRange, PagingInfo? paging = null) =>
        new(paging ?? PagingInfo.Default) { AgeRange = ageRange };

    /// <summary>
    ///     Validates the search parameters.
    /// </summary>
    public override void Validate()
    {
        base.Validate();
        // Additional domain-specific validation can be added here
    }
}
