using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Data;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Extensions;

/// <summary>
///     Extension methods for database operations.
///     Provides methods for data seeding.
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
        ///     Seeds the pricing database with sample data (development only).
        ///     Includes retry logic to wait for migrations to complete.
        /// </summary>
        public async Task SeedPricingDataAsync()
        {
            using var scope = services.CreateScope();
            var seeder = scope.ServiceProvider.GetRequiredService<PricingDataSeeder>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<PricingDataSeeder>>();

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
                    logger.LogError(ex, "An error occurred while seeding the Pricing database.");
                    throw;
                }
            }
        }
    }
}
