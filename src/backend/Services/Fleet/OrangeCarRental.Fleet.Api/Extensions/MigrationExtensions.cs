using Microsoft.EntityFrameworkCore;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Api.Extensions;

public static class MigrationExtensions
{
    /// <summary>
    ///     Applies database migrations based on configuration.
    ///     In development/Aspire: runs automatically on startup
    ///     In production/Azure: should be run via separate migration job
    /// </summary>
    public static async Task<WebApplication> MigrateDatabaseAsync<TContext>(
        this WebApplication app,
        ILogger? logger = null) where TContext : DbContext
    {
        logger ??= app.Services.GetRequiredService<ILogger<TContext>>();

        // Check if auto-migration is enabled (default: true for Development, false for Production)
        var configuration = app.Configuration;
        var autoMigrate = configuration.GetValue<bool?>("Database:AutoMigrate")
                          ?? app.Environment.IsDevelopment();

        if (!autoMigrate)
        {
            logger.LogInformation(
                "Auto-migration is disabled for {DbContext}. Database migrations must be applied manually or via migration job.",
                typeof(TContext).Name);
            return app;
        }

        try
        {
            logger.LogInformation("Applying database migrations for {DbContext}...", typeof(TContext).Name);

            await using var scope = app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation(
                    "Found {Count} pending migrations for {DbContext}: {Migrations}",
                    pendingMigrations.Count(),
                    typeof(TContext).Name,
                    string.Join(", ", pendingMigrations));

                await dbContext.Database.MigrateAsync();

                logger.LogInformation(
                    "Successfully applied migrations for {DbContext}",
                    typeof(TContext).Name);
            }
            else
            {
                logger.LogInformation(
                    "No pending migrations for {DbContext}. Database is up to date.",
                    typeof(TContext).Name);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "An error occurred while migrating the database for {DbContext}",
                typeof(TContext).Name);

            // In development, we want to fail fast
            if (app.Environment.IsDevelopment()) throw;

            // In production, log but don't crash (assuming migration job will handle it)
            logger.LogWarning(
                "Application will continue, but database may not be up to date. " +
                "Ensure migrations are applied via migration job.");
        }

        return app;
    }

    /// <summary>
    ///     Runs database migrations and exits. Use this for migration-only jobs in Azure.
    /// </summary>
    public static async Task<int> RunMigrationsAndExitAsync<TContext>(
        this WebApplication app) where TContext : DbContext
    {
        var logger = app.Services.GetRequiredService<ILogger<TContext>>();

        try
        {
            logger.LogInformation("Starting migration job for {DbContext}...", typeof(TContext).Name);

            await using var scope = app.Services.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

            var pendingMigrations = await dbContext.Database.GetPendingMigrationsAsync();

            if (pendingMigrations.Any())
            {
                logger.LogInformation(
                    "Applying {Count} pending migrations: {Migrations}",
                    pendingMigrations.Count(),
                    string.Join(", ", pendingMigrations));

                await dbContext.Database.MigrateAsync();

                logger.LogInformation("Migrations applied successfully!");
                return 0; // Success
            }

            logger.LogInformation("Database is already up to date. No migrations to apply.");
            return 0; // Success
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Migration job failed for {DbContext}", typeof(TContext).Name);
            return 1; // Failure
        }
    }
}
