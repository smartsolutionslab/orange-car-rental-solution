using SmartSolutionsLab.OrangeCarRental.Fleet.Domain.Repositories;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Data;
using SmartSolutionsLab.OrangeCarRental.Fleet.Infrastructure.Persistence;

namespace SmartSolutionsLab.OrangeCarRental.Fleet.Api.Extensions;

/// <summary>
/// Extension methods for data seeding in the Fleet API.
/// </summary>
public static class DataSeedingExtensions
{
    /// <summary>
    /// Seeds the Fleet database with sample data if running in Development environment.
    /// </summary>
    public static async Task SeedFleetDataAsync(this IApplicationBuilder app)
    {
        using var scope = app.ApplicationServices.CreateScope();
        var services = scope.ServiceProvider;
        var environment = services.GetRequiredService<IHostEnvironment>();

        // Only seed in development
        if (!environment.IsDevelopment())
        {
            return;
        }

        var logger = services.GetRequiredService<ILogger<FleetDataSeeder>>();

        try
        {
            var context = services.GetRequiredService<FleetDbContext>();
            var repository = services.GetRequiredService<IVehicleRepository>();

            var seeder = new FleetDataSeeder(context, repository, logger);
            await seeder.SeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the Fleet database.");
            throw;
        }
    }
}
