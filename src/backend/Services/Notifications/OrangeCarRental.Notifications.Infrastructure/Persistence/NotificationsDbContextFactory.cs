using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace SmartSolutionsLab.OrangeCarRental.Notifications.Infrastructure.Persistence;

/// <summary>
///     Design-time factory for NotificationsDbContext to support EF Core migrations and database commands.
///     This allows 'dotnet ef' commands to work without running the full Aspire application.
/// </summary>
public class NotificationsDbContextFactory : IDesignTimeDbContextFactory<NotificationsDbContext>
{
    public NotificationsDbContext CreateDbContext(string[] args)
    {
        // Build configuration to read from appsettings.json in the API project
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../OrangeCarRental.Notifications.Api"))
            .AddJsonFile("appsettings.json", false)
            .AddJsonFile("appsettings.Development.json", true)
            .Build();

        var connectionString = configuration.GetConnectionString("NotificationsDatabase");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string 'NotificationsDatabase' not found in appsettings.json");
        }

        var optionsBuilder = new DbContextOptionsBuilder<NotificationsDbContext>();
        optionsBuilder.UseSqlServer(connectionString,
            sqlOptions => sqlOptions.MigrationsAssembly("OrangeCarRental.Notifications.Infrastructure"));

        return new NotificationsDbContext(optionsBuilder.Options);
    }
}
