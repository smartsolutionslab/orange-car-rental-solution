using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SmartSolutionsLab.OrangeCarRental.Location.Infrastructure.Persistence;

public class LocationsDbContextFactory : IDesignTimeDbContextFactory<LocationsDbContext>
{
    public LocationsDbContext CreateDbContext(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../OrangeCarRental.Location.Api"))
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var connectionString = configuration.GetConnectionString("LocationsDatabase");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'LocationsDatabase' not found in appsettings.json");
        }

        var optionsBuilder = new DbContextOptionsBuilder<LocationsDbContext>();
        optionsBuilder.UseSqlServer(connectionString,
            sqlOptions => sqlOptions.MigrationsAssembly("OrangeCarRental.Location.Infrastructure"));

        return new LocationsDbContext(optionsBuilder.Options);
    }
}
