using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Infrastructure.Extensions;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence.Repositories;

/// <summary>
///     Entity Framework implementation of ICustomerRepository.
///     Provides data access for Customer aggregates with complex filtering and search capabilities.
/// </summary>
public sealed class CustomerRepository(CustomersDbContext context) : ICustomerRepository
{
    private DbSet<Customer> Customers => context.Customers;

    public async Task<Customer> GetByIdAsync(
        CustomerIdentifier id,
        CancellationToken cancellationToken = default)
    {
        var customer = await Customers.FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return customer ?? throw new EntityNotFoundException(typeof(Customer), id);
    }

    public async Task<Customer> GetByEmailAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        // Compare value objects directly - EF Core will use the value converter
        var customer = await Customers.FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

        return customer ?? throw new EntityNotFoundException(typeof(Customer), email);
    }

    public async Task<bool> ExistsWithEmailAsync(
        Email email,
        CancellationToken cancellationToken = default)
    {
        return await Customers.AnyAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsWithEmailAsync(
        Email email,
        CustomerIdentifier excludeCustomerIdentifier,
        CancellationToken cancellationToken = default)
    {
        return await Customers.AnyAsync(c => c.Email == email && c.Id != excludeCustomerIdentifier, cancellationToken);
    }

    public async Task<IReadOnlyList<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await Customers
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public IAsyncEnumerable<Customer> StreamAllAsync(CancellationToken cancellationToken = default)
    {
        return Customers
            .AsNoTracking()
            .AsAsyncEnumerable();
    }

    public async Task<PagedResult<Customer>> SearchAsync(
        CustomerSearchParameters parameters,
        CancellationToken cancellationToken = default)
    {
        parameters.Validate();

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var query = Customers
            .AsNoTracking()
            .ApplyFilters(parameters, today)
            .ApplySorting(parameters.Sorting, SortFieldSelectors, c => c.RegisteredAtUtc, defaultDescending: true);

        return await query.ToPagedResultAsync(parameters.Paging, cancellationToken);
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default) =>
        await context.Customers.AddAsync(customer, cancellationToken);

    public Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        context.Customers.Update(customer);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(CustomerIdentifier id, CancellationToken cancellationToken = default)
    {
        var customer = await GetByIdAsync(id, cancellationToken);
        context.Customers.Remove(customer);
    }

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
        context.SaveChangesAsync(cancellationToken);

    /// <summary>
    ///     Sort field selectors for customer queries.
    /// </summary>
    private static readonly Dictionary<string, Expression<Func<Customer, object?>>> SortFieldSelectors = new(StringComparer.OrdinalIgnoreCase)
    {
        [CustomerSortFields.FirstName] = c => c.Name.FirstName,
        [CustomerSortFields.FirstNameAlt] = c => c.Name.FirstName,
        [CustomerSortFields.LastName] = c => c.Name.LastName,
        [CustomerSortFields.LastNameAlt] = c => c.Name.LastName,
        [CustomerSortFields.Email] = c => c.Email,
        [CustomerSortFields.PhoneNumber] = c => c.PhoneNumber,
        [CustomerSortFields.PhoneNumberAlt] = c => c.PhoneNumber,
        [CustomerSortFields.Phone] = c => c.PhoneNumber,
        [CustomerSortFields.DateOfBirth] = c => c.DateOfBirth,
        [CustomerSortFields.DateOfBirthAlt] = c => c.DateOfBirth,
        [CustomerSortFields.BirthDate] = c => c.DateOfBirth,
        [CustomerSortFields.City] = c => c.Address.City,
        [CustomerSortFields.PostalCode] = c => c.Address.PostalCode,
        [CustomerSortFields.PostalCodeAlt] = c => c.Address.PostalCode,
        [CustomerSortFields.Zip] = c => c.Address.PostalCode,
        [CustomerSortFields.Status] = c => c.Status,
        [CustomerSortFields.RegisteredAt] = c => c.RegisteredAtUtc,
        [CustomerSortFields.RegisteredAtAlt] = c => c.RegisteredAtUtc,
        [CustomerSortFields.Created] = c => c.RegisteredAtUtc,
        [CustomerSortFields.CreatedAt] = c => c.RegisteredAtUtc,
        [CustomerSortFields.UpdatedAt] = c => c.UpdatedAtUtc,
        [CustomerSortFields.UpdatedAtAlt] = c => c.UpdatedAtUtc,
        [CustomerSortFields.Modified] = c => c.UpdatedAtUtc,
        [CustomerSortFields.ModifiedAt] = c => c.UpdatedAtUtc,
        [CustomerSortFields.LicenseExpiry] = c => c.DriversLicense.ExpiryDate,
        [CustomerSortFields.LicenseExpiryAlt] = c => c.DriversLicense.ExpiryDate,
        [CustomerSortFields.ExpiryDate] = c => c.DriversLicense.ExpiryDate
    };
}

/// <summary>
///     Filter extension methods for Customer queries.
/// </summary>
internal static class CustomerQueryExtensions
{
    /// <summary>
    ///     Applies all filters from CustomerSearchParameters to the query.
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
                EF.Functions.Like(c.Name.FirstName.Value, $"%{searchTermValue}%") ||
                EF.Functions.Like(c.Name.LastName.Value, $"%{searchTermValue}%"));
        }

        // Direct value object filters
        query = query
            .WhereIf(parameters.Email is not null, c => c.Email == parameters.Email)
            .WhereIf(parameters.PhoneNumber is not null, c => c.PhoneNumber == parameters.PhoneNumber)
            .WhereIf(parameters.Status.HasValue, c => c.Status == parameters.Status!.Value)
            .WhereIf(parameters.City is not null, c => c.Address.City == parameters.City!.Value)
            .WhereIf(parameters.PostalCode is not null, c => c.Address.PostalCode == parameters.PostalCode!.Value);

        // Age range filtering - calculated from DateOfBirth
        if (parameters.AgeRange is { HasFilter: true })
        {
            if (parameters.AgeRange.Min.HasValue)
            {
                var maxDateOfBirth = today.AddYears(-parameters.AgeRange.Min.Value);
                query = query.Where(c => c.DateOfBirth.Value <= maxDateOfBirth);
            }

            if (parameters.AgeRange.Max.HasValue)
            {
                var minDateOfBirth = today.AddYears(-(parameters.AgeRange.Max.Value + 1));
                query = query.Where(c => c.DateOfBirth.Value >= minDateOfBirth);
            }
        }

        // License expiry filtering
        if (parameters.LicenseExpiringDays is { HasFilter: true } && parameters.LicenseExpiringDays.Max.HasValue)
        {
            var expiryThreshold = today.AddDays(parameters.LicenseExpiringDays.Max.Value);
            query = query.Where(c =>
                c.DriversLicense.ExpiryDate >= today &&
                c.DriversLicense.ExpiryDate <= expiryThreshold);
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
