using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SmartSolutionsLab.OrangeCarRental.Location.Infrastructure.Persistence;

public class LocationsDbContextFactory : IDesignTimeDbContextFactory<LocationsDbContext>
{
    public LocationsDbContext CreateDbContext(string[] args)
    {
        // Design-time connection string for EF Core migrations
        // This is only used for generating migrations, not at runtime
        const string designTimeConnectionString = "Server=localhost;Database=OrangeCarRental_Locations;Trusted_Connection=True;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<LocationsDbContext>();
        optionsBuilder.UseSqlServer(designTimeConnectionString,
            sqlOptions => sqlOptions.MigrationsAssembly("OrangeCarRental.Location.Infrastructure"));

        return new LocationsDbContext(optionsBuilder.Options);
    }
}
