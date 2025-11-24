using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

/// <summary>
///     Design-time factory for FleetDbContext to support EF Core migrations.
///     This is only used during design-time operations (dotnet ef migrations).
/// </summary>
public class DesignTimeFleetDbContextFactory : IDesignTimeDbContextFactory<FleetDbContext>
{
    public FleetDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<FleetDbContext>();

        // Use in-memory database for design-time migrations
        // The actual connection string is injected at runtime via Aspire/environment variables
        optionsBuilder.UseSqlServer(
            "Server=(localdb)\\mssqllocaldb;Database=OrangeCarRental_Fleet_DesignTime;Trusted_Connection=True;MultipleActiveResultSets=true",
            sqlOptions => sqlOptions.MigrationsHistoryTable("__EFMigrationsHistory", "fleet")
        );

        return new FleetDbContext(optionsBuilder.Options);
    }
}
