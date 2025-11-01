using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

/// <summary>
/// Design-time factory for FleetDbContext to support EF Core migrations and database commands.
/// This allows 'dotnet ef' commands to work without running the full Aspire application.
/// </summary>
public class FleetDbContextFactory : IDesignTimeDbContextFactory<FleetDbContext>
{
    public FleetDbContext CreateDbContext(string[] args)
    {
        // Build configuration to read from appsettings.json in the API project
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../OrangeCarRental.Fleet.Api"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var connectionString = configuration.GetConnectionString("FleetDatabase");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'FleetDatabase' not found in appsettings.json");
        }

        var optionsBuilder = new DbContextOptionsBuilder<FleetDbContext>();
        optionsBuilder.UseSqlServer(connectionString,
            sqlOptions => sqlOptions.MigrationsAssembly("OrangeCarRental.Fleet.Infrastructure"));

        return new FleetDbContext(optionsBuilder.Options);
    }
}
