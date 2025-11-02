using Microsoft.EntityFrameworkCore;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Data;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Persistence;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Api.Extensions;

public static class DatabaseExtensions
{
    /// <summary>
    /// Runs pending migrations and exits (for container init jobs).
    /// </summary>
    public static async Task<int> RunMigrationsAndExitAsync<TContext>(this WebApplication app)
        where TContext : DbContext
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();

            logger.LogInformation("Running database migrations for {Context}...", typeof(TContext).Name);
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations completed successfully for {Context}.", typeof(TContext).Name);

            return 0; // Success
        }
        catch (Exception ex)
        {
            var logger = app.Services.GetRequiredService<ILogger<TContext>>();
            logger.LogError(ex, "An error occurred while migrating the database for {Context}.", typeof(TContext).Name);
            return 1; // Failure
        }
    }

    /// <summary>
    /// Applies pending database migrations (auto-migration for development/Aspire).
    /// </summary>
    public static async Task MigrateDatabaseAsync<TContext>(this WebApplication app)
        where TContext : DbContext
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<TContext>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();

        try
        {
            var pendingMigrations = await context.Database.GetPendingMigrationsAsync();
            if (pendingMigrations.Any())
            {
                logger.LogInformation("Applying {Count} pending migrations for {Context}...",
                    pendingMigrations.Count(), typeof(TContext).Name);
                await context.Database.MigrateAsync();
                logger.LogInformation("Database migrations applied successfully for {Context}.", typeof(TContext).Name);
            }
            else
            {
                logger.LogInformation("No pending migrations for {Context}.", typeof(TContext).Name);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database for {Context}.", typeof(TContext).Name);
            throw;
        }
    }

    /// <summary>
    /// Seeds the customers database with sample data (development only).
    /// </summary>
    public static async Task SeedCustomersDataAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<CustomerDataSeeder>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

        try
        {
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the Customers database.");
            throw;
        }
    }
}
