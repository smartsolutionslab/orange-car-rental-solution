using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence.Repositories;

/// <summary>
///     Filter extension methods for Customer queries.
/// </summary>
internal static class CustomerQueryExtensions
{
    /// <summary>
    ///     Applies all filters from CustomerSearchParameters to the query.
    ///     Note: Null-forgiving operators used as EF Core queries operate on persisted data
    ///     where these properties are guaranteed to be non-null.
    /// </summary>
    public static IQueryable<Customer> ApplyFilters(
        this IQueryable<Customer> query,
        CustomerSearchParameters parameters,
        DateOnly today)
    {
        // Name search - search in both FirstName and LastName using LIKE
        if (parameters.SearchTerm is not null)
        {
            var searchTermValue = parameters.SearchTerm.Value.Value;
            query = query.Where(c =>
                EF.Functions.Like(c.Name!.Value.FirstName.Value, $"%{searchTermValue}%") ||
                EF.Functions.Like(c.Name!.Value.LastName.Value, $"%{searchTermValue}%"));
        }

        // Direct value object filters
        query = query
            .WhereIf(parameters.Email is not null, c => c.Email == parameters.Email)
            .WhereIf(parameters.PhoneNumber is not null, c => c.PhoneNumber == parameters.PhoneNumber)
            .WhereIf(parameters.Status.HasValue, c => c.Status == parameters.Status!.Value)
            .WhereIf(parameters.City is not null, c => c.Address!.Value.City == parameters.City!.Value)
            .WhereIf(parameters.PostalCode is not null, c => c.Address!.Value.PostalCode == parameters.PostalCode!.Value);

        // Age range filtering - calculated from DateOfBirth
        if (parameters.AgeRange is { HasFilter: true })
        {
            if (parameters.AgeRange.Min.HasValue)
            {
                var maxDateOfBirth = today.AddYears(-parameters.AgeRange.Min.Value);
                query = query.Where(c => c.DateOfBirth!.Value <= maxDateOfBirth);
            }

            if (parameters.AgeRange.Max.HasValue)
            {
                var minDateOfBirth = today.AddYears(-(parameters.AgeRange.Max.Value + 1));
                query = query.Where(c => c.DateOfBirth!.Value >= minDateOfBirth);
            }
        }

        // License expiry filtering
        if (parameters.LicenseExpiringDays is { HasFilter: true } && parameters.LicenseExpiringDays.Max.HasValue)
        {
            var expiryThreshold = today.AddDays(parameters.LicenseExpiringDays.Max.Value);
            query = query.Where(c =>
                c.DriversLicense!.Value.ExpiryDate >= today &&
                c.DriversLicense!.Value.ExpiryDate <= expiryThreshold);
        }

        // Registration date range filtering
        if (parameters.RegisteredDateRange is { HasFilter: true })
        {
            if (parameters.RegisteredDateRange.From.HasValue)
            {
                var fromDate = parameters.RegisteredDateRange.From.Value.ToDateTime(TimeOnly.MinValue);
                query = query.Where(c => c.RegisteredAtUtc >= fromDate);
            }

            if (parameters.RegisteredDateRange.To.HasValue)
            {
                var toDate = parameters.RegisteredDateRange.To.Value.ToDateTime(TimeOnly.MaxValue);
                query = query.Where(c => c.RegisteredAtUtc <= toDate);
            }
        }

        return query;
    }
}
