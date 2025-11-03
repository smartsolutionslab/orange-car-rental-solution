using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.BuildingBlocks.Domain;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence.Repositories;

/// <summary>
/// Entity Framework implementation of ICustomerRepository.
/// Provides data access for Customer aggregates with complex filtering and search capabilities.
/// </summary>
public sealed class CustomerRepository(CustomersDbContext context) : ICustomerRepository
{
    public async Task<Customer?> GetByIdAsync(CustomerId id, CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer?> GetByEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        // Compare value objects directly - EF Core will use the value converter
        return await context.Customers
            .FirstOrDefaultAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsWithEmailAsync(Email email, CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .AnyAsync(c => c.Email == email, cancellationToken);
    }

    public async Task<bool> ExistsWithEmailAsync(
        Email email,
        CustomerId excludeCustomerId,
        CancellationToken cancellationToken = default)
    {
        return await context.Customers
            .AnyAsync(c => c.Email == email && c.Id != excludeCustomerId, cancellationToken);
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
        if (!string.IsNullOrWhiteSpace(parameters.SearchTerm))
        {
            var searchTerm = parameters.SearchTerm.Trim();
            query = query.Where(c =>
                EF.Functions.Like(c.FirstName, $"%{searchTerm}%") ||
                EF.Functions.Like(c.LastName, $"%{searchTerm}%"));
        }

        // Email filter - use EF.Functions for string operations on converted value objects
        if (!string.IsNullOrWhiteSpace(parameters.Email))
        {
            var email = parameters.Email.Trim().ToLowerInvariant();
            // EF Core will translate this using the value converter
            query = query.Where(c => EF.Functions.Like(c.Email, $"%{email}%"));
        }

        // Phone number filter
        if (!string.IsNullOrWhiteSpace(parameters.PhoneNumber))
        {
            var phoneNumber = parameters.PhoneNumber.Trim();
            query = query.Where(c => EF.Functions.Like(c.PhoneNumber, $"%{phoneNumber}%"));
        }

        // Status filter
        if (parameters.Status.HasValue)
        {
            query = query.Where(c => c.Status == parameters.Status.Value);
        }

        // City filter
        if (!string.IsNullOrWhiteSpace(parameters.City))
        {
            var city = parameters.City.Trim();
            query = query.Where(c => EF.Functions.Like(c.Address.City, $"%{city}%"));
        }

        // Postal code filter
        if (!string.IsNullOrWhiteSpace(parameters.PostalCode))
        {
            var postalCode = parameters.PostalCode.Trim();
            query = query.Where(c => c.Address.PostalCode == postalCode);
        }

        // Age range filtering - calculated from DateOfBirth
        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        if (parameters.MinAge.HasValue)
        {
            // Calculate the maximum date of birth for minimum age
            var maxDateOfBirth = today.AddYears(-parameters.MinAge.Value);
            query = query.Where(c => c.DateOfBirth <= maxDateOfBirth);
        }

        if (parameters.MaxAge.HasValue)
        {
            // Calculate the minimum date of birth for maximum age
            // Add 1 to MaxAge to include people who haven't had their birthday yet
            var minDateOfBirth = today.AddYears(-(parameters.MaxAge.Value + 1));
            query = query.Where(c => c.DateOfBirth >= minDateOfBirth);
        }

        // License expiry filtering - find licenses expiring within N days
        if (parameters.LicenseExpiringWithinDays.HasValue)
        {
            var expiryThreshold = today.AddDays(parameters.LicenseExpiringWithinDays.Value);
            query = query.Where(c =>
                c.DriversLicense.ExpiryDate >= today &&
                c.DriversLicense.ExpiryDate <= expiryThreshold);
        }

        // Registration date range filtering
        if (parameters.RegisteredFrom.HasValue)
        {
            query = query.Where(c => c.RegisteredAtUtc >= parameters.RegisteredFrom.Value);
        }

        if (parameters.RegisteredTo.HasValue)
        {
            query = query.Where(c => c.RegisteredAtUtc <= parameters.RegisteredTo.Value);
        }

        // Get total count before pagination
        var totalCount = await query.CountAsync(cancellationToken);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<Customer>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    public async Task AddAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        await context.Customers.AddAsync(customer, cancellationToken);
    }

    public Task UpdateAsync(Customer customer, CancellationToken cancellationToken = default)
    {
        context.Customers.Update(customer);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(CustomerId id, CancellationToken cancellationToken = default)
    {
        var customer = await GetByIdAsync(id, cancellationToken);
        if (customer != null)
        {
            context.Customers.Remove(customer);
        }
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await context.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Applies sorting to the query based on the specified field and direction.
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
                sortDescending ? query.OrderByDescending(c => c.FirstName) : query.OrderBy(c => c.FirstName),

            "lastname" or "last_name" =>
                sortDescending ? query.OrderByDescending(c => c.LastName) : query.OrderBy(c => c.LastName),

            "email" =>
                sortDescending ? query.OrderByDescending(c => c.Email) : query.OrderBy(c => c.Email),

            "phonenumber" or "phone_number" or "phone" =>
                sortDescending ? query.OrderByDescending(c => c.PhoneNumber) : query.OrderBy(c => c.PhoneNumber),

            "dateofbirth" or "date_of_birth" or "birthdate" =>
                sortDescending ? query.OrderByDescending(c => c.DateOfBirth) : query.OrderBy(c => c.DateOfBirth),

            "city" =>
                sortDescending ? query.OrderByDescending(c => c.Address.City) : query.OrderBy(c => c.Address.City),

            "postalcode" or "postal_code" or "zip" =>
                sortDescending ? query.OrderByDescending(c => c.Address.PostalCode) : query.OrderBy(c => c.Address.PostalCode),

            "status" =>
                sortDescending ? query.OrderByDescending(c => c.Status) : query.OrderBy(c => c.Status),

            "registeredat" or "registered_at" or "created" or "createdat" =>
                sortDescending ? query.OrderByDescending(c => c.RegisteredAtUtc) : query.OrderBy(c => c.RegisteredAtUtc),

            "updatedat" or "updated_at" or "modified" or "modifiedat" =>
                sortDescending ? query.OrderByDescending(c => c.UpdatedAtUtc) : query.OrderBy(c => c.UpdatedAtUtc),

            "licenseexpiry" or "license_expiry" or "expirydate" =>
                sortDescending ? query.OrderByDescending(c => c.DriversLicense.ExpiryDate) : query.OrderBy(c => c.DriversLicense.ExpiryDate),

            // Default: sort by registration date
            _ => query.OrderByDescending(c => c.RegisteredAtUtc)
        };
    }
}
