using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Data;

namespace SmartSolutionsLab.OrangeCarRental.Customers.Infrastructure.Extensions;

/// <summary>
///     Extension methods for database operations.
///     Provides methods for migrations and data seeding.
///     Uses C# 14 Extension Members syntax.
/// </summary>
public static class DatabaseExtensions
{
    private const int MaxRetries = 90;
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(2);

    /// <summary>
    ///     C# 14 Extension Members for IServiceProvider database operations.
    /// </summary>
    extension(IServiceProvider services)
    {
        /// <summary>
        ///     Applies pending database migrations for the specified context.
        ///     Auto-migration for development/Aspire environments.
        /// </summary>
        public async Task MigrateDatabaseAsync<TContext>()
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
        ///     Includes retry logic to wait for migrations to complete.
        /// </summary>
        public async Task SeedCustomersDataAsync()
        {
            using var scope = services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<CustomerDataSeeder>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<CustomerDataSeeder>>();

            for (var attempt = 1; attempt <= MaxRetries; attempt++)
            {
                try
                {
                    await seeder.SeedAsync();
                    return;
                }
                catch (SqlException ex) when (ex.Number == 208) // Invalid object name - table doesn't exist
                {
                    if (attempt == MaxRetries)
                    {
                        logger.LogError(ex, "Database schema not available after {MaxRetries} attempts. Seeding failed.", MaxRetries);
                        throw;
                    }

                    logger.LogWarning("Database schema not ready (attempt {Attempt}/{MaxRetries}). Waiting for migrations...", attempt, MaxRetries);
                    await Task.Delay(RetryDelay);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "An error occurred while seeding the Customers database.");
                    throw;
                }
            }
        }

        /// <summary>
        ///     Runs pending migrations and exits (for container init jobs).
        /// </summary>
        public async Task<int> RunMigrationsAndExitAsync<TContext>()
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
}
