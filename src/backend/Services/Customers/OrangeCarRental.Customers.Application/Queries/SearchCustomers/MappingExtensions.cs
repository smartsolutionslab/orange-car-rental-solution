using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
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
    public static CustomerDto ToDto(this Customer customer) => new()
    {
        Id = customer.Id.Value,
        FirstName = customer.FirstName,
        LastName = customer.LastName,
        FullName = customer.FullName,
        Email = customer.Email.Value,
        PhoneNumber = customer.PhoneNumber.Value,
        PhoneNumberFormatted = customer.PhoneNumber.FormattedValue,
        DateOfBirth = customer.DateOfBirth,
        Age = customer.Age,
        Address = customer.Address.ToDto(),
        DriversLicense = customer.DriversLicense.ToDto(),
        Status = customer.Status.ToString(),
        CanMakeReservation = customer.CanMakeReservation(),
        RegisteredAtUtc = customer.RegisteredAtUtc,
        UpdatedAtUtc = customer.UpdatedAtUtc
    };

    /// <summary>
    ///     Maps an Address value object to an AddressDto.
    /// </summary>
    public static AddressDto ToDto(this Address address) => new()
    {
        Street = address.Street,
        City = address.City.Value,
        PostalCode = address.PostalCode.Value,
        Country = address.Country
    };

    /// <summary>
    ///     Maps a DriversLicense value object to a DriversLicenseDto.
    /// </summary>
    public static DriversLicenseDto ToDto(this DriversLicense license) => new()
    {
        LicenseNumber = license.LicenseNumber,
        IssueCountry = license.IssueCountry,
        IssueDate = license.IssueDate,
        ExpiryDate = license.ExpiryDate,
        IsValid = license.IsValid(),
        IsEuLicense = license.IsEuLicense(),
        DaysUntilExpiry = license.DaysUntilExpiry()
    };

    /// <summary>
    ///     Maps a PagedResult of Customer aggregates to a SearchCustomersResult.
    /// </summary>
    public static SearchCustomersResult ToDto(this PagedResult<Customer> pagedResult) => new()
    {
        Customers = pagedResult.Items.Select(c => c.ToDto()).ToList(),
        TotalCount = pagedResult.TotalCount,
        PageNumber = pagedResult.PageNumber,
        PageSize = pagedResult.PageSize,
        TotalPages = pagedResult.TotalPages
    };

    /// <summary>
    ///     Maps a SearchCustomersQuery to CustomerSearchParameters.
    ///     Handles parsing of primitive types to value objects.
    /// </summary>
    public static CustomerSearchParameters ToSearchParameters(this SearchCustomersQuery query)
    {
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

        return new CustomerSearchParameters
        {
            SearchTerm = query.SearchTerm?.Trim(),
            Email = email,
            PhoneNumber = phoneNumber,
            Status = status,
            City = city,
            PostalCode = postalCode,
            MinAge = query.MinAge,
            MaxAge = query.MaxAge,
            LicenseExpiringWithinDays = query.LicenseExpiringWithinDays,
            RegisteredFrom = query.RegisteredFrom,
            RegisteredTo = query.RegisteredTo,
            SortBy = query.SortBy,
            SortDescending = query.SortDescending,
            PageNumber = query.PageNumber ?? 1,
            PageSize = query.PageSize ?? 20
        };
    }
}
