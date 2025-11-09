using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain.Exceptions;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence.Repositories;

/// <summary>
///     Entity Framework implementation of ICustomerRepository.
///     Provides data access for Customer aggregates with complex filtering and search capabilities.
/// </summary>
public sealed class CustomerRepository(CustomersDbContext context) : ICustomerRepository
{
    public async Task<Customer> GetByIdAsync(CustomerIdentifier id, CancellationToken cancellationToken = default)
    {
        var customer = await context.Customers
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);

        return customer ?? throw new EntityNotFoundException(typeof(Customer), id);
    }

    public async Task<Customer> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        // Compare value objects directly - EF Core will use the value converter
        var customer = await context.Customers
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);

        return customer ?? throw new EntityNotFoundException(typeof(Customer), email);
    }

    public async Task<bool> ExistsWithEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .AnyAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsWithEmailAsync(
        Email email,
        CustomerIdentifier excludeCustomerIdentifier,
        CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .AnyAsync(c => c.Email == email && c.Id != excludeCustomerIdentifier, cancellationToken);
    }

    public async Task<List<Customer>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<Customer>> SearchAsync(
        CustomerSearchParameters parameters,
        CancellationToken cancellationToken = default)
    {
        parameters.Validate();

        // Start with base query
        var query = context.Customers.AsNoTracking().AsQueryable();

        // Apply filters using database-level WHERE clauses

        // Name search - search in both FirstName and LastName using LIKE
        if (parameters.SearchTerm is not null)
        {
            var searchTermValue = parameters.SearchTerm.Value.Value;
            query = query.Where(c =>
                EF.Functions.Like(c.Name.FirstName.Value, $"%{searchTermValue}%") ||
                EF.Functions.Like(c.Name.LastName.Value, $"%{searchTermValue}%"));
        }

        // Email filter - use value object directly
        if (parameters.Email is not null) query = query.Where(c => c.Email == parameters.Email);

        // Phone number filter - use value object directly
        if (parameters.PhoneNumber is not null) query = query.Where(c => c.PhoneNumber == parameters.PhoneNumber);

        // Status filter
        if (parameters.Status.HasValue) query = query.Where(c => c.Status == parameters.Status.Value);

        // City filter - use value object directly
        if (parameters.City is not null)
            query = query.Where(c => c.Address.City == parameters.City.Value);

        // Postal code filter - use value object directly
        if (parameters.PostalCode is not null)
            query = query.Where(c => c.Address.PostalCode == parameters.PostalCode.Value);

        // Age range filtering - calculated from DateOfBirth
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (parameters.MinAge.HasValue)
        {
            var maxDateOfBirth = today.AddYears(-parameters.MinAge.Value);
            query = query.Where(c => c.DateOfBirth <= maxDateOfBirth);
        }

        if (parameters.MaxAge.HasValue)
        {
            var minDateOfBirth = today.AddYears(-(parameters.MaxAge.Value + 1));
            query = query.Where(c => c.DateOfBirth >= minDateOfBirth);
        }

        // License expiry filtering
        if (parameters.LicenseExpiringWithinDays.HasValue)
        {
            var expiryThreshold = today.AddDays(parameters.LicenseExpiringWithinDays.Value);
            query = query.Where(c =>
                c.DriversLicense.ExpiryDate >= today &&
                c.DriversLicense.ExpiryDate <= expiryThreshold);
        }

        // Registration date range filtering
        if (parameters.RegisteredFrom.HasValue)
            query = query.Where(c => c.RegisteredAtUtc >= parameters.RegisteredFrom.Value);

        if (parameters.RegisteredTo.HasValue)
            query = query.Where(c => c.RegisteredAtUtc <= parameters.RegisteredTo.Value);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        // Get total count and apply pagination
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip(parameters.Skip)
            .Take(parameters.Take)
            .ToListAsync(cancellationToken);

        return new PagedResult<Customer>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
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
        if (customer != null) context.Customers.Remove(customer);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default) =>
        await context.SaveChangesAsync(cancellationToken);

    /// <summary>
    ///     Applies sorting to the query based on the specified field and direction.
    /// </summary>
    private static IQueryable<Customer> ApplySorting(
        IQueryable<Customer> query,
        string? sortBy,
        bool sortDescending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            // Default sorting: newest first
            return query.OrderByDescending(c => c.RegisteredAtUtc);
        }

        // Normalize sort field name
        var sortField = sortBy.Trim().ToLowerInvariant();

        return sortField switch
        {
            "firstname" or "first_name" =>
                sortDescending ? query.OrderByDescending(c => c.Name.FirstName) : query.OrderBy(c => c.Name.FirstName),

            "lastname" or "last_name" =>
                sortDescending ? query.OrderByDescending(c => c.Name.LastName) : query.OrderBy(c => c.Name.LastName),

            "email" =>
                sortDescending ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),

            "phonenumber" or "phone_number" or "phone" =>
                sortDescending ? query.OrderByDescending(c => c.PhoneNumber) : query.OrderBy(c => c.PhoneNumber),

            "dateofbirth" or "date_of_birth" or "birthdate" =>
                sortDescending ? query.OrderByDescending(c => c.DateOfBirth) : query.OrderBy(c => c.DateOfBirth),

            "city" =>
                sortDescending ? query.OrderByDescending(c => c.Address.City) : query.OrderBy(c => c.Address.City),

            "postalcode" or "postal_code" or "zip" =>
                sortDescending
                    ? query.OrderByDescending(c => c.Address.PostalCode)
                    : query.OrderBy(c => c.Address.PostalCode),

            "status" =>
                sortDescending ? query.OrderByDescending(c => c.Status) : query.OrderBy(c => c.Status),

            "registeredat" or "registered_at" or "created" or "createdat" =>
                sortDescending
                    ? query.OrderByDescending(c => c.RegisteredAtUtc)
                    : query.OrderBy(c => c.RegisteredAtUtc),

            "updatedat" or "updated_at" or "modified" or "modifiedat" =>
                sortDescending ? query.OrderByDescending(c => c.UpdatedAtUtc) : query.OrderBy(c => c.UpdatedAtUtc),

            "licenseexpiry" or "license_expiry" or "expirydate" =>
                sortDescending
                    ? query.OrderByDescending(c => c.DriversLicense.ExpiryDate)
                    : query.OrderBy(c => c.DriversLicense.ExpiryDate),

            // Default: sort by registration date
            _ => query.OrderByDescending(c => c.RegisteredAtUtc)
        };
    }
}
