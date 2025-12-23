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

    /// <summary>
    ///     Sort field selectors for customer queries.
    ///     Note: .Value used to access nullable struct properties as EF Core queries operate
    ///     on persisted data where these properties are guaranteed to be non-null.
    /// </summary>
    private static readonly Dictionary<string, Expression<Func<Customer, object?>>> SortFieldSelectors = new(StringComparer.OrdinalIgnoreCase)
    {
        ["firstname"] = c => c.Name!.Value.FirstName,
        ["first_name"] = c => c.Name!.Value.FirstName,
        ["lastname"] = c => c.Name!.Value.LastName,
        ["last_name"] = c => c.Name!.Value.LastName,
        ["email"] = c => c.Email!.Value,
        ["phonenumber"] = c => c.PhoneNumber!.Value,
        ["phone_number"] = c => c.PhoneNumber!.Value,
        ["phone"] = c => c.PhoneNumber!.Value,
        ["dateofbirth"] = c => c.DateOfBirth!.Value,
        ["date_of_birth"] = c => c.DateOfBirth!.Value,
        ["birthdate"] = c => c.DateOfBirth!.Value,
        ["city"] = c => c.Address!.Value.City,
        ["postalcode"] = c => c.Address!.Value.PostalCode,
        ["postal_code"] = c => c.Address!.Value.PostalCode,
        ["zip"] = c => c.Address!.Value.PostalCode,
        ["status"] = c => c.Status,
        ["registeredat"] = c => c.RegisteredAtUtc,
        ["registered_at"] = c => c.RegisteredAtUtc,
        ["created"] = c => c.RegisteredAtUtc,
        ["createdat"] = c => c.RegisteredAtUtc,
        ["updatedat"] = c => c.UpdatedAtUtc,
        ["updated_at"] = c => c.UpdatedAtUtc,
        ["modified"] = c => c.UpdatedAtUtc,
        ["modifiedat"] = c => c.UpdatedAtUtc,
        ["licenseexpiry"] = c => c.DriversLicense!.Value.ExpiryDate,
        ["license_expiry"] = c => c.DriversLicense!.Value.ExpiryDate,
        ["expirydate"] = c => c.DriversLicense!.Value.ExpiryDate
    };
}

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
