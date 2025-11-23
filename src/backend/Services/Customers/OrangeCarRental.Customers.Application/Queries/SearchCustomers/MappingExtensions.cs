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
        // Parse search term to value object if provided
        SearchTerm? searchTerm = null;
        if (!string.IsNullOrWhiteSpace(query.SearchTerm))
        {
            try
            {
                searchTerm = SearchTerm.Of(query.SearchTerm.Trim());
            }
            catch (ArgumentException)
            {
                // Invalid search term format (too short/long) - leave as null to filter nothing
            }
        }

        // Parse status if provided
        CustomerStatus? status = null;
        if (!string.IsNullOrWhiteSpace(query.Status))
            if (Enum.TryParse<CustomerStatus>(query.Status, true, out var parsedStatus))
                status = parsedStatus;

        // Parse email to value object if provided
        Email? email = null;
        if (!string.IsNullOrWhiteSpace(query.Email))
        {
            try
            {
                email = Email.Of(query.Email.Trim());
            }
            catch (ArgumentException)
            {
                // Invalid email format - leave as null to filter nothing
            }
        }

        // Parse phone number to value object if provided
        PhoneNumber? phoneNumber = null;
        if (!string.IsNullOrWhiteSpace(query.PhoneNumber))
        {
            try
            {
                phoneNumber = PhoneNumber.Of(query.PhoneNumber.Trim());
            }
            catch (ArgumentException)
            {
                // Invalid phone number format - leave as null to filter nothing
            }
        }

        // Parse city to value object if provided
        City? city = null;
        if (!string.IsNullOrWhiteSpace(query.City))
        {
            try
            {
                city = City.Of(query.City.Trim());
            }
            catch (ArgumentException)
            {
                // Invalid city format - leave as null to filter nothing
            }
        }

        // Parse postal code to value object if provided
        PostalCode? postalCode = null;
        if (!string.IsNullOrWhiteSpace(query.PostalCode))
        {
            try
            {
                postalCode = PostalCode.Of(query.PostalCode.Trim());
            }
            catch (ArgumentException)
            {
                // Invalid postal code format - leave as null to filter nothing
            }
        }

        return new CustomerSearchParameters(
            searchTerm,
            email,
            phoneNumber,
            status,
            city,
            postalCode,
            query.MinAge,
            query.MaxAge,
            query.LicenseExpiringWithinDays,
            query.RegisteredFrom,
            query.RegisteredTo,
            query.SortBy,
            query.SortDescending,
            query.PageNumber ?? 1,
            query.PageSize ?? 20);
    }
}
