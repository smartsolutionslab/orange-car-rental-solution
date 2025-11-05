using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Data;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Extensions;

/// <summary>
///     Extension methods for database operations.
///     Provides methods for migrations and data seeding.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    ///     Applies pending database migrations for the specified context.
    ///     Auto-migration for development/Aspire environments.
    /// </summary>
    public static async Task MigrateDatabaseAsync<TContext>(this IServiceProvider services)
        where TContext : DbContext
    {
        using var scope = services.CreateScope();
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
                logger.LogInformation("No pending migrations for {Context}.", typeof(TContext).Name);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while migrating the database for {Context}.", typeof(TContext).Name);
            throw;
        }
    }

    /// <summary>
    ///     Seeds the customers database with sample data (development only).
    /// </summary>
    public static async Task SeedCustomersDataAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<CustomerDataSeeder>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomerDataSeeder>>();

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

    /// <summary>
    ///     Runs pending migrations and exits (for container init jobs).
    /// </summary>
    public static async Task<int> RunMigrationsAndExitAsync<TContext>(this IServiceProvider services)
        where TContext : DbContext
    {
        try
        {
            using var scope = services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<TContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();

            logger.LogInformation("Running database migrations for {Context}...", typeof(TContext).Name);
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations completed successfully for {Context}.", typeof(TContext).Name);

            return 0; // Success
        }
        catch (Exception ex)
        {
            using var scope = services.CreateScope();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<TContext>>();
            logger.LogError(ex, "An error occurred while migrating the database for {Context}.", typeof(TContext).Name);
            return 1; // Failure
        }
    }
}
