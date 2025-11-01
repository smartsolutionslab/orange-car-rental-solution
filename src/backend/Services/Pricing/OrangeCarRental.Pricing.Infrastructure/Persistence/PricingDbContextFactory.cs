using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for creating PricingDbContext instances.
/// Used by EF Core tools for migrations.
/// </summary>
public class PricingDbContextFactory : IDesignTimeDbContextFactory<PricingDbContext>
{
    public PricingDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<PricingDbContext>();

        // Use SQL Server with a connection string for design-time operations
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=OrangeCarRental_Pricing;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True",
            b => b.MigrationsAssembly(typeof(PricingDbContext).Assembly.FullName));

        return new PricingDbContext(optionsBuilder.Options);
    }
}
