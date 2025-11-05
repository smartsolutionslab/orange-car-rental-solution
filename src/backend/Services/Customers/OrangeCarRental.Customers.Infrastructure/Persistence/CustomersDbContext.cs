using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.Customers.Domain.Customer;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence.Configurations;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence;

/// <summary>
///     Database context for the Customers service.
///     Manages customer data and profile operations.
/// </summary>
public sealed class CustomersDbContext : DbContext
{
    public CustomersDbContext(DbContextOptions<CustomersDbContext> options) : base(options)
    {
    }

    public DbSet<Customer> Customers => Set<Customer>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfiguration(new CustomerConfiguration());

        // Set default schema
        modelBuilder.HasDefaultSchema("customers");
    }
}
