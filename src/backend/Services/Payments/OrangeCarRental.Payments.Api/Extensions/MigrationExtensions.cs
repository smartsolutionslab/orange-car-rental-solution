using Microsoft.EntityFrameworkCore;

namespace SmartSolutionsLab.OrangeCarRental.Payments.Api.Extensions;

public static class MigrationExtensions
{
    public static async Task<WebApplication> MigrateDatabaseAsync<TContext>(
        this WebApplication app,
        ILogger? logger = null) where TContext : DbContext
    {
        logger ??= app.Services.GetRequiredService<ILogger<TContext>>();

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

            if (app.Environment.IsDevelopment()) throw;

            logger.LogWarning(
                "Application will continue, but database may not be up to date. " +
                "Ensure migrations are applied via migration job.");
        }

        return app;
    }

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
                return 0;
            }

            logger.LogInformation("Database is already up to date. No migrations to apply.");
            return 0;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Migration job failed for {DbContext}", typeof(TContext).Name);
            return 1;
        }
    }
}
