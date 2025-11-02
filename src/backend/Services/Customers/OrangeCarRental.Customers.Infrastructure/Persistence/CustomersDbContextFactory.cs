using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for creating CustomersDbContext instances.
/// Used by EF Core tools for migrations.
/// </summary>
public class CustomersDbContextFactory : IDesignTimeDbContextFactory<CustomersDbContext>
{
    public CustomersDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CustomersDbContext>();

        // Use SQL Server with a connection string for design-time operations
        optionsBuilder.UseSqlServer(
            "Server=localhost;Database=OrangeCarRental_Customers;Trusted_Connection=True;TrustServerCertificate=True;",
            b => b.MigrationsAssembly(typeof(CustomersDbContext).Assembly.FullName));

        return new CustomersDbContext(optionsBuilder.Options);
    }
}
