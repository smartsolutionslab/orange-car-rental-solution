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
        // Design-time connection string for EF Core migrations
        // This is only used for generating migrations, not at runtime
        const string designTimeConnectionString = "Server=localhost;Database=OrangeCarRental_Notifications;Trusted_Connection=True;TrustServerCertificate=True";

        var optionsBuilder = new DbContextOptionsBuilder<NotificationsDbContext>();
        optionsBuilder.UseSqlServer(designTimeConnectionString,
            sqlOptions => sqlOptions.MigrationsAssembly("OrangeCarRental.Notifications.Infrastructure"));

        return new NotificationsDbContext(optionsBuilder.Options);
    }
}
