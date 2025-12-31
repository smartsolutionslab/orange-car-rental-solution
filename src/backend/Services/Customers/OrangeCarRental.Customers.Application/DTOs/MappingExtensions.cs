using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.ValueObjects;
using SmartSolutionsLab.OrangeCarRental.Customers.Application.Queries;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Application.DTOs;

/// <summary>
///     Extension methods for mapping between domain objects and DTOs.
///     Uses C# 14 Extension Members syntax.
/// </summary>
public static class MappingExtensions
{
    /// <summary>
    ///     C# 14 Extension Members for Customer mapping.
    /// </summary>
    extension(Customer customer)
    {
        /// <summary>
        ///     Maps a Customer aggregate to a CustomerDto.
        /// </summary>
        public CustomerDto ToDto()
        {
            return new CustomerDto(
                customer.Id.Value,
                customer.Name.FirstName.Value,
                customer.Name.LastName.Value,
                customer.FullName,
                customer.Email.Value,
                customer.PhoneNumber.Value,
                customer.PhoneNumber.FormattedValue,
                customer.DateOfBirth.Value,
                customer.Age,
                customer.Address.ToDto(),
                customer.DriversLicense.ToDto(),
                customer.Status.ToString(),
                customer.CanMakeReservation(),
                customer.RegisteredAtUtc,
                customer.UpdatedAtUtc);
        }
    }

    /// <summary>
    ///     C# 14 Extension Members for Address mapping.
    /// </summary>
    extension(Address address)
    {
        /// <summary>
        ///     Maps an Address value object to an AddressDto.
        /// </summary>
        public AddressDto ToDto() => new(
            address.Street,
            address.City.Value,
            address.PostalCode.Value,
            address.Country);
    }

    /// <summary>
    ///     C# 14 Extension Members for DriversLicense mapping.
    /// </summary>
    extension(DriversLicense license)
    {
        /// <summary>
        ///     Maps a DriversLicense value object to a DriversLicenseDto.
        /// </summary>
        public DriversLicenseDto ToDto() => new(
            license.LicenseNumber,
            license.IssueCountry,
            license.IssueDate,
            license.ExpiryDate,
            license.IsValid(),
            license.IsEuLicense(),
            license.DaysUntilExpiry());
    }

    /// <summary>
    ///     C# 14 Extension Members for SearchCustomersQuery mapping.
    /// </summary>
    extension(SearchCustomersQuery query)
    {
        /// <summary>
        ///     Maps a SearchCustomersQuery to CustomerSearchParameters.
        ///     Direct mapping since query already uses value objects.
        /// </summary>
        public CustomerSearchParameters ToSearchParameters()
        {
            return new CustomerSearchParameters(
                query.SearchTerm,
                query.Email,
                query.PhoneNumber,
                query.Status,
                query.City,
                query.PostalCode,
                query.AgeRange,
                query.LicenseExpiringDays,
                query.RegisteredDateRange,
                query.Paging,
                query.Sorting);
        }
    }
}
