using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries.SearchCustomers;

/// <summary>
///     Extension methods for mapping between domain objects and DTOs.
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    ///     Maps a Customer aggregate to a CustomerDto.
    /// </summary>
    public static CustomerDto ToDto(this Customer customer) => new(
        customer.Id.Value,
        customer.Name.FirstName.Value,
        customer.Name.LastName.Value,
        customer.FullName,
        customer.Email.Value,
        customer.PhoneNumber.Value,
        customer.PhoneNumber.FormattedValue,
        customer.DateOfBirth,
        customer.Age,
        customer.Address.ToDto(),
        customer.DriversLicense.ToDto(),
        customer.Status.ToString(),
        customer.CanMakeReservation(),
        customer.RegisteredAtUtc,
        customer.UpdatedAtUtc);

    /// <summary>
    ///     Maps an Address value object to an AddressDto.
    /// </summary>
    public static AddressDto ToDto(this Address address) => new
    (
        address.Street,
        address.City.Value,
        address.PostalCode.Value,
        address.Country
    );

    /// <summary>
    ///     Maps a DriversLicense value object to a DriversLicenseDto.
    /// </summary>
    public static DriversLicenseDto ToDto(this DriversLicense license) => new(
        license.LicenseNumber,
        license.IssueCountry,
        license.IssueDate,
        license.ExpiryDate,
        license.IsValid(),
        license.IsEuLicense(),
        license.DaysUntilExpiry());

    /// <summary>
    ///     Maps a PagedResult of Customer aggregates to a SearchCustomersResult.
    /// </summary>
    public static SearchCustomersResult ToDto(this PagedResult<Customer> pagedResult) => new(
        pagedResult.Items.Select(c => c.ToDto()).ToList(),
        pagedResult.TotalCount,
        pagedResult.PageNumber,
        pagedResult.PageSize,
        pagedResult.TotalPages);

    /// <summary>
    ///     Maps a SearchCustomersQuery to CustomerSearchParameters.
    ///     Handles parsing of primitive types to value objects.
    /// </summary>
    public static CustomerSearchParameters ToSearchParameters(this SearchCustomersQuery query)
    {
        return new CustomerSearchParameters(
            SearchTerm.FromNullable(query.SearchTerm),
            Email.FromNullable(query.Email),
            PhoneNumber.FromNullable(query.PhoneNumber),
            query.Status.TryParseCustomerStatus(),
            City.FromNullable(query.City),
            PostalCode.FromNullable(query.PostalCode),
            IntRange.TryCreate(query.MinAge, query.MaxAge, out var ageRange)
                ? ageRange
                : null,
            query.LicenseExpiringWithinDays.HasValue
                ? IntRange.UpTo(query.LicenseExpiringWithinDays.Value)
                : null,
            DateRange.Create(
                query.RegisteredFrom.HasValue ? DateOnly.FromDateTime(query.RegisteredFrom.Value) : null,
                query.RegisteredTo.HasValue ? DateOnly.FromDateTime(query.RegisteredTo.Value) : null),
            PagingInfo.Create(query.PageNumber ?? 1, query.PageSize ?? PagingInfo.DefaultPageSize),
            SortingInfo.Create(query.SortBy, query.SortDescending)
        );
    }


}
