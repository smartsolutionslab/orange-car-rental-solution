using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Data;

namespace SmartSolutionsLab.OrangeCarRental.Pricing.Infrastructure.Extensions;

/// <summary>
///     Extension methods for database operations.
///     Provides methods for data seeding.
/// </summary>
public static class DatabaseExtensions
{
    /// <summary>
    ///     Seeds the pricing database with sample data (development only).
    /// </summary>
    public static async Task SeedPricingDataAsync(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var seeder = scope.ServiceProvider.GetRequiredService<PricingDataSeeder>();
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<PricingDataSeeder>>();

        try
        {
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the Pricing database.");
            throw;
        }
    }
}
